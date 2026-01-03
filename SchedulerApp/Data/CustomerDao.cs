using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
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
                @"SELECT c.customerId, c.customerName, c.active, a.address, a.phone, a.postalCode, ci.city, co.country
                  FROM customer c
                  JOIN address a ON c.addressId = a.addressId
                  JOIN city ci ON a.cityId = ci.cityId
                  JOIN country co ON ci.countryId = co.countryId",
                conn))
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    list.Add(new Customer {
                        CustomerId = Convert.ToInt32(reader["customerId"]),
                        CustomerName = reader["customerName"].ToString()!,
                        Address = reader["address"].ToString()!,
                        PhoneNumber = reader["phone"].ToString()!,
                        PostalCode = reader["postalCode"].ToString()!,
                        City = reader["city"].ToString()!,
                        Country = reader["country"].ToString()!,
                        Active = Convert.ToInt32(reader["active"]) == 1
                    });
                }
            }

            return list;
        }


        public int AddCustomer(Customer customer, Address address, User user, string cityName, string countryName, bool active) {
            using (var conn = Database.GetConnection()) {
                using (var trans = conn.BeginTransaction()) {
                    try {
                        int countryId;
                        using (var cmd = new MySqlCommand(
                            "SELECT countryId FROM country WHERE country = @country",
                            conn, trans)) {
                            cmd.Parameters.AddWithValue("@country", countryName.Trim());
                            var result = cmd.ExecuteScalar();

                            if (result != null) {
                                countryId = Convert.ToInt32(result);
                            } else {
                                using var insertCmd = new MySqlCommand(
                                    @"INSERT INTO country
                                     (country, createDate, createdBy, lastUpdate, lastUpdateBy)
                                     VALUES
                                     (@country, NOW(), @user, NOW(), @user);
                                     SELECT LAST_INSERT_ID();",
                                    conn, trans);

                                insertCmd.Parameters.AddWithValue("@country", countryName.Trim());
                                insertCmd.Parameters.AddWithValue("@user", user.Username);
                                countryId = Convert.ToInt32(insertCmd.ExecuteScalar());
                            }
                        }
                        int cityId;
                        using (var cmd = new MySqlCommand(
                            @"SELECT cityId FROM city
                              WHERE city = @city AND countryId = @countryId",
                            conn, trans)) {
                            cmd.Parameters.AddWithValue("@city", cityName.Trim());
                            cmd.Parameters.AddWithValue("@countryId", countryId);
                            var result = cmd.ExecuteScalar();

                            if (result != null) {
                                cityId = Convert.ToInt32(result);
                            } else {
                                using var insertCmd = new MySqlCommand(
                                    @"INSERT INTO city
                                      (city, countryId, createDate, createdBy, lastUpdate, lastUpdateBy)
                                      VALUES
                                      (@city, @countryId, NOW(), @user, NOW(), @user);
                                      SELECT LAST_INSERT_ID();",
                                    conn, trans);

                                insertCmd.Parameters.AddWithValue("@city", cityName.Trim());
                                insertCmd.Parameters.AddWithValue("@countryId", countryId);
                                insertCmd.Parameters.AddWithValue("@user", user.Username);
                                cityId = Convert.ToInt32(insertCmd.ExecuteScalar());
                            }
                        }
                        int addressId;
                        using (var cmd = new MySqlCommand(
                            @"INSERT INTO address
                              (address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdate, lastUpdateBy)
                              VALUES
                              (@address1, @address2, @cityId, @postalCode, @phone, NOW(), @user, NOW(), @user);
                              SELECT LAST_INSERT_ID();",
                            conn, trans)) {
                            cmd.Parameters.AddWithValue("@address1", address.Address1.Trim());
                            cmd.Parameters.AddWithValue("@address2", address.Address2?.Trim() ?? "");
                            cmd.Parameters.AddWithValue("@cityId", cityId);
                            cmd.Parameters.AddWithValue("@postalCode", address.PostalCode.Trim());
                            cmd.Parameters.AddWithValue("@phone", address.Phone.Trim());
                            cmd.Parameters.AddWithValue("@user", user.Username);

                            addressId = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        using (var cmd = new MySqlCommand(
                            @"INSERT INTO customer
                              (customerName, addressId, active, createDate, createdBy, lastUpdate, lastUpdateBy)
                              VALUES
                              (@name, @addressId, @active, NOW(), @user, NOW(), @user);
                              SELECT LAST_INSERT_ID();",
                            conn, trans)) {
                            cmd.Parameters.AddWithValue("@name", customer.CustomerName.Trim());
                            cmd.Parameters.AddWithValue("@addressId", addressId);
                            cmd.Parameters.AddWithValue("@active", active);
                            cmd.Parameters.AddWithValue("@user", user.Username);

                            int customerId = Convert.ToInt32(cmd.ExecuteScalar());
                            trans.Commit();
                            return customerId;
                        }
                    } catch {
                        trans.Rollback();
                        throw;
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

        public Address GetAddressById(int customerId) {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                @"SELECT a.addressId, a.address, a.address2, a.cityId,
                 a.postalCode, a.phone
                 FROM customer c
                 JOIN address a ON c.addressId = a.addressId
                 WHERE c.customerId = @customerId", conn)) {
                cmd.Parameters.AddWithValue("@customerId", customerId);
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


        public void UpdateCustomer(Customer customer, Address address, User user, string cityName, int countryId) {
            using (var conn = Database.GetConnection()) {
                using (var trans = conn.BeginTransaction()) {
                    try {
                        int cityId;

                        using (var cmd = new MySqlCommand(
                            @"SELECT cityId FROM city
                              WHERE city = @city AND countryId = @countryId",
                            conn, trans)) {
                            cmd.Parameters.AddWithValue("@city", cityName.Trim());
                            cmd.Parameters.AddWithValue("@countryId", countryId);

                            var result = cmd.ExecuteScalar();
                            if (result != null) {
                                cityId = Convert.ToInt32(result);
                            } else {
                                using var insertCmd = new MySqlCommand(
                                    @"INSERT INTO city
                                      (city, countryId, createDate, createdBy, lastUpdate, lastUpdateBy)
                                      VALUES
                                      (@city, @countryId, NOW(), @user, NOW(), @user);
                                      SELECT LAST_INSERT_ID();",
                                    conn, trans);

                                insertCmd.Parameters.AddWithValue("@city", cityName.Trim());
                                insertCmd.Parameters.AddWithValue("@countryId", countryId);
                                insertCmd.Parameters.AddWithValue("@user", user.Username);

                                cityId = Convert.ToInt32(insertCmd.ExecuteScalar());
                            }
                        }
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
            }
        }


        public Country GetCountryById(int countryId) {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                @"SELECT co.countryId, co.country
                 FROM country co
                 JOIN city c ON c.countryId = co.countryId
                 WHERE co.countryId = @countryId", conn)) {
                cmd.Parameters.AddWithValue("@countryId", countryId);
                using (var reader = cmd.ExecuteReader()) {
                    if (!reader.Read()) return null;

                    return new Country {
                        Name = reader["country"].ToString()!,
                        CountryId = Convert.ToInt32(reader["countryId"])!
                    };
                }
            }
        }

        public City GetCityById(int cityId) {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                @"SELECT c.cityId, c.city, c.countryId
                 FROM city c
                 JOIN address a ON c.cityId = a.cityId
                 WHERE c.cityId = @cityId", conn)) {
                cmd.Parameters.AddWithValue("@cityId", cityId);
                using (var reader = cmd.ExecuteReader()) {
                    if (!reader.Read()) return null;

                    return new City {
                        Name = reader["city"].ToString()!,
                        CityId = Convert.ToInt32(reader["cityId"]),
                        CountryId = Convert.ToInt32(reader["countryId"])
                    };
                }
            }
        }
    }
}
