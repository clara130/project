using Microsoft.Win32;
using proj;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFApp_Navigation
{
    public partial class DocumentsPage : Page
    {
        private readonly string _email;
        private readonly string _category;
        private readonly Data _db = new Data();
        private string _selectedDocumentId = null;

        public DocumentsPage(string email, string category)
        {
            InitializeComponent();
            _email = email;
            _category = category;
            LoadDocuments();
        }

        private void LoadDocuments()
        {
            string query = $@"
                SELECT d.DocumentId, d.Name 
                FROM Documents d
                JOIN UserDocuments ud ON d.DocumentId = ud.DocumentId
                JOIN Users u ON ud.UserId = u.UserId
                JOIN Categories c ON d.CategoryId = c.CategoryId
                WHERE u.Email = '{_email}' AND c.Name = '{_category}'";

            var results = _db.SelectPairs(query);
            DocumentListBox.ItemsSource = results;
            DocumentListBox.DisplayMemberPath = "Value";
            DocumentListBox.SelectedValuePath = "Key";
        }

        private void DocumentListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DocumentListBox.SelectedItem is KeyValuePair<string, string> item)
            {
                _selectedDocumentId = item.Key;
                LoadDocumentDetails(_selectedDocumentId);
                DeleteButton.Visibility = Visibility.Visible;
            }
        }

        private void LoadDocumentDetails(string docId)
        {
            string query = $"SELECT Name, ExpiryDate, Notify, Notes FROM Documents WHERE DocumentId = '{docId}'";
            var result = _db.SelectRow(query);

            if (result != null && result.Count >= 4)
            {
                DocumentNameBox.Text = result[0];
                DeadlinePicker.SelectedDate = string.IsNullOrEmpty(result[1]) ? null : DateTime.Parse(result[1]);
                ReminderCheck.IsChecked = result[2] == "1";
                NotesBox.Text = result[3];
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = DocumentNameBox.Text.Trim();
            string notes = NotesBox.Text.Trim();
            string notifyStr = ReminderCheck.IsChecked == true ? "1" : "0";
            string deadlineStr = DeadlinePicker.SelectedDate.HasValue ? $"'{DeadlinePicker.SelectedDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL";

            string userId = _db.GetSingleValue($"SELECT UserId FROM Users WHERE Email = '{_email}'");
            string categoryId = _db.GetSingleValue($"SELECT CategoryId FROM Categories WHERE Name = '{_category}'");

            if (_selectedDocumentId != null)
            {
                string update = $@"
                    UPDATE Documents
                    SET Name = '{name}', ExpiryDate = {deadlineStr}, Notify = {notifyStr}, Notes = '{notes}'
                    WHERE DocumentId = '{_selectedDocumentId}'";

                _db.ExecuteNonQuery(update);
                MessageBox.Show("Document updated.");
            }
            else
            {
                string newId = Guid.NewGuid().ToString();
                string insert = $@"
                    INSERT INTO Documents (DocumentId, Name, ExpiryDate, Notify, Notes, CategoryId)
                    VALUES ('{newId}', '{name}', {deadlineStr}, {notifyStr}, '{notes}', '{categoryId}')";
                string link = $@"
                    INSERT INTO UserDocuments (UserId, DocumentId)
                    VALUES ('{userId}', '{newId}')";

                _db.ExecuteNonQuery(insert);
                _db.ExecuteNonQuery(link);
                MessageBox.Show("Document added.");
            }

            LoadDocuments();
            DeleteButton.Visibility = Visibility.Collapsed;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDocumentId == null)
            {
                MessageBox.Show("Select a document first.");
                return;
            }

            if (MessageBox.Show("Delete this document?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string delLink = $"DELETE FROM UserDocuments WHERE DocumentId = '{_selectedDocumentId}'";
                string delDoc = $"DELETE FROM Documents WHERE DocumentId = '{_selectedDocumentId}'";

                _db.ExecuteNonQuery(delLink);
                _db.ExecuteNonQuery(delDoc);
                MessageBox.Show("Deleted.");

                _selectedDocumentId = null;
                DocumentNameBox.Text = "";
                NotesBox.Text = "";
                DeadlinePicker.SelectedDate = null;
                ReminderCheck.IsChecked = false;
                DeleteButton.Visibility = Visibility.Collapsed;

                LoadDocuments();
            }
        }

        private void AddScan_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDocumentId == null)
            {
                MessageBox.Show("Select a document first.");
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                FileInfo file = new FileInfo(dialog.FileName);
                if (file.Length > 2 * 1024 * 1024)
                {
                    MessageBox.Show("Scan image must be under 2MB.");
                    return;
                }

                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DocumentScans");
                Directory.CreateDirectory(folder);

                string newName = Guid.NewGuid().ToString() + file.Extension;
                string relativePath = Path.Combine("DocumentScans", newName).Replace("\\", "/");
                string fullPath = Path.Combine(folder, newName);

                File.Copy(dialog.FileName, fullPath, true);

                _db.ExecuteNonQuery($"UPDATE documents SET scanpath = '{relativePath}' WHERE documentid = '{_selectedDocumentId}'");
                MessageBox.Show("Scan uploaded.");
            }
        }

        private void ViewScan_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDocumentId == null)
            {
                MessageBox.Show("Select a document first.");
                return;
            }

            string path = _db.GetSingleValue($"SELECT scanpath FROM documents WHERE documentid = '{_selectedDocumentId}'");
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("No scan uploaded for this document.");
                return;
            }

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            if (!File.Exists(fullPath))
            {
                MessageBox.Show("Scan file not found.");
                return;
            }

            Window viewer = new Window
            {
                Title = "View Scan",
                Width = 600,
                Height = 800,
                Content = new Image
                {
                    Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute)),
                    Stretch = Stretch.Uniform
                }
            };
            viewer.ShowDialog();
        }

        private void Placeholder_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.Text == "Document Name")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void Placeholder_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Document Name";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void GoBack_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new CategoryPage(_email, _category));
        }
    }
}