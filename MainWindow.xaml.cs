using System.Windows;

namespace WPFApp_Navigation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new WelcomePage());
        }
    }
}
