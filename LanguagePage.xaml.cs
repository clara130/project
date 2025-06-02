using System.Windows;
using System.Windows.Controls;

namespace WPFApp_Navigation
{
    public partial class LanguagePage : Page
    {
        private readonly string _email;
        public LanguagePage(string email)
        {
            InitializeComponent();
            _email = email;
        }

        private void GoBack_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new UserPage(_email, null));
        }
    }
}