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
    public partial class WarrantyForm : Form
    {
        static MongoClient client = new MongoClient();
        static IMongoDatabase db = client.GetDatabase("quanlibaohanh");
        static IMongoCollection<Warranty> Warrantycollection = db.GetCollection<Warranty>("warranty");
        static IMongoCollection<Products> ProductCollection = db.GetCollection<Products>("products");
        static IMongoCollection<Customers> CustomerCollection = db.GetCollection<Customers>("customers");
        public WarrantyForm()
        {
            InitializeComponent();
            this.Load += WarrantyForm_Load;
            this.dataGridView1.CellClick += DataGridView1_CellClick;
            this.dataGridView2.CellClick += DataGridView2_CellClick;
            this.dataGridView3.CellClick += DataGridView3_CellClick;
            this.textBoxTimKiemProduct.TextChanged += TextBoxTimKiemProduct_TextChanged;
            this.textBoxTimKiemCus.TextChanged += TextBoxTimKiemCus_TextChanged;
            this.buttonTao.Click += ButtonTao_Click;
        }

        private void ButtonTao_Click(object sender, EventArgs e)
        {
            string proId = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            string cusId = dataGridView3.CurrentRow.Cells[0].Value.ToString();
            DateTime currentDate = DateTime.Now;
            int period = int.Parse(dataGridView2.CurrentRow.Cells[7].Value.ToString());
            DateTime expDate = currentDate.AddMonths(period);
            Warranty w = new Warranty(ObjectId.Parse(cusId), ObjectId.Parse(proId), currentDate,expDate,1);
            Warrantycollection.InsertOne(w);
            ReadAllDocuments();
        }

        private void TextBoxTimKiemCus_TextChanged(object sender, EventArgs e)
        {
            string cus = textBoxTimKiemCus.Text;
            List<Customers> listCustomers = CustomerCollection.AsQueryable().Where(p => p.Name.Contains(cus)).ToList<Customers>();
            dataGridView3.DataSource = listCustomers;
        }

        private void TextBoxTimKiemProduct_TextChanged(object sender, EventArgs e)
        {
            string pro = textBoxTimKiemProduct.Text;
            List<Products> listPro = ProductCollection.AsQueryable().Where(p => p.Name.Contains(pro)).ToList();
            dataGridView2.DataSource = listPro;
        }

        private void DataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBoxCusNew.Text = dataGridView3.Rows[e.RowIndex].Cells[1].Value.ToString();
        }

        private void DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBoxProNew.Text = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count)
            {
                return; 
            }

            textBoxCustomer.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            textBoxProduct.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

            DateTime activationDate;
            if (DateTime.TryParse(dataGridView1.Rows[e.RowIndex].Cells[2].Value?.ToString(), out activationDate))
            {
                dateTimePickerACT.Value = activationDate;
            }

            DateTime expDate;
            if (DateTime.TryParse(dataGridView1.Rows[e.RowIndex].Cells[3].Value?.ToString(), out expDate))
            {
                dateTimePickerEXP.Value = expDate;
            }
            textBoxStatus.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
        }


        private void WarrantyForm_Load(object sender, EventArgs e)
        {
            ReadAllDocuments();
        }

        public void ReadAllDocuments()
        {
            loadWarranty();
            List<Customers> listCustomers = CustomerCollection.AsQueryable().ToList<Customers>();
            dataGridView3.DataSource = listCustomers;
            List<Products> listPro = ProductCollection.AsQueryable().ToList<Products>();
            dataGridView2.DataSource = listPro;
        }
        public string getCusName(ObjectId id)
        {
            // Find the customer by ID
            var customer = CustomerCollection.Find(p => p.Id == id).FirstOrDefault();

            // Check if customer is null and return a default value or handle accordingly
            return customer?.Name ?? "Unknown Customer"; // Provide default value if null
        }
        public string getProName(ObjectId id)
        {
            return ProductCollection.Find(p=>p.Id == id).FirstOrDefault().Name;
        }
        public string TranferStatus(int n)
        {
            return n == 0 ? "Hết bảo hành" : "Còn bảo hành";
        }
        void loadWarranty()
        {
            List<Warranty> listWarranty = Warrantycollection.AsQueryable().ToList<Warranty>();
            var list = listWarranty.Select(w => new
            {
                Customer_Name = getCusName(w.customer_id),
                Product_Name = getProName(w.product_id),
                ActivationDate = w.activation_date,
                ExpirationDate = w.expiry_date,
                Status = TranferStatus(w.status)
            }).ToList();
            dataGridView1.DataSource = list;
        }
    }
}
