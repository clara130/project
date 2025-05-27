using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj
{
    public class User
    {
        public string UserId { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string ProfilePicture { get; set; }
        public List<Document> Documents { get; private set; }
        public List<Category> Categories { get; private set; }
        public User(string email, string password)
        {
            UserId = Guid.NewGuid().ToString();
            Email = email;
            Password = password;
            Documents = new List<Document>();
            Categories = new List<Category>();
        }
        public void ChangePassword(string newPassword)
        {
            Password = newPassword;
        }


    }
}

