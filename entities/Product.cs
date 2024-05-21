using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.entities
{
    [Serializable]
    public class Product
    {
        public int Id { get; set; }
        public string model { get; set; }

        public List<Component> components { get; set; }
        public List<int> Ids { get; set; }
        public Product() {
            components = new List<Component>();
            Ids = new List<int>();
        }
        public Product( string model) : this()
        {

            this.model = model;

        }
    }
}
