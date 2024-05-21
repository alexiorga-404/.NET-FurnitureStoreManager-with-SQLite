using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.entities;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    public partial class AddProducts : Form
    {
        public Product product { get; set; }
        // public List<entities.Component> Components { get; set; }

        #region Attributes

        private const string ConnectionString = "Data Source=database.sqlite";
        public readonly List<entities.Component> _Components = new List<entities.Component>();


        #endregion

        public AddProducts()
        {
            
            
            product = new Product();
            InitializeComponent();
        }

        private void DisplayComponents()
        {
            listView1.Items.Clear();
            //sort
            foreach (var component in product.components)
            {
                if (product.Ids.Any(id => id == component.Id))
                {
                    ListViewItem lvi = new ListViewItem(component.compName);
                    lvi.SubItems.Add(component.numberComp.ToString());
                    lvi.Tag = component;
                    listView1.Items.Add(lvi);
                }
            }
            
        }

        private void UpdateComponent(entities.Component component)
        {
            const string query = "UPDATE Component SET compName=@compName, " +
                        "numberComp=@numberComp WHERE Id=@Id";
            

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);


                command.Parameters.AddWithValue("@Id",component.Id);
                command.Parameters.AddWithValue("@compName", component.compName);
                command.Parameters.AddWithValue("@numberComp", component.numberComp);

                command.ExecuteNonQuery();

            }
        }

        private void btnAddComp_Click(object sender, EventArgs e)
        {

            AddComponents form = new AddComponents();
            entities.Component component = new entities.Component();
            form.component = component;
            if (form.ShowDialog() == DialogResult.OK)
            {
                component = form.GetUpdatedComponent();
                CreateComponent(component);
                product.components.Add(component);
               // product.Ids.Add(component.Id);
                
                DisplayComponents();
                //AddComponent(component);

            }
        }

        private void CreateComponent(entities.Component component)
        {
            const string query = "INSERT INTO Component (compName, numberComp) VALUES" +
                        "(@compName, @numberComp);";
            const string getLastIdQuery = "SELECT last_insert_rowid();";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);
               
                //command.Parameters.AddWithValue("@Id", component.Id);

                command.Parameters.AddWithValue("@compName", component.compName);
                command.Parameters.AddWithValue("@numberComp", component.numberComp);
                
                command.ExecuteNonQuery();

                // Add the component to the local list
                //_Components.Add(component);
                // product.components.Add(component);
                SQLiteCommand getLastIdCommand = new SQLiteCommand(getLastIdQuery, connection);
                long lastInsertedId = (long)getLastIdCommand.ExecuteScalar();
                component.Id = (int)lastInsertedId; // Assign the generated ID to the component

                // Add the component ID to the product's list of IDs
                product.Ids.Add(component.Id);

            }
        }


        private void ReadComponents()
        {
            product.components.Clear();
            string query = "SELECT * FROM Component";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        long id = (long)reader["Id"];
                        string compName = (string)reader["compName"];
                        long numberComp = (long)reader["numberComp"];

                        entities.Component component =
                            new entities.Component(compName, (int)numberComp,(int)id);
                        if (product.Ids.Any(ID => ID == component.Id))
                            product.components.Add(component);
                    }
                }

            }
        }

        private void DeleteComponent(int id)
        {
            string query = "DELETE FROM Component WHERE Id=@Id;";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);

                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();

            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {

            /*if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Choose a component");
                return;
            }
            if (MessageBox.Show("Are you sure?", "Delete participant",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    ListViewItem selectedItem = listView1.SelectedItems[0];
                    entities.Component component = (entities.Component)selectedItem.Tag;
                    
                    
                    DeleteComponent(component.Id);
                    product.components.Remove(component);

                    DisplayComponents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                 if (listView1.SelectedItems.Count == 1)
                 {
                     entities.Component component = listView1.SelectedItems[0].Tag as entities.Component;
                   
                    DeleteComponent(component.Id);
                    product.components.Remove(component);

                    DisplayComponents();


                }
            }*/

            if (listView1.SelectedItems.Count > 0)
            {
                entities.Component component = listView1.SelectedItems[0].Tag as entities.Component;
                DeleteComponent(component.Id);
                
                ReadComponents();
                product.components.Remove(component);
                DisplayComponents();
            }
        }

        private void AddProducts_Load(object sender, EventArgs e)
        {
            if (product != null)
            {
                textBox1.Text = product.model;
                ReadComponents();
                DisplayComponents();
                
            }

            /*try
            {
                if (!CreateComponentTable())
                {
                    MessageBox.Show("Failed to create Component table!");
                    return;
                }

                LoadComponents();
                DisplayComponents();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }*/
        }


       


       /* private void LoadComponents()
        {
            const string query = "SELECT * FROM Component";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var command = new SQLiteCommand(query, connection);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string compName = (string)reader["ComponentName"];
                        int compNumber = Convert.ToInt32(reader["ComponentNumber"]);

                        entities.Component component = new entities.Component
                        {
                            compName = compName,
                            numberComp = compNumber
                        };
                        _Components.Add(component);
                    }
                }
            }
        }
       */

       /* private void DeleteComponent(entities.Component Component)
        {
            const string query = "DELETE FROM Component WHERE ComponentName=@componentName";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                //Remove from the database
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@componentName", Component.compName);

                command.ExecuteNonQuery();

                //Remove from the local copy
                _Components.Remove(Component);
            }
        }*/

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (product != null)
            {
                if (textBox1.Text.Length < 3)
                {
                    MessageBox.Show("Enter a valid name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;

                }
                
                 product.model = textBox1.Text;
                
                // product.components=
            }
        }



        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddComponents form = new AddComponents();

            if (listView1.SelectedItems.Count == 1)
            {
                entities.Component component = listView1.SelectedItems[0].Tag as entities.Component;
                form.component = component;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    UpdateComponent(component);
                    ReadComponents();
                    DisplayComponents();
                }

            }
        }
        

       private void btnEdit_Click(object sender, EventArgs e)
        {
            AddComponents form = new AddComponents();

            if (listView1.SelectedItems.Count == 1)
            {
                entities.Component component = listView1.SelectedItems[0].Tag as entities.Component;
                form.component = component;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    UpdateComponent(component);
                    DisplayComponents();
                }

            }
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (textBox1.Text.Length < 3)
            {
                e.Cancel = true;
                errorProdName.SetError((Control)sender, "Invalid Name");
            }
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            errorProdName.SetError((Control)sender, null);
        }

        private void serializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = File.Create(sfd.FileName))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, product);
                }
            }
        }
        private void deserializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = File.OpenRead(ofd.FileName))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    product = (Product)bf.Deserialize(fs);
                    textBox1.Text = product.model;
                    DisplayComponents();
                }
            }
        }

        public Product GetUpdatedProduct()
        {
            return product;
        }

        private void customControl12_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textBox1.DoDragDrop(textBox1.Text, DragDropEffects.Copy);
        }


        //DRAG AND DROP
        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            
           
            if (e.Data.GetDataPresent(DataFormats.Text, true))
            {
               
                if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move 
                    && (Control.ModifierKeys & Keys.Control )!=Keys.Control)
                {
                  
                    e.Effect = DragDropEffects.Move;
                }
                else
                {
                   
                    e.Effect = DragDropEffects.Copy;
                }
            }

        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text, true))
            {
                listView1.Items.Add((string)e.Data.GetData(DataFormats.Text));
            }
        }
    }
}

