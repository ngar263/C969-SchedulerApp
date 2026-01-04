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
        int AddCustomer(Customer customer, Address address, User user, string cityName, string countryName, bool active);
        void UpdateCustomer(Customer customer, Address address, User user, string cityName, int countryId);
        void DeleteCustomer(int customerId);
        string GetNameById(int customerId);
        Address GetAddressById(int customerId);
        Country GetCountryById(int countryId);
        City GetCityById(int cityId);
    }
}
