using Microsoft.Win32;
using proj;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WPFApp_Navigation
{
    public partial class YourInformationPage : Page
    {
        private readonly string _originalEmail;
        private readonly Data db = new Data();
        private string _currentProfilePath;

        public YourInformationPage(string email)
        {
            InitializeComponent();
            _originalEmail = email;
            EmailTextBox.Text = email;
            _currentProfilePath = db.GetProfilePicturePath(_originalEmail);
            LoadProfilePicture();
        }

        private void LoadProfilePicture()
        {
            try
            {
                string fullPath = null;
                if (!string.IsNullOrEmpty(_currentProfilePath))
                {
                    fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _currentProfilePath);
                }

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = File.Exists(fullPath)
                    ? new Uri(fullPath, UriKind.Absolute)
                    : new Uri("pack://application:,,,/icon.jpg", UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                ProfileImageBrush.ImageSource = image;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load profile image: " + ex.Message);
            }
        }

        private void ChangeProfilePicture_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                FileInfo file = new FileInfo(dialog.FileName);
                if (file.Length > 2 * 1024 * 1024)
                {
                    MessageBox.Show("Image must be under 2MB.");
                    return;
                }

                string profilePicturesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProfilePictures");
                Directory.CreateDirectory(profilePicturesFolder);

                string newFileName = Guid.NewGuid().ToString() + file.Extension;
                string destRelative = Path.Combine("ProfilePictures", newFileName).Replace("\\", "/");
                string destFullPath = Path.Combine(profilePicturesFolder, newFileName);

                File.Copy(dialog.FileName, destFullPath, true);

                if (db.ExecuteNonQuery($"UPDATE users SET profilepicturepath = '{destRelative}' WHERE email = '{_originalEmail}'"))
                {
                    _currentProfilePath = destRelative;
                    LoadProfilePicture();
                    MessageBox.Show("Profile picture updated!");
                }
                else
                {
                    MessageBox.Show("Failed to update profile picture in the database.");
                }

            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            string newEmail = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;

            if (newEmail != _originalEmail && db.SelectEmails("SELECT email FROM users").Contains(newEmail))
            {
                MessageBox.Show("Email already exists.");
                return;
            }

            if (PasswordSection.Visibility == Visibility.Visible &&
                (!string.IsNullOrWhiteSpace(password) || !string.IsNullOrWhiteSpace(confirm)))
            {
                if (password.Length < 8)
                {
                    MessageBox.Show("Password must be at least 8 characters.");
                    return;
                }
                if (password != confirm)
                {
                    MessageBox.Show("Passwords do not match.");
                    return;
                }

                string hashed = HashHelper.HashPassword(password);
                db.ExecuteNonQuery($"UPDATE users SET password = '{hashed}' WHERE email = '{_originalEmail}'");
            }

            if (newEmail != _originalEmail)
            {
                db.ExecuteNonQuery($"UPDATE users SET email = '{newEmail}' WHERE email = '{_originalEmail}'");
                MessageBox.Show("Email changed. Please use the new email to log in next time.");
            }
            else
            {
                MessageBox.Show("Changes saved.");
            }
        }

        private void TogglePasswordSection_Click(object sender, RoutedEventArgs e)
        {
            PasswordSection.Visibility =
                PasswordSection.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void GoBack_Click(object sender, MouseButtonEventArgs e)
        {
            string profilePicturePath = db.GetProfilePicturePath(_originalEmail);
            NavigationService?.Navigate(new UserPage(_originalEmail, profilePicturePath));
        }
    }
}
