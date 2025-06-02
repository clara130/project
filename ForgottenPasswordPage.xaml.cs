using proj;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFApp_Navigation
{
    public partial class ForgottenPasswordPage : Page
    {
        private readonly Data db = new Data();

        public ForgottenPasswordPage()
        {
            InitializeComponent();
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string userId = db.GetSingleValue($"SELECT UserId FROM Users WHERE Email = '{email}'");

            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("Email not found.");
                return;
            }

            string tempPassword = Guid.NewGuid().ToString().Substring(0, 8); // or use a better generator
            string hashed = HashHelper.HashPassword(tempPassword);

            if (db.ExecuteNonQuery($"UPDATE Users SET Password = '{hashed}' WHERE Email = '{email}'"))
            {
                if (db.SendTemporaryPassword(email, tempPassword))
                {
                    MessageBox.Show("Temporary password sent! Check your email.");
                    NavigationService?.Navigate(new LoginPage());
                }
                else
                {
                    MessageBox.Show("Failed to send email.");
                }
            }
            else
            {
                MessageBox.Show("Failed to reset password.");
            }
        }


        private void Back_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new LoginPage());
        }


    }
}
