using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.entities;

namespace WindowsFormsApp1
{
    public partial class AddClients : Form
    {
       
        public Client client { get; set; }
        public List<Product> products { get; set; }
        public AddClients()
        {
            products = new List<Product>();
            InitializeComponent();
           
        }


        private void AddEditForm_Load(object sender, EventArgs e)
        {

             checkedListBox1.Items.Clear();
            

            if (client != null)
            {
                tbName.Text = client.name;
                foreach (var product in products)
                {
                    string name = product.model;
                    checkedListBox1.Items.Add(name);

                    if (client.products.Any(p => p.model == product.model))
                    {
                        checkedListBox1.SetItemChecked(checkedListBox1.Items.IndexOf(name), true);
                    }
                }

            }
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (client != null)
                {
                    client.products.Clear();
                    if (tbName.Text.Length < 3)
                    {
                        throw new InvalidClientNameException(tbName.Text);
                        
                    }
                    
                    client.name = tbName.Text;

                    if (checkedListBox1.CheckedItems.Count < 1)
                    {

                        throw new Exception("Please select at least one product."); // Standard exception
                        
                    }

                    foreach (var checkedItem in checkedListBox1.CheckedItems)
                    {
                        string model = checkedItem.ToString();
                        // Find the corresponding product from the original list
                        Product product = products.FirstOrDefault(p => p.model == model);

                        if (product != null)
                        {
                            client.products.Add(product);
                        }
                    }

                }
            }
            catch (InvalidClientNameException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;//so the invalid data doesn t get added to the listView
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;


            }

        }


        public class InvalidClientNameException : Exception
        {
            //custom exception
            public InvalidClientNameException(string name) : base(String.Format("Invalid Name: {0}",name)) { }

        }
        private void tbName_Validating(object sender, CancelEventArgs e)
        {
           /* try
            {
                if (tbName.Text.Length < 3)
                {
                    e.Cancel = true;
                    //errorCustName.SetError((Control)sender, "Name should have at least 3 chracters");
                    throw new InvalidClientNameException("Client name is invalid.");
                }
                else
                {
                   // errorProducts.SetError((Control)sender, null);
                }
            }
            catch (InvalidClientNameException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
            
        }

        private void checkedListBox1_Validating(object sender, CancelEventArgs e)
        {
            /*
            try
            {
                if (checkedListBox1.CheckedItems.Count < 1)
                {
                    e.Cancel = true;
                    // errorProducts.SetError((Control)sender, "Choose  Item!");
                    throw new Exception("Please select at least one product.");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void checkedListBox1_Validated(object sender, EventArgs e)
        {
            //errorProducts.SetError((Control)sender, null);
            
        }
        private void tbName_Validated(object sender, EventArgs e)
        {
            // errorCustName.SetError((Control)sender, null);
        }

        public Client GetUpdatedClient()
        {
            return client;
        }
    }
}
