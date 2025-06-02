using proj;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFApp_Navigation
{
    public partial class SignUpPage : Page
    {
        private readonly Data db = new Data();

        public SignUpPage()
        {
            InitializeComponent();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (password.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            var existing = db.SelectEmails("SELECT Email FROM Users");
            if (existing.Contains(email))
            {
                MessageBox.Show("Email already used.");
                return;
            }

            // Ensure default categories exist
            string[] categories = { "Personal", "House", "Car", "Education" };
            foreach (string cat in categories)
            {
                string exists = db.GetSingleValue($"SELECT CategoryId FROM Categories WHERE Name = '{cat}'");
                if (string.IsNullOrEmpty(exists))
                {
                    string newId = Guid.NewGuid().ToString();
                    db.ExecuteNonQuery($"INSERT INTO Categories (CategoryId, Name) VALUES ('{newId}', '{cat}')");
                }
            }

            string hashed = HashHelper.HashPassword(password);
            string userId = Guid.NewGuid().ToString();
            string insertUserQuery = $"INSERT INTO Users (UserId, Email, Password) VALUES ('{userId}', '{email}', '{hashed}')";

            if (db.ExecuteNonQuery(insertUserQuery))
            {
                NavigationService?.Navigate(new DashboardPage(email));
            }
            else
            {
                MessageBox.Show("Sign-up failed.");
            }

        }

        private void Back_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new WelcomePage());
        }
    }
}
