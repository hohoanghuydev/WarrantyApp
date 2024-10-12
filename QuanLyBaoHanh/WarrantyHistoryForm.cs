using Microsoft.SqlServer.Server;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBaoHanh
{
    public partial class WarrantyHistoryForm : Form
    {
        static MongoClient client = new MongoClient();
        static IMongoDatabase db = client.GetDatabase("quanlibaohanh");
        static IMongoCollection<Warranty> Warrantycollection = db.GetCollection<Warranty>("warranty");
        static IMongoCollection<WarrantyHistory> WarrantyHistoryCollection = db.GetCollection<WarrantyHistory>("warrantyhistory");
        static IMongoCollection<Products> ProductCollection = db.GetCollection<Products>("products");
        static IMongoCollection<Customers> CustomerCollection = db.GetCollection<Customers>("customers");
        static IMongoCollection<Employee> employeeCollection = db.GetCollection<Employee>("employees");
        List<RepairDetail> repairDetails = new List<RepairDetail>();
        CultureInfo culture = new CultureInfo("vi-VN");

        public WarrantyHistoryForm()
        {
            InitializeComponent();
            this.Load += WarrantyHistoryForm_Load;
            this.dataGridViewHistory.CellClick += DataGridViewHistory_CellClick;
            this.btnFilter.Click += BtnFilter_Click;
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            //1. customer = 0, product = 0 => load het | 2. customer = 1, product = 0 => load theo customer | 3. customer = 1, product = 1 => load theo customer va product | 4. customer = 0, product = 1 => load theo product
            if (comboBoxCustomers.SelectedIndex <= 0 && comboBoxProducts.SelectedIndex <= 0)
            {
                LoadWarranty();
            }
            else if (comboBoxProducts.SelectedIndex <= 0 && comboBoxCustomers.SelectedIndex > 0)
            {
                LoadWarranty(comboBoxCustomers.SelectedItem as Customers);
            }
            else if (comboBoxCustomers.SelectedIndex <= 0 && comboBoxProducts.SelectedIndex > 0)
            {
                LoadWarranty(comboBoxProducts.SelectedItem as Products);
            }
            else
            {
                var productSelected = comboBoxProducts.SelectedItem as Products;
                var customerSeleted = comboBoxCustomers.SelectedItem as Customers;
                LoadWarranty(customerSeleted.Id, productSelected.Id);
            }
        }

        private void LoadComboBoxProducts()
        {
            var products = ProductCollection.AsQueryable().ToList<Products>();

            products.Insert(0, new Products("Lọc theo sản phẩm..."));
            comboBoxProducts.DataSource = products;
            comboBoxProducts.DisplayMember = "Name";
        }

        private Employee GetEmployee(ObjectId employeeId)
        {
            if (employeeId == null) return null;

            return employeeCollection.Find(emp => emp.Id == employeeId).FirstOrDefault();
        }

        private void DataGridViewHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewHistory.Rows.Count)
            {
                var cellValue = dataGridViewHistory.Rows[e.RowIndex].Cells[0].Value;
                if (cellValue != null)
                {
                    ObjectId warrantyHistoryId = ObjectId.Parse(cellValue.ToString());
                    WarrantyHistory warrantyHistory = WarrantyHistoryCollection.Find(w => w.Id == warrantyHistoryId).FirstOrDefault();

                    loadRepairParts(warrantyHistoryId);
                    textBoxId.Text = warrantyHistory.WarrantyId.ToString();
                    textBoxDes.Text = warrantyHistory.Description;
                    comboBoxEmployee.Text = GetEmployee(warrantyHistory.RepairStaff).Name;
                    if (warrantyHistory.WarrantyType == 0)
                    {
                        comboBoxType.SelectedIndex = 0;
                    }
                    else
                    {
                        comboBoxType.SelectedIndex = 1;
                    }
                }
            }
        }

        private void WarrantyHistoryForm_Load(object sender, EventArgs e)
        {
            loadWarrantyHistory();
            LoadWarranty();
            LoadComboBoxCustomers();
            LoadComboBoxProducts();
            LoadComboBoxEmployees();
            LoadWarrantyType();
        }

        private void LoadWarrantyType()
        {
            List<WarrantyType> warrantyTypes = new List<WarrantyType>
            {
                new WarrantyType(0, "Bảo hành không hoàn toàn"),
                new WarrantyType(1, "Bảo hành hoàn toàn")
            };

            comboBoxType.DataSource = warrantyTypes;
            comboBoxEmployee.DisplayMember = "NameType";
        }

        private void LoadComboBoxEmployees()
        {
            var employees = employeeCollection.AsQueryable().ToList<Employee>();

            comboBoxEmployee.DataSource = employees;
            comboBoxEmployee.DisplayMember = "Name";
        }

        private void LoadComboBoxCustomers()
        {
            var customers = CustomerCollection.AsQueryable().ToList<Customers>();

            customers.Insert(0, new Customers("Lọc theo khách hàng..."));
            comboBoxCustomers.DataSource = customers;
            comboBoxCustomers.DisplayMember = "Name";
        }

        private void loadWarrantyHistory()
        {
            List<WarrantyHistory> listWarrantyHistory = WarrantyHistoryCollection.AsQueryable().ToList<WarrantyHistory>();
            var listFormat = listWarrantyHistory.Select(warrantyHistory => new
            {
                ID = warrantyHistory.Id,
                WarrantyId = warrantyHistory.WarrantyId,
                Description = warrantyHistory.Description,
                ReceptionDate = warrantyHistory.ReceptionDate.ToString("dd/MM/yyyy hh:mm", culture),
                ReturnDate = warrantyHistory.ReturnDate?.ToString("dd/MM/yyyy hh:mm", culture),
                RepairStaff = GetEmployee(warrantyHistory.RepairStaff).Name,
                WarrantyType = TransferWarrantyType(warrantyHistory.WarrantyType),
                RepairStatus = TransferStatus(warrantyHistory.RepairStatus),
                Total = string.Format("{0:N0}đ", warrantyHistory.Total)
            }).ToList();
            dataGridViewHistory.DataSource = listFormat;
        }

        private string TransferStatus(int status)
        {
            return status == 0 ? "Đã hoàn tất" : "Đang bảo hành";
        }

        private string TransferWarrantyType(int type)
        {
            return type == 0 ? "Bảo hành không hoàn toàn" : "Bảo hành hoàn toàn";
        }

        public List<RepairDetail> GetRepairDetailsByWarrantyId(ObjectId warrantyId)
        {
            var warrantyHistory = WarrantyHistoryCollection.Find(p => p.Id == warrantyId).FirstOrDefault();
            if (warrantyHistory == null)
            {
                return new List<RepairDetail>();
            }

            return warrantyHistory.RepairDetails ?? new List<RepairDetail>();
        }

        private void loadRepairParts(ObjectId warrantyId)
        {
            repairDetails.Clear();
            repairDetails = GetRepairDetailsByWarrantyId(warrantyId);

            dataGridViewDetail.DataSource = repairDetails;
        }

        public string GetCusName(ObjectId id)
        {
            var customer = CustomerCollection.Find(p => p.Id == id).FirstOrDefault();

            return customer?.Name ?? "Unknown Customer";
        }

        public string GetProName(ObjectId id)
        {
            return ProductCollection.Find(p => p.Id == id).FirstOrDefault().Name;
        }

        public string TransferWarrantyStatus(int n)
        {
            return n == 0 ? "Hết bảo hành" : "Còn bảo hành";
        }

        private void LoadWarranty(ObjectId customerId, ObjectId productId)
        {
            List<Warranty> listWarranty = Warrantycollection.Find(warranty => warranty.customer_id == customerId && warranty.product_id == productId).ToList<Warranty>();
            var list = listWarranty.Select(w => new
            {
                ID = w.Id,
                Customer_Name = GetCusName(w.customer_id),
                Product_Name = GetProName(w.product_id),
                ActivationDate = w.activation_date,
                ExpirationDate = w.expiry_date,
                Status = TransferWarrantyStatus(w.status)
            }).ToList();
            dataGridViewWarranty.DataSource = list;
        }

        private void LoadWarranty(Products product)
        {
            List<Warranty> listWarranty = Warrantycollection.Find(warranty => warranty.product_id == product.Id).ToList<Warranty>();
            var list = listWarranty.Select(w => new
            {
                ID = w.Id,
                Customer_Name = GetCusName(w.customer_id),
                Product_Name = GetProName(w.product_id),
                ActivationDate = w.activation_date,
                ExpirationDate = w.expiry_date,
                Status = TransferWarrantyStatus(w.status)
            }).ToList();
            dataGridViewWarranty.DataSource = list;
        }

        private void LoadWarranty(Customers customer)
        {
            List<Warranty> listWarranty = Warrantycollection.Find(warranty => warranty.customer_id == customer.Id).ToList<Warranty>();
            var list = listWarranty.Select(w => new
            {
                ID = w.Id,
                Customer_Name = GetCusName(w.customer_id),
                Product_Name = GetProName(w.product_id),
                ActivationDate = w.activation_date,
                ExpirationDate = w.expiry_date,
                Status = TransferWarrantyStatus(w.status)
            }).ToList();
            dataGridViewWarranty.DataSource = list;
        }

        private void LoadWarranty()
        {
            List<Warranty> listWarranty = Warrantycollection.AsQueryable().ToList<Warranty>();
            var list = listWarranty.Select(w => new
            {
                ID = w.Id,
                Customer_Name = GetCusName(w.customer_id),
                Product_Name = GetProName(w.product_id),
                ActivationDate = w.activation_date,
                ExpirationDate = w.expiry_date,
                Status = TransferWarrantyStatus(w.status)
            }).ToList();
            dataGridViewWarranty.DataSource = list;
        }

        private void buttonThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRepairPart.Text) || string.IsNullOrWhiteSpace(txtPrice.Text) || dataGridViewHistory.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng nhập thông tin thành phần sửa chữa!");
                return;
            }

            string repairPart = txtRepairPart.Text;
            int price = int.Parse(txtPrice.Text);

            RepairDetail detail = new RepairDetail(repairPart, price);
            repairDetails.Add(detail);
            UpdateRepairDetail();
        }

        private void UpdateRepairDetail()
        {
            ObjectId warrantyHistoryId = ObjectId.Parse(dataGridViewHistory.CurrentRow.Cells[0].Value.ToString());
            var warrantyHistory = WarrantyHistoryCollection.Find(w => w.Id == warrantyHistoryId).FirstOrDefault();
            int repairStatus = warrantyHistory.RepairStatus;

            if (repairStatus == 0)
            {
                MessageBox.Show("Lịch sử bảo hành này đã hoàn thành");
                return;
            }
            
            var rowWarrantyHistory = Builders<WarrantyHistory>.Update
                .Set("warranty_id", warrantyHistory.WarrantyId)
                .Set("description", warrantyHistory.Description)
                .Set("reception_date", warrantyHistory.ReceptionDate)
                .Set("repair_details", repairDetails)
                .Set("repair_status", repairStatus)
                .Set("return_date", (DateTime?)null)
                .Set("repair_staff", warrantyHistory.RepairStaff)
                .Set("warranty_type", warrantyHistory.WarrantyType)
                .Set("total", repairDetails.Sum(part => part.Price));

            WarrantyHistoryCollection.UpdateOne(w => w.Id == warrantyHistoryId, rowWarrantyHistory);
            loadRepairParts(warrantyHistoryId);
            loadWarrantyHistory();
        }

        private bool CheckWarrantyStatus(ObjectId warrantyId)
        {
            var warrantyDocument = Warrantycollection.Find(warranty => warranty.Id == warrantyId).FirstOrDefault();

            if (warrantyDocument == null) return false;

            return warrantyDocument.status == 0 ? false : true;
        }

        public bool IsWarrantyHistoryExists(ObjectId warrantyId)
        {
            var warrantyHistoryDocument = WarrantyHistoryCollection.Find(warrantyHistory => warrantyHistory.WarrantyId == warrantyId && warrantyHistory.RepairStatus == 1).FirstOrDefault();

            if (warrantyHistoryDocument == null) return false;

            return true;
        }

        private void buttonTao_Click(object sender, EventArgs e)
        {
            if (dataGridViewWarranty.CurrentRow == null) return;

            ObjectId warrantyId = ObjectId.Parse(dataGridViewWarranty.CurrentRow.Cells[0].Value.ToString());

            if (!CheckWarrantyStatus(warrantyId))
            {
                MessageBox.Show("Đã hết hạn bảo hành");
                return;
            }

            if (IsWarrantyHistoryExists(warrantyId))
            {
                MessageBox.Show("Sản phẩm đang thực hành bảo hành");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxDes.Text) || string.IsNullOrWhiteSpace(comboBoxEmployee.Text))
            {
                MessageBox.Show("Nhập mô tả tình trạng sản phẩm");
                return;
            } 

            string description = textBoxDes.Text;
            ObjectId repairStaff = (comboBoxEmployee.SelectedItem as Employee).Id;
            int repairStatus = 1;
            int warrantyType = (comboBoxType.SelectedItem as WarrantyType).Type;
            int total = 0;
            DateTime receptionDate = DateTime.Now;

            WarrantyHistory warrantyHistory = new WarrantyHistory(warrantyId, description, receptionDate, new List<RepairDetail>(), repairStatus, null, repairStaff, warrantyType, total);
            WarrantyHistoryCollection.InsertOne(warrantyHistory);
            loadWarrantyHistory();
        }

        private void buttonHoanThanh_Click(object sender, EventArgs e)
        {
            ObjectId warrantyHistoryId = ObjectId.Parse(dataGridViewHistory.CurrentRow.Cells[0].Value.ToString());
            WarrantyHistory warrantyHistory = WarrantyHistoryCollection.Find(w => w.Id == warrantyHistoryId).FirstOrDefault();
            int repairStatus = warrantyHistory.RepairStatus;

            if (repairStatus == 0)
            {
                MessageBox.Show("Lịch sử bảo hành đã hoàn thành");
                return;
            }

            ObjectId warrantyId = ObjectId.Parse(textBoxId.Text);
            string description = textBoxDes.Text;
            ObjectId repairStaff = (comboBoxEmployee.SelectedItem as Employee).Id;
            repairStatus = 0;
            int warrantyType = (comboBoxType.SelectedItem as WarrantyType).Type;
            int total = warrantyHistory.Total;
            DateTime receptionDate = warrantyHistory.ReceptionDate;
            DateTime returnDate = DateTime.Now;

            var rowWarrantyHistory = Builders<WarrantyHistory>.Update
                .Set("warranty_id", warrantyId)
                .Set("description", description)
                .Set("reception_date", receptionDate)
                .Set("repair_details", repairDetails)
                .Set("repair_status", repairStatus)
                .Set("return_date", returnDate)
                .Set("repair_staff", repairStaff)
                .Set("warranty_type", warrantyType)
                .Set("total", total);

            WarrantyHistoryCollection.UpdateOne(w => w.Id == warrantyHistoryId, rowWarrantyHistory);
            loadRepairParts(warrantyHistoryId);
            loadWarrantyHistory();
        }
    }
}
