using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Data
{
    public interface ICustomerDao
    {
        List<Customer> GetAllCustomers();
        int AddCustomer(Customer customer, Address address);
        void UpdateCustomer(Customer customer, Address address);
        void DeleteCustomer(int customerId);
        Address GetAddressById(int addressId);
    }
}
