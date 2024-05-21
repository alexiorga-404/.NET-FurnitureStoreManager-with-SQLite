using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.entities
{
    [Serializable]
    public class Component
    {
        public int Id { get; set; }
        public string compName { get; set; }
        public int numberComp { get; set; }

        public Component() { }


        public Component(string comp, int size) : this()
        {
            this.compName = comp;
            this.numberComp = size;
        }

        public Component(string comp, int size,int Id) : this(comp,size)
        {
           
            this.Id = Id;
        }

    }
}

