using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPFApp_Navigation
{
    public partial class UserPage : Page
    {
        private readonly string _email;
        private readonly string _profilePicturePath;

        public UserPage(string email, string profilePicturePath)
        {
            InitializeComponent();
            _email = email;
            _profilePicturePath = profilePicturePath;
            LoadProfilePicture();
        }

        private void LoadProfilePicture()
        {
            try
            {
                BitmapImage image = new BitmapImage();

                if (!string.IsNullOrEmpty(_profilePicturePath))
                {
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _profilePicturePath);
                    if (File.Exists(fullPath))
                    {
                        image.BeginInit();
                        image.UriSource = new Uri(fullPath, UriKind.Absolute);
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                        ProfileImageBrush.ImageSource = image;
                        return;
                    }
                }

                // fallback to default icon
                image.BeginInit();
                image.UriSource = new Uri("pack://application:,,,/icon.jpg", UriKind.Absolute);
                image.EndInit();
                ProfileImageBrush.ImageSource = image;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading profile picture: " + ex.Message);
            }
        }





        private void GoBack_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new DashboardPage(_email));
        }

        private void GoToInformation_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new YourInformationPage(_email));
        private void GoToRingtone_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new RingtonePage(_email));
        private void GoToHelp_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new HelpPage(_email));
        private void GoToPrivacy_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new PrivacyPermissionsPage(_email));
        private void GoToDelete_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new DeleteAccountPage(_email));
    }
}