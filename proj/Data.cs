using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace proj
{
    public class Data
    {
        private string connectionString =
            "datasource=127.0.0.1;" +
            "port=3307;" +
            "username=root;" +
            "password=;" +
            "database=lastcall;"; // Change to your actual database name
        private int Insert(string query)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand commandDatabase = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();
                    int result = commandDatabase.ExecuteNonQuery();
                    return (int)commandDatabase.LastInsertedId;
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
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
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

        public void CreateTables()
        {
            string createUsers = @"
    CREATE TABLE IF NOT EXISTS Users (
        UserId VARCHAR(36) PRIMARY KEY,
        Email VARCHAR(255) NOT NULL UNIQUE,
        Password VARCHAR(255) NOT NULL,
        ProfilePicture TEXT
    );";

            string createCategories = @"
    CREATE TABLE IF NOT EXISTS Categories (
        CategoryId VARCHAR(36) PRIMARY KEY,
        Name VARCHAR(255) NOT NULL
    );";

            string createDocuments = @"
    CREATE TABLE IF NOT EXISTS Documents (
        DocumentId VARCHAR(36) PRIMARY KEY,
        Name VARCHAR(255) NOT NULL,
        ExpiryDate DATETIME,
        CategoryId VARCHAR(36),
        FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
    );";

            string createUserDocuments = @"
    CREATE TABLE IF NOT EXISTS UserDocuments (
        UserId VARCHAR(36),
        DocumentId VARCHAR(36),
        PRIMARY KEY (UserId, DocumentId),
        FOREIGN KEY (UserId) REFERENCES Users(UserId),
        FOREIGN KEY (DocumentId) REFERENCES Documents(DocumentId)
    );";

            string createUserCategories = @"
    CREATE TABLE IF NOT EXISTS UserCategories (
        UserId VARCHAR(36),
        CategoryId VARCHAR(36),
        PRIMARY KEY (UserId, CategoryId),
        FOREIGN KEY (UserId) REFERENCES Users(UserId),
        FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
    );";

            ExecuteNonQuery(createUsers);
            ExecuteNonQuery(createCategories);
            ExecuteNonQuery(createDocuments);
            ExecuteNonQuery(createUserDocuments);
            ExecuteNonQuery(createUserCategories);
        }

    }
}