using proj;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPFApp_Navigation
{
    public partial class AdminDashboardPage : Page
    {
        private readonly Data _db = new Data();

        public AdminDashboardPage()
        {
            InitializeComponent();
            LoadStats();
        }

        private void LoadStats()
        {
            string count = _db.GetSingleValue("SELECT COUNT(*) FROM Documents");
            DocumentCountText.Text = count;

            var users = _db.SelectPairs("SELECT UserId, Email FROM Users");
            UserListBox.ItemsSource = users;
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserListBox.SelectedItem is KeyValuePair<string, string> user)
            {
                if (MessageBox.Show($"Delete user {user.Value}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string query = $"DELETE FROM Users WHERE UserId = '{user.Key}'";
                    if (_db.ExecuteNonQuery(query))
                    {
                        MessageBox.Show("User deleted.");
                        LoadStats();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete user.");
                    }
                }
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
