using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj
{
    public class Document
    {
        public string DocumentId { get; private set; }
        public string Name { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Document(string name, DateTime expiryDate)
        {
            DocumentId = Guid.NewGuid().ToString();
            Name = name;
            ExpiryDate = expiryDate;
        }
    }
}
