using System.Windows;
using System.Windows.Controls;

namespace WPFApp_Navigation
{
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginPage());
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new SignUpPage());
        }
    }
}