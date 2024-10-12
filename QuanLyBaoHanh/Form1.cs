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
    public partial class Form1 : Form
    {
        ProductForm producF;
        EmployeeForm employeeForm;
        CustomerForm customerForm;
        WarrantyForm warrantyForm;
        WarrantyHistoryForm warrantyHistoryForm;
        private List<Form> openForms = new List<Form>();
        public Form1()
        {
            InitializeComponent();
            this.buttonProduct.Click += ButtonProduct_Click;
            this.buttonEmp.Click += ButtonEmp_Click;
            this.buttonCustomer.Click += ButtonCustomer_Click;
            this.buttonWarranty.Click += ButtonWarranty_Click;
            this.buttonHistoryWarranty.Click += ButtonHistoryWarranty_Click;
        }
        private Stack<Form> formStack = new Stack<Form>();

        private void ShowForm(Form newForm)
        {
            if (panelMain.Controls.Count > 0)
            { 
                var currentForm = panelMain.Controls[0] as Form;
                if (currentForm != null)
                {
                    currentForm.Close();
                    panelMain.Controls.Remove(currentForm);
                }
            }
            newForm.TopLevel = false;
            newForm.FormBorderStyle = FormBorderStyle.None;
            newForm.Dock = DockStyle.Fill;

            panelMain.Controls.Clear();
            panelMain.Controls.Add(newForm);
            newForm.Show();
        }

        private void ButtonHistoryWarranty_Click(object sender, EventArgs e)
        {
            ShowForm(new WarrantyHistoryForm());
        }

        private void ButtonWarranty_Click(object sender, EventArgs e)
        {
            ShowForm(new WarrantyForm());
        }

        private void ButtonCustomer_Click(object sender, EventArgs e)
        {
            ShowForm(new CustomerForm());
        }

        private void ButtonEmp_Click(object sender, EventArgs e)
        {
            ShowForm(new EmployeeForm());
        }

        private void ButtonProduct_Click(object sender, EventArgs e)
        {
            ShowForm(new ProductForm());
        }



    }
}
