using SchedulerApp.Data;
using SchedulerApp.Models;
using SchedulerApp.Services;
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
    /// Interaction logic for CustomersWindow.xaml
    /// </summary>
    public partial class CustomersWindow : Window {
        private readonly CustomerService _customerService;
        private readonly ICustomerDao _customerDao;
        private readonly CityDao _cityDao;
        private readonly CountryDao _countryDao;
        private readonly User _currentUser;
        private List<City> _cities;
        private List<Country> _countries;
        
        public CustomersWindow(User user) {
            InitializeComponent();
            _customerDao = new CustomerDao();
            _customerService = new CustomerService((ICustomerDao)_customerDao);
            _cityDao = new CityDao();
            _countryDao = new CountryDao();
            _currentUser = user;
            LoadCustomers();
        }

        private void LoadCustomers() {
            dgCustomers.ItemsSource = _customerDao.GetAllCustomers();
            _cities = _cityDao.GetAllCities();
            _countries = _countryDao.GetAllCountries();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            var editor = new CustomerEditorWindow(_cities, _countries);
            if (editor.ShowDialog() == true) {
                try {
                    var custId = _customerService.AddCustomer(editor.Customer, editor.Address, _currentUser, editor.cmbCity.Text.Trim(), editor.cmbCountry.Text.Trim());
                    LoadCustomers();
                } catch (Exception ex) {
                    MessageBox.Show("Error adding customer: " + ex.Message);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            if (dgCustomers.SelectedItem is Customer selected) {
                var customerId = selected.CustomerId;
                var address = _customerDao.GetAddressById(customerId);
                var city = _customerDao.GetCityById(address.CityId);
                var country = _customerDao.GetCountryById(city.CountryId);
                var editor = new CustomerEditorWindow(selected, address, city, country, _cities, _countries);
                if (editor.ShowDialog() == true) {
                    try {
                        _customerService.UpdateCustomer(editor.Customer, editor.Address, _currentUser, editor.cmbCity.Text, editor.cmbCountry.Text, editor.chkActive.IsChecked!.Value);
                        LoadCustomers();
                    } catch (Exception ex) {
                        MessageBox.Show("Error editing customer: " + ex.Message);
                    }
                }
            } else {
                MessageBox.Show("Select a customer first.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            if (dgCustomers.SelectedItem is Customer selected) {
                if (MessageBox.Show("Delete selected customer?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                    try {
                        _customerService.DeleteCustomer(selected.CustomerId);
                        LoadCustomers();
                    } catch (Exception ex) {
                        MessageBox.Show("Error deleting customer: " + ex.Message);
                    }
                }
            }
        }
    }
}
