using MySql.Data.MySqlClient;
using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Data
{
    public class CustomerDao : ICustomerDao {
        public List<Customer> GetAllCustomers() {
            var list = new List<Customer>();
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT cusomterId, customerName, addressId, active FROM customer",conn))
            using (var reader =  cmd.ExecuteReader()) {
                while (reader.Read()) {
                    list.Add(new Customer {
                        CustomerId = Convert.ToInt32(reader["customerId"]),
                        CustomerName = reader["customerName"].ToString()!,
                        AddressId = Convert.ToInt32(reader["addressId"]),
                        Active = Convert.ToInt32(reader["active"]) == 1
                    });
                }
            }
            return list;
        }
        public int AddCustomer(Customer customer, Address address) {
            using (var conn = Database.GetConnection())
            using (var trans =  conn.BeginTransaction()) {
                using (var cmd = new MySqlCommand(
                    @"INSERT INTO address (address, address2, cityId, postalCode, phone, createDate, createdBy)
                     VALUES (@address1, @address2, @cityId, @postalCode, @phone, NOW(), 'system'); SELECT LAST_INSERT_ID();", conn, trans)) {
                    cmd.Parameters.AddWithValue("@address", address.Address1);
                    cmd.Parameters.AddWithValue("@address2", address.Address2);
                    cmd.Parameters.AddWithValue("@cityId", address.CityId);
                    cmd.Parameters.AddWithValue("@postalCode", address.PostalCode);
                    cmd.Parameters.AddWithValue("@phone", address.Phone);
                    var addressId = Convert.ToInt32(cmd.ExecuteScalar());

                    using (var cmd2 = new MySqlCommand(
                        @"INSERT INTO customer (customerName, addressId, active, createDate, createdBy)
                          Values (@name, @addressId, @active, NOW(), 'system'); SELECT LAST_INSERT_ID();", conn, trans)) {
                        cmd.Parameters.AddWithValue("@name", customer.CustomerName);
                        cmd.Parameters.AddWithValue("@addressId", customer.AddressId);
                        cmd.Parameters.AddWithValue("@active", customer.Active ? 1 : 0);
                        var customerId = Convert.ToInt32(cmd2.ExecuteScalar());
                        trans.Commit();
                        return customerId;
                    }
                }
            }
        }

        public void DeleteCustomer(int customerId) {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                "DELETE FROM customer WHERE customerId=@id", conn)) {
                cmd.Parameters.AddWithValue("@id", customerId);
                cmd.ExecuteNonQuery();
            }
        }

        public Address GetAddressById(int addressId) {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT addressId, address, address2, cityId, postalCode, phone FROM address WHERE addressId=@addressId", conn)) {
                cmd.Parameters.AddWithValue("@addressId", addressId);
                using (var reader = cmd.ExecuteReader()) {
                    if (!reader.Read()) return null;
                    return new Address {
                        AddressId = Convert.ToInt32(reader["addressId"]),
                        Address1 = reader["address"].ToString()!,
                        Address2 = reader["address2"].ToString()!,
                        CityId = Convert.ToInt32(reader["cityId"]),
                        PostalCode = reader["postalCode"].ToString()!,
                        Phone = reader["phone"].ToString()!
                    };
                }
            }
        }


        public void UpdateCustomer(Customer customer, Address address) {
            using (var conn = Database.GetConnection())
            using (var trans = conn.BeginTransaction()) {
                using (var cmd = new MySqlCommand(
                    @"UPDATE address SET address=@address1, address2=@address2, cityId=@cityId, postalCode=@postalCode, phone=@phone, lastUpdate=NOW()
                      WHERE addressId=@addressId", conn, trans)) {
                    cmd.Parameters.AddWithValue("@address1", address.Address1);
                    cmd.Parameters.AddWithValue("@address2", address.Address2 ?? "");
                    cmd.Parameters.AddWithValue("@cityId", address.CityId);
                    cmd.Parameters.AddWithValue("@postalCode", address.PostalCode ?? "");
                    cmd.Parameters.AddWithValue("@phone", address.Phone ?? "");
                    cmd.Parameters.AddWithValue("@addressId", address.AddressId);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd2 = new MySqlCommand(
                    @"UPDDATE customer SET customerName=@name, active=@active, lastUpdate=NOW()
                      WHERE customerId=@id", conn, trans)) {
                    cmd2.Parameters.AddWithValue("@name", customer.CustomerName);
                    cmd2.Parameters.AddWithValue("@active", customer.Active ? 1 : 0);
                    cmd2.Parameters.AddWithValue("@id", customer.CustomerId);
                    cmd2.ExecuteNonQuery();
                }
                trans.Commit();
            } 
        }
    }
}
