using proj;
using System.Windows;
using System.Windows.Controls;

namespace WPFApp_Navigation
{
    public partial class DeleteAccountPage : Page
    {
        private readonly string _email;
        private readonly Data _db = new Data();

        public DeleteAccountPage(string email)
        {
            InitializeComponent();
            _email = email;
        }

        private void GoBack_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new UserPage(_email, null));
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you absolutely sure? This cannot be undone.",
                                         "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            string userId = _db.GetSingleValue($"SELECT UserId FROM Users WHERE Email = '{_email}'");

            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("User not found.");
                return;
            }

            // Step-by-step deletion
            _db.ExecuteNonQuery($"DELETE FROM UserDocuments WHERE UserId = '{userId}'");
            _db.ExecuteNonQuery($"DELETE FROM Users WHERE UserId = '{userId}'");

            MessageBox.Show("Account deleted. Goodbye!");
            NavigationService?.Navigate(new WelcomePage());
        }
    }
}
