using MySql.Data.MySqlClient;
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
        private readonly CityDao _cityDao;
        public CustomerService(ICustomerDao customerDao) => _customerDao = customerDao;

        public int AddCustomer(Customer customer, Address address, User user, string cityName, string countryName) {
            customer.CustomerName = customer.CustomerName?.Trim()!;
            address.Address1 = address.Address1.Trim();
            address.Phone = address.Phone.Trim();

            ValidationService.ValidateCustomerInput(customer.CustomerName, address.Address1, address.Phone, cityName, countryName);

            try {
                return _customerDao.AddCustomer(customer, address, user, cityName, countryName);
            } catch (Exception ex) {
                throw new ApplicationException("Failed to add customer.", ex);
            }
        }

        public void UpdateCustomer(Customer customer, Address address, User user, string cityName, string countryName, bool isActive) {
            using var conn = Database.GetConnection();
            using var trans = conn.BeginTransaction();
            try {
                customer.Active = isActive;
                int countryId = ResolveOrCreateCountry(countryName, user, conn, trans);
                int cityId = ResolveOrCreateCity(cityName, countryId, user, conn, trans);
                using (var cmd = new MySqlCommand(
                    @"UPDATE address
                      SET address = @address1,
                      address2 = @address2,
                      cityId = @cityId,
                      postalCode = @postalCode,
                      phone = @phone,
                      lastUpdate = NOW(),
                      lastUpdateBy = @user
                      WHERE addressId = @addressId",
                    conn, trans)) {
                    cmd.Parameters.AddWithValue("@address1", address.Address1.Trim());
                    cmd.Parameters.AddWithValue("@address2", address.Address2?.Trim() ?? "");
                    cmd.Parameters.AddWithValue("@cityId", cityId);
                    cmd.Parameters.AddWithValue("@postalCode", address.PostalCode.Trim());
                    cmd.Parameters.AddWithValue("@phone", address.Phone.Trim());
                    cmd.Parameters.AddWithValue("@addressId", address.AddressId);
                    cmd.Parameters.AddWithValue("@user", user.Username);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new MySqlCommand(
                    @"UPDATE customer
                      SET customerName = @name,
                      active = @active,
                      lastUpdate = NOW(),
                      lastUpdateBy = @user
                      WHERE customerId = @id",
                    conn, trans)) {
                    cmd.Parameters.AddWithValue("@name", customer.CustomerName.Trim());
                    cmd.Parameters.AddWithValue("@active", customer.Active ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", customer.CustomerId);
                    cmd.Parameters.AddWithValue("@user", user.Username);
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            } catch {
                trans.Rollback();
                throw;
            }
        }

        public void DeleteCustomer(int customerId) {
            try {
                _customerDao.DeleteCustomer(customerId);
            } catch (Exception ex) {
                throw new ApplicationException("Failed to delete customer.", ex);
            }
        }

        private int ResolveOrCreateCountry(string countryName, User user, MySqlConnection conn, MySqlTransaction trans) {
            using var cmd = new MySqlCommand(
                "SELECT countryId FROM country WHERE country = @name",
                conn, trans);

            cmd.Parameters.AddWithValue("@name", countryName.Trim());

            var result = cmd.ExecuteScalar();
            if (result != null)
                return Convert.ToInt32(result);

            using var insert = new MySqlCommand(
                @"INSERT INTO country
                  (country, createDate, createdBy, lastUpdate, lastUpdateBy)
                  VALUES
                  (@name, NOW(), @user, NOW(), @user);
                  SELECT LAST_INSERT_ID();",
                conn, trans);

            insert.Parameters.AddWithValue("@name", countryName.Trim());
            insert.Parameters.AddWithValue("@user", user.Username);

            return Convert.ToInt32(insert.ExecuteScalar());
        }

        private int ResolveOrCreateCity(string cityName, int countryId, User user, MySqlConnection conn, MySqlTransaction trans) {
            using var cmd = new MySqlCommand(
                @"SELECT cityId FROM city
                  WHERE city = @name AND countryId = @countryId",
                conn, trans);

            cmd.Parameters.AddWithValue("@name", cityName.Trim());
            cmd.Parameters.AddWithValue("@countryId", countryId);

            var result = cmd.ExecuteScalar();
            if (result != null)
                return Convert.ToInt32(result);

            using var insert = new MySqlCommand(
                @"INSERT INTO city
                (city, countryId, createDate, createdBy, lastUpdate, lastUpdateBy)
                VALUES
                (@name, @countryId, NOW(), @user, NOW(), @user);
                SELECT LAST_INSERT_ID();",
                conn, trans);

            insert.Parameters.AddWithValue("@name", cityName.Trim());
            insert.Parameters.AddWithValue("@countryId", countryId);
            insert.Parameters.AddWithValue("@user", user.Username);

            return Convert.ToInt32(insert.ExecuteScalar());
        }
    }
}
