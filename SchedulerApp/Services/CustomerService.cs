using SchedulerApp.Data;
using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Services
{
    public class CustomerService
    {
        private readonly ICustomerDao _customerDao;
        public CustomerService(ICustomerDao customerDao) => _customerDao = customerDao;

        public int AddCustomer(Customer customer, Address address) {
            customer.CustomerName = customer.CustomerName?.Trim()!;
            address.Address1 = address.Address1.Trim();
            address.Phone = address.Phone.Trim();

            ValidationService.ValidateCustomerInput(customer.CustomerName, address.Address1, address.Phone);

            try {
                return _customerDao.AddCustomer(customer, address);
            } catch (Exception ex) {
                throw new ApplicationException("Failed to add customer.", ex);
            }
        }

        public void UpdateCustomer(Customer customer, Address address) {
            customer.CustomerName = customer.CustomerName.Trim()!;
            address.Address1 = address.Address1.Trim();
            address.Phone = address.Phone.Trim()!;

            ValidationService.ValidateCustomerInput(customer.CustomerName, address.Address1, address.Phone);

            try {
                _customerDao.UpdateCustomer(customer, address);
            } catch (Exception ex) {
                throw new ApplicationException("Failed to update customer.", ex);
            }
        }

        public void DeleteCustomer(int customerId) {
            try {
                _customerDao.DeleteCustomer(customerId);
            } catch (Exception ex) {
                throw new ApplicationException("Failed to delete customer.", ex);
            }
        }
    }
}
