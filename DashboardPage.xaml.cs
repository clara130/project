using proj;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFApp_Navigation
{
    public partial class DashboardPage : Page
    {
        private static bool _popupShown = false;
        private readonly string _email;
        private readonly bool _isAdmin;
        private readonly Data _db = new Data();

        public DashboardPage(string email, bool isAdmin = false)
        {
            InitializeComponent();
            _email = email;
            _isAdmin = isAdmin;
            EmailTextBlock.Text = _email;
            LoadProfilePicture();
            LoadDeadlines();

            if (_isAdmin)
                AddAdminButton();
        }

        private void AddAdminButton()
        {
            Button adminBtn = new Button
            {
                Content = "Admin Dashboard",
                Background = Brushes.DarkRed,
                Foreground = Brushes.White,
                Margin = new Thickness(5),
                Height = 35
            };
            adminBtn.Click += (s, e) => NavigationService?.Navigate(new AdminDashboardPage());
            CategoryStackPanel.Children.Add(adminBtn);
        }

        private void LoadProfilePicture()
        {
            try
            {
                string path = _db.GetProfilePicturePath(_email);
                BitmapImage image = new BitmapImage();

                if (!string.IsNullOrEmpty(path))
                {
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
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

                image.BeginInit();
                image.UriSource = new Uri("pack://application:,,,/icon.jpg", UriKind.Absolute);
                image.EndInit();
                ProfileImageBrush.ImageSource = image;
            }
            catch { }
        }

        private void LoadDeadlines()
        {
            var deadlines = _db.GetUpcomingDeadlines(_email);
            DeadlineList.Items.Clear();

            foreach ((string name, DateTime date) in deadlines)
            {
                TextBlock deadlineText = new TextBlock
                {
                    Text = $"{name} - {date:dd MMM yyyy}"
                };

                if ((date - DateTime.Today).TotalDays <= 7)
                {
                    deadlineText.FontWeight = FontWeights.Bold;
                    deadlineText.Foreground = Brushes.Red;
                }

                DeadlineList.Items.Add(deadlineText);
            }

            if (!_popupShown)
            {
                var docsToNotify = _db.GetNotifiableDocuments(_email);
                if (docsToNotify.Count > 0)
                {
                    string message = "You have upcoming deadlines:\n\n";
                    foreach (var (name, date) in docsToNotify)
                    {
                        message += $"- {name} (due {date:dd MMM yyyy})\n";
                    }

                    MessageBox.Show(message, "Reminder", MessageBoxButton.OK, MessageBoxImage.Warning);
                    _popupShown = true;
                }
            }
        }

        private void Logout_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new WelcomePage());
        }

        private void Email_Click(object sender, MouseButtonEventArgs e)
        {
            string profilePath = _db.GetProfilePicturePath(_email);
            NavigationService?.Navigate(new UserPage(_email, profilePath));
        }

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string category)
            {
                NavigationService?.Navigate(new CategoryPage(_email, category));
            }
        }
    }
}
