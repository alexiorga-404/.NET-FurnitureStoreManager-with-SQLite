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
using System.Data.SQLite;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace WindowsFormsApp1
{
    public partial class MainForm : Form
    {
        #region Attributes

        private const string ConnectionString = "Data Source=database.sqlite";
        private readonly List<entities.Component> _participants = new List<entities.Component>();


        #endregion
        public List<Client> clients { get; set; }
        public List<Product> products { get; set; }
        public MainForm()
        {
            clients = new List<Client>();
            products = new List<Product>();
            InitializeComponent();
            KeyPreview = true;

            printDocument1.PrintPage += printDocument1_PrintPage;



        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.F:
                        //open file menu
                        Point location = new Point(0, menuStrip2.Height);
                        fileToolStripMenuItem.DropDown.Show(location);
                        //MessageBox.Show("Alt+f pressed");
                        break;
                    case Keys.S:
                        save();
                        break;
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
        private void CreateComponent(entities.Component component)
        {
            const string query = "INSERT INTO Component (Id,compName, numberComp) VALUES" +
                        "(@Id,@compName, @numberComp);";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);

                //command.Parameters.AddWithValue("@Id", component.Id);

                command.Parameters.AddWithValue("@Id", component.Id);
                command.Parameters.AddWithValue("@compName", component.compName);
                command.Parameters.AddWithValue("@numberComp", component.numberComp);
                command.ExecuteNonQuery();

                // Add the component to the local list
                //_Components.Add(component);
                //product.components.Add

            }
        }

        private void CreateProduct(Product product)
        {
            const string query = "INSERT INTO Product (model) VALUES (@model); SELECT last_insert_rowid();";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@model", product.model);

                // Execute the query and get the last inserted product ID
                long productId = (long)command.ExecuteScalar();

                // Update the product ID
                product.Id = (int)productId;

                // Add product-component relationships to the junction table
                foreach (var component in product.components)
                {
                    AddProductComponent(productId, component.Id);
                }
            }
        }
        private void AddProductComponent(long productId, long componentId)
        {
            const string query = "INSERT INTO ProductComponent (ProductId, ComponentId) VALUES (@ProductId, @ComponentId);";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@ComponentId", componentId);

                command.ExecuteNonQuery();
            }
        }

        private void UpdateProduct(Product product)
        {
            const string query = "UPDATE Product SET model = @model WHERE Id = @Id;";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@model", product.model);
                command.Parameters.AddWithValue("@Id", product.Id);

                command.ExecuteNonQuery();
            }
        }
        private void DeleteProduct(int productId)
        {
            const string deleteProductQuery = "DELETE FROM Product WHERE Id = @Id;";
            const string getProductComponentsQuery = "SELECT ComponentId FROM ProductComponent WHERE ProductId = @ProductId;";
            const string deleteComponentQuery = "DELETE FROM Component WHERE Id = @ComponentId;";

            //const string deleteProductComponentQuery = "DELETE FROM ProductComponent WHERE ProductId = @ProductId;";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                List<int> componentIds = new List<int>();
                SQLiteCommand getProductComponentsCommand = new SQLiteCommand(getProductComponentsQuery, connection);
                getProductComponentsCommand.Parameters.AddWithValue("@ProductId", productId);
                using (SQLiteDataReader reader = getProductComponentsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        componentIds.Add(reader.GetInt32(0));
                    }
                }

                // Delete all components associated with the product
                foreach (int componentId in componentIds)
                {
                    SQLiteCommand deleteComponentCommand = new SQLiteCommand(deleteComponentQuery, connection);
                    deleteComponentCommand.Parameters.AddWithValue("@ComponentId", componentId);
                    deleteComponentCommand.ExecuteNonQuery();
                }
                // Delete the product from the Product table
                SQLiteCommand deleteProductCommand = new SQLiteCommand(deleteProductQuery, connection);
                deleteProductCommand.Parameters.AddWithValue("@Id", productId);
                deleteProductCommand.ExecuteNonQuery();

                // Delete all product-component relationships associated with the product
               // SQLiteCommand deleteProductComponentCommand = new SQLiteCommand(deleteProductComponentQuery, connection);
               // deleteProductComponentCommand.Parameters.AddWithValue("@ProductId", productId);
                //deleteProductComponentCommand.ExecuteNonQuery();
            }
            DeleteProductComponents(productId);
            
        }

        private void DeleteProductComponents(int productId)
        {
            const string deleteQuery = "DELETE FROM ProductComponent WHERE ProductId = @ProductId;";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);
                command.Parameters.AddWithValue("@ProductId", productId);
                command.ExecuteNonQuery();
            }
        }
        private void UpdateProductComponents(int productId, List<int> componentIds)
        {
            // Delete existing relationships
            DeleteProductComponents(productId);

            // Add new relationships
            foreach (int componentId in componentIds)
            {
                AddProductComponent(productId, componentId);
            }
        }


        private void LoadItems()
        {
            for (int i = 0; i < 100; i++)
            {
                DeleteProduct(i);
                DeleteComponent(i);
            }
            string[] items = new string[] { "masa", "dulap", "noptiera", "comoda" };

            for (int i = 0; i < 4; i++)
            {
                Product product = new Product();
                product.model = items[i];
                entities.Component comp = new entities.Component();
                comp.compName = "picior";
                comp.numberComp = i;
                comp.Id = i+1;
                product.components.Add(comp);
                product.Ids.Add(i + 1);
                CreateComponent(comp);
                CreateProduct(product);

                ListViewItem lvi = new ListViewItem(product.model);
                lvi.SubItems.Add(product.components.Count.ToString());
                lvi.Tag = product;
                lvProducts.Items.Add(lvi);
                products.Add(product);
                
               // AddProducts form = new AddProducts();
               // form._Components
            }
            
            
        }

       
        private void MainForm_Load(object sender, EventArgs e)
        {

            LoadItems();
        }


        private void DisplayClients()
        {
            lvClients.Items.Clear();
            foreach (var item in clients)
            {
                ListViewItem lvi = new ListViewItem(item.name);
                lvi.SubItems.Add(item.products.Count.ToString());
                lvi.Tag = item;
                lvClients.Items.Add(lvi);
            }
        }

        private void lvClients_MouseDoubleClick(object sender, MouseEventArgs e)
        {


            if (lvClients.SelectedItems.Count == 1)
            {
                AddClients form = new AddClients();
                Client client = lvClients.SelectedItems[0].Tag as Client;
                form.client = client;
                form.products = products;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    DisplayClients();
                }
            }
        }

        private void btnAddClient_Click(object sender, EventArgs e)
        {
            AddClients form = new AddClients();
            Client client = new Client();
            form.client = client;
            form.products = products;
            if (form.ShowDialog() == DialogResult.OK)
            {
                client = form.GetUpdatedClient();
                clients.Add(client);
                DisplayClients();
            }


        }


        private void DisplayProducts()
        {
            lvProducts.Items.Clear();
            //sort

            foreach (var product in products)
            {
                ListViewItem lvi = new ListViewItem(product.model);
                lvi.SubItems.Add(product.components.Count.ToString());
                lvi.Tag = product;
                lvProducts.Items.Add(lvi);
            }
        }

        private void btnAddProd_Click(object sender, EventArgs e)
        {
            AddProducts form = new AddProducts();
            Product product = new Product();
            form.product = product;
            if (form.ShowDialog() == DialogResult.OK)
            {
                product = form.GetUpdatedProduct();
                products.Add(product);
                CreateProduct(product);
                DisplayProducts();
            }

        }

        private void lvProducts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddProducts form = new AddProducts();

            if (lvProducts.SelectedItems.Count == 1)
            {
                Product product = lvProducts.SelectedItems[0].Tag as Product;
                form.product = product;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    UpdateProduct(product);
                    UpdateProductComponents(product.Id,product.Ids);
                    DisplayProducts();
                }

            }
        }

        private void textFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    // Serialize clients and their products
                    foreach (var client in clients)
                    {
                        sw.WriteLine($"Client: {client.name}");
                        foreach (var product in client.products)
                        {
                            sw.WriteLine($"\tProduct: {product.model}");
                            foreach (var component in product.components)
                            {
                                sw.WriteLine($"\t\tComponent: {component.compName}, Size: {component.numberComp}");
                            }
                        }
                        sw.WriteLine(); // Separate each client's data
                    }
                    foreach (var product in products)
                    {
                        sw.WriteLine($"\tProduct: {product.model}");
                        foreach (var component in product.components)
                        {
                            sw.WriteLine($"\t\tComponent: {component.compName}, Size: {component.numberComp}");
                        }
                    }
                }
            }
        }

        private void binaryFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = File.Create(sfd.FileName))
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    // Serialize clients and their products
                    bf.Serialize(fs, clients);

                    // Serialize standalone products
                    bf.Serialize(fs, products);
                }
            }
        }

        private void binaryFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = File.OpenRead(ofd.FileName))
                {
                   // deserialize clients and their products
                    BinaryFormatter bf = new BinaryFormatter();
                    clients = (List<Client>)bf.Deserialize(fs);
                    DisplayClients();

                    products = (List<Product>)bf.Deserialize(fs);
                    DisplayProducts();

                }
            }
        }
        

        private void save()
        {
            //something
        }
        private void toolStripbtnSave_Click(object sender, EventArgs e)
        {
            save();
        }

       

        private void btnEdit_Click(object sender, EventArgs e)
        {
            

            if (lvProducts.SelectedItems.Count == 1)
            {
                AddProducts form = new AddProducts();
                Product product = lvProducts.SelectedItems[0].Tag as Product;
                form.product = product;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    UpdateProduct(product);
                    UpdateProductComponents(product.Id, product.Ids);
                    DisplayProducts();
                }

            }

            if (lvClients.SelectedItems.Count == 1)
            {
                AddClients form = new AddClients();
                Client client = lvClients.SelectedItems[0].Tag as Client;
                form.client = client;
                form.products = products;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    DisplayClients();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            

            if (lvProducts.SelectedItems.Count == 1)
            {
               
                Product product = lvProducts.SelectedItems[0].Tag as Product;

                DeleteProduct(product.Id);
                products.Remove(product);
               
                 DisplayProducts();
            }

            if (lvClients.SelectedItems.Count == 1)
            {
                
                Client client = lvClients.SelectedItems[0].Tag as Client;
                clients.Remove(client);
                
                 DisplayClients();
                
            }

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            //  e.Graphics.DrawString("aaaauughhhhtt", this.Font, Brushes.Black, 100, 100);
            
            Font font = new Font("Times New Roman", 13);
            Font titleFont = new Font("Times New Roman", 16);
            SolidBrush brush = new SolidBrush(Color.Black);
            SolidBrush redBrush = new SolidBrush(Color.DarkRed);


            float yPos = 20;
            float leftMargin = e.MarginBounds.Left;

            
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;

            //Clients title
            e.Graphics.DrawString("Clients:", titleFont, brush, e.MarginBounds.Left + (e.MarginBounds.Width / 2), yPos,stringFormat);
            yPos += titleFont.GetHeight() * (float)1.20;

            //print clients
            switch (clients.Count)
            {
                case 0:
                    e.Graphics.DrawString("Empty", titleFont, redBrush, e.MarginBounds.Left + (e.MarginBounds.Width / 2), yPos, stringFormat);
                    yPos += titleFont.GetHeight() * (float)1.20;
                    break;
                default:
                    foreach (var client in clients)
                    {

                        e.Graphics.DrawString($" {client.name}", font, brush, leftMargin, yPos);
                        yPos += font.GetHeight() * (float)1.20;
                        foreach (var product in client.products)
                        {

                            e.Graphics.DrawString($"\tProduct: {product.model}", font, brush, leftMargin, yPos);
                            yPos += font.GetHeight() * (float)1.20;


                            foreach (var component in product.components)
                            {

                                e.Graphics.DrawString($"\t\tComponent: {component.compName}, Size: {component.numberComp}", font, brush, leftMargin, yPos);
                                yPos += font.GetHeight() * (float)1.20;
                            }
                        }


                        yPos += 20;
                    }
                    break;
            }
           

            //Products title
            e.Graphics.DrawString("Products:", titleFont, brush, e.MarginBounds.Left + (e.MarginBounds.Width / 2), yPos, stringFormat);
            yPos += titleFont.GetHeight() * (float)1.20;

            //print products
            switch (products.Count)
            {
                case 0:
                    e.Graphics.DrawString("Empty", titleFont, redBrush, e.MarginBounds.Left + (e.MarginBounds.Width / 2), yPos, stringFormat);
                    yPos += titleFont.GetHeight() * (float)1.20;
                    break;
                default:
                    foreach (var product in products)
                    {

                        e.Graphics.DrawString($" {product.model}", font, brush, leftMargin, yPos);
                        yPos += font.GetHeight() * (float)1.20;


                        foreach (var component in product.components)
                        {

                            e.Graphics.DrawString($"\tComponent: {component.compName}, Size: {component.numberComp}", font, brush, leftMargin, yPos);
                            yPos += font.GetHeight() * (float)1.2;
                        }
                    }
                    break;
            }
            
        }


        private void printPreviewDialog1_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;

            printPreviewDialog1.ShowDialog();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDocument1.Print();
        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
