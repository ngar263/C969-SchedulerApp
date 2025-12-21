using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SchedulerApp.Views {
    /// <summary>
    /// Interaction logic for CustomerEditorWindow.xaml
    /// </summary>
    public partial class CustomerEditorWindow : Window {
        public Customer Customer { get; private set; } = new Customer();
        public Address Address { get; private set; } = new Address();
        public CustomerEditorWindow() {
            InitializeComponent();
            this.Title = "Add Customer";
        }

        public CustomerEditorWindow(Customer customer, Address address) : this() {
            InitializeComponent();
            Customer = customer;
            Address = address ?? new Address();
            txtName.Text = Customer.CustomerName;
            txtAddress.Text = Address.Address1;
            txtPhone.Text = Address.Phone;
            txtPostalCode.Text = Address.PostalCode;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            Customer.CustomerName = txtName.Text;
            Address.Address1 = txtAddress.Text;
            Address.Phone = txtPhone.Text;
            Address.PostalCode = txtPostalCode.Text;
            if (string.IsNullOrEmpty(Customer.CustomerName)) { MessageBox.Show("Please enter a name."); return; } ;
            if (string.IsNullOrEmpty(Address.Address1)) { MessageBox.Show("Please enter an address."); return; } ;
            if (string.IsNullOrEmpty(Address.Phone)) { MessageBox.Show("Please enter a phone number."); return; } ;
            if (string.IsNullOrEmpty(Address.PostalCode)) { MessageBox.Show("Please enter a postal code."); return; } ;

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
            this.Close();
        }
    }
}
