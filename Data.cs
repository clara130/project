using System;
using System.Collections.Generic;
using MySqlConnector;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace proj
{
    public class Data
    {
        private string connectionString =
            "datasource=127.0.0.1;" +
            "port=3307;" +
            "username=root;" +
            "password=;" +
            "database=lastcall;";

        public int Insert(string query)
        {
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    return (int)command.LastInsertedId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
                }
            }
        }

        public bool ExecuteNonQuery(string query)
        {
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                    return false;
                }
            }
        }

        public string GetPasswordByEmail(string email)
        {
            const string query = "SELECT Password FROM Users WHERE Email = @Email";
            using (var connection = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                try
                {
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.Read() ? reader.GetString(0) : null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                    return null;
                }
            }
        }

        public List<string> SelectEmails(string query)
        {
            var emails = new List<string>();
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            emails.Add(reader.GetString(0));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                }
            }
            return emails;
        }

        public (bool isValid, bool isAdmin) VerifyUser(string email, string password)
        {
            const string query = "SELECT IsAdmin FROM Users WHERE Email = @Email AND Password = @Password";
            using (var connection = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                try
                {
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (true, reader.GetBoolean(0));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                }
                return (false, false);
            }
        }

        public string GetProfilePicturePath(string email)
        {
            const string query = "SELECT profilepicturepath FROM users WHERE email = @Email";
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result?.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                    return null;
                }
            }
        }


        public List<(string name, DateTime date)> GetUpcomingDeadlines(string email)
        {
            var list = new List<(string name, DateTime date)>();
            const string query = @"
                SELECT d.Name, d.ExpiryDate
                FROM Documents d
                JOIN UserDocuments ud ON d.DocumentId = ud.DocumentId
                JOIN Users u ON ud.UserId = u.UserId
                WHERE u.Email = @Email AND d.ExpiryDate IS NOT NULL
                ORDER BY d.ExpiryDate ASC
                LIMIT 5";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add((reader.GetString(0), reader.GetDateTime(1)));
                    }
                }
            }

            return list;
        }

        public List<(string title, DateTime? expiryDate, bool notify)> GetDocumentsByCategory(string email, string category)
        {
            var list = new List<(string, DateTime?, bool)>();
            const string query = @"
                SELECT d.Name, d.ExpiryDate, d.Notify
                FROM Documents d
                JOIN UserDocuments ud ON d.DocumentId = ud.DocumentId
                JOIN Users u ON ud.UserId = u.UserId
                JOIN Categories c ON d.CategoryId = c.CategoryId
                WHERE u.Email = @Email AND c.Name = @Category";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Category", category);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string title = reader.GetString(0);
                        DateTime? expiry = reader.IsDBNull(1) ? null : reader.GetDateTime(1);
                        bool notify = !reader.IsDBNull(2) && reader.GetBoolean(2);
                        list.Add((title, expiry, notify));
                    }
                }
            }

            return list;
        }

        public string GetSingleValue(string query)
        {
            using (var connection = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    var result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                    return null;
                }
            }
        }



        public bool SendTemporaryPassword(string recipientEmail, string tempPassword)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("lastcallllllllll@outlook.com");
                mail.To.Add(recipientEmail);
                mail.Subject = "Your Temporary Password";
                mail.Body = $"Your temporary password is: {tempPassword}";

                SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com", 587);
                smtp.Credentials = new NetworkCredential("lastcallllllllll@outlook.com", "jrncigjplkfivpec"); // app password
                smtp.EnableSsl = true;
                smtp.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message);
                return false;
            }
        }

        public List<(string name, DateTime expiryDate)> GetNotifiableDocuments(string email)
        {
            var list = new List<(string name, DateTime expiryDate)>();
            string query = @"
        SELECT d.Name, d.ExpiryDate
        FROM Documents d
        JOIN UserDocuments ud ON d.DocumentId = ud.DocumentId
        JOIN Users u ON ud.UserId = u.UserId
        WHERE u.Email = @Email AND d.ExpiryDate IS NOT NULL AND d.Notify = 1
        AND DATEDIFF(d.ExpiryDate, CURDATE()) <= 7";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        DateTime date = reader.GetDateTime(1);
                        list.Add((name, date));
                    }
                }
            }

            return list;
        }

        public List<string> SelectRow(string query)
        {
            var row = new List<string>();

            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.IsDBNull(i) ? null : reader.GetString(i));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                }
            }

            return row;
        }

        public List<KeyValuePair<string, string>> SelectPairs(string query)
        {
            var list = new List<KeyValuePair<string, string>>();

            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string key = reader.GetString(0);
                            string value = reader.GetString(1);
                            list.Add(new KeyValuePair<string, string>(key, value));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                }
            }

            return list;
        }







    }
}
