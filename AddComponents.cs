using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.entities;

namespace WindowsFormsApp1
{
    public partial class AddComponents : Form
    {
        private  string ConnectionString = "Data Source=database.sqlite";

        public entities.Component component { get; set; }
        
        public AddComponents()
        {
            InitializeComponent();
        }

        

        


        private void AddFurniture_Load(object sender, EventArgs e)
        {
            if (component != null)
            {
                textBox1.Text = component.compName;
                numericUpDown1.Value = component.numberComp;
               // nudId.Value = component.Id;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            
            if (component != null)
            {
                if (textBox1.Text.Length < 3)
                {
                    MessageBox.Show("Enter a valid name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;


                }
                component.compName = textBox1.Text;
                component.numberComp = (int)numericUpDown1.Value;
               // component.Id = (int)nudId.Value;
               // CreateComponent(component);
            }
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (textBox1.Text.Length < 3)
            {
                e.Cancel = true;
                errorComponentName.SetError((Control)sender, "Invalid Name");

            }
        }

        private void numericUpDown1_Validating(object sender, CancelEventArgs e)
        {
            if (numericUpDown1.Value < 1)
            {
                e.Cancel = true;
                errorComponentName.SetError((Control)sender, "Invalid Number");
            }
        }



        private void textBox1_Validated(object sender, EventArgs e)
        {
            errorComponentName.SetError((Control)sender, null);
        }

        private void numericUpDown1_Validated(object sender, EventArgs e)
        {
            errorComponentNr.SetError((Control)sender, null);
        }

        public entities.Component GetUpdatedComponent()
        {
            return component;
        }
    }
}