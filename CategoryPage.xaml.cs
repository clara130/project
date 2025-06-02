using proj;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFApp_Navigation
{
    public partial class CategoryPage : Page
    {
        private readonly string _email;
        private readonly string _category;
        private readonly Data _db = new Data();

        public CategoryPage(string email, string category)
        {
            InitializeComponent();
            _email = email;
            _category = category;
            LoadDocuments();
        }

        private void LoadDocuments()
        {
            DocumentList.Items.Clear();
            var docs = _db.GetDocumentsByCategory(_email, _category);

            foreach (var (title, date, notify) in docs)
            {
                Button row = new Button
                {
                    Content = $"{title}    {(date.HasValue ? date.Value.ToString("dd MMM yyyy") : "")}    {(notify ? "🔔" : "🔕")}",
                    Tag = title,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 5, 0, 5),
                    Padding = new Thickness(5),
                    Background = Brushes.White
                };

                row.Click += Document_Click;
                DocumentList.Items.Add(row);
            }
        }

        private void Document_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string title)
            {
                NavigationService?.Navigate(new DocumentsPage(_email, _category));
            }
        }

        private void AddDocument_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new DocumentsPage(_email, _category));
        }

        private void GoBack_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new DashboardPage(_email));
        }
    }
}