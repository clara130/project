using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj
{
    public class Category
    {
        public string CategoryId { get; private set; }
        public string Name { get; set; }
        public List<Document> Documents { get; private set; }
        public Category(string name)
        {
            CategoryId = Guid.NewGuid().ToString();
            Name = name;
            Documents = new List<Document>();
        }
        public void AddDocument(Document document)
        {
            Documents.Add(document);
        }
    }
}
