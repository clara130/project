using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj
{
    public class Admin
    {
        public string AdminId { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public Admin(string username, string password)
        {
            AdminId = Guid.NewGuid().ToString();
            Username = username;
            Password = password;
        }
        public void ViewLogs()
        {
            // Logic to view logs
        }
        public void ViewStats()
        {
            // Logic to view statistics
        }
    }
}
