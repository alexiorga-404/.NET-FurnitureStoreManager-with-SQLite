using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.entities
{
    [Serializable]
    public class Client
    {
        public string name { get; set; }

        public List<Product> products;

       

        public List<string> productNames;
        public Client() {
            products = new List<Product>();
            productNames = new List<string>();
        }

        public Client(string name) : this()
        {
            this.name = name;
          

        }
    }
}
