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
    public partial class EmployeeForm : Form
    {
        static MongoClient client = new MongoClient();
        static IMongoDatabase db = client.GetDatabase("quanlibaohanh");
        static IMongoCollection<Employee> collection = db.GetCollection<Employee>("employees");

        public EmployeeForm()
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
            textBoxPosition.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            textBoxPhoneNumber.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            textBoxEmail.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ObjectId id = ObjectId.Parse(textBoxID.Text);
            var updateDef = Builders<Employee>.Update
                .Set("name", textBoxName.Text)
                .Set("position", textBoxPosition.Text)
                .Set("phone_number", textBoxPhoneNumber.Text)
                .Set("email", textBoxEmail.Text);
            collection.UpdateOne(s => s.Id == id, updateDef);
            ReadAllDocument();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ObjectId id = ObjectId.Parse(textBoxID.Text);
            collection.DeleteOne(s => s.Id == id);
            ReadAllDocument();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Employee emp = new Employee(
                textBoxName.Text,
                textBoxPosition.Text,
                textBoxPhoneNumber.Text,
                textBoxEmail.Text
            );
            collection.InsertOne(emp);
            ReadAllDocument();
        }

        public void ReadAllDocument()
        {
            List<Employee> lstEmp = collection.AsQueryable().ToList();
            dataGridView1.DataSource = lstEmp;
        }
    }
}
