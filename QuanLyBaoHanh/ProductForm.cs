using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBaoHanh
{
    public partial class ProductForm : Form
    {
        static MongoClient client = new MongoClient();
        static IMongoDatabase db = client.GetDatabase("quanlibaohanh");
        static IMongoCollection<Products> collection = db.GetCollection<Products>("products");

        public ProductForm()
        {
            InitializeComponent();
            ReadAllDocument();
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            button3.Click += Button3_Click;
            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBoxID.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            textBoxName.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            textBoxPrice.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            textBoxPrice.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            textBoxColor.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            textBoxSerialNumber.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
            textBoxImportDate.Text = DateTime.Parse(dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString()).ToString("yyyy-MM-dd");
            textBoxExportDate.Text = DateTime.Parse(dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString()).ToString("yyyy-MM-dd");
            textBoxWarrantyPeriod.Text = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString();
            textBoxSupplier.Text = dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Products product = new Products(
                textBoxName.Text,
                decimal.Parse(textBoxPrice.Text),
                textBoxColor.Text,
                textBoxSerialNumber.Text,
                DateTime.Parse(textBoxImportDate.Text),
                DateTime.Parse(textBoxExportDate.Text),
                int.Parse(textBoxWarrantyPeriod.Text),
                textBoxSupplier.Text
            );
            collection.InsertOne(product);
            ReadAllDocument();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ObjectId id = ObjectId.Parse(textBoxID.Text);
            var updateDef = Builders<Products>.Update
                .Set("name", textBoxName.Text)
                .Set("price", decimal.Parse(textBoxPrice.Text))
                .Set("color", textBoxColor.Text)
                .Set("serial_number", textBoxSerialNumber.Text)
                .Set("import_date", DateTime.Parse(textBoxImportDate.Text))
                .Set("export_date", DateTime.Parse(textBoxExportDate.Text))
                .Set("warranty_period", int.Parse(textBoxWarrantyPeriod.Text))
                .Set("supplier", textBoxSupplier.Text);
            collection.UpdateOne(s => s.Id == id, updateDef);
            ReadAllDocument();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ObjectId id = ObjectId.Parse(textBoxID.Text);
            collection.DeleteOne(s => s.Id == id);
            ReadAllDocument();
        }

        public void ReadAllDocument()
        {
            List<Products> lstProducts = collection.AsQueryable().ToList();
            dataGridView1.DataSource = lstProducts;
        }



    }
}
