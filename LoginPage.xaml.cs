using proj;
using System.Windows;
using System.Windows.Controls;

namespace WPFApp_Navigation
{
    public partial class LoginPage : Page
    {
        private readonly Data db = new Data();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (email == "wikreplin@gmail.com" && password == "lastcalladmin1")
            {
                NavigationService?.Navigate(new DashboardPage(email, true));
                return;
            }

            string storedPassword = db.GetPasswordByEmail(email);
            if (!string.IsNullOrEmpty(storedPassword) &&
                HashHelper.ComparePasswords(password, storedPassword))
            {
                NavigationService?.Navigate(new DashboardPage(email, false));
            }
            else
            {
                MessageBox.Show("Invalid credentials. Please try again.");
            }
        }

        private void ForgotPassword_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new ForgottenPasswordPage());
        }

        private void Back_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new WelcomePage());
        }
    }
}
