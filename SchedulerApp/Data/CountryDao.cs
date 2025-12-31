using MySql.Data.MySqlClient;
using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Data {
    public class CountryDao {
        public int GetOrCreateCountry(string countryName, string username, MySqlConnection conn,MySqlTransaction trans) {
            using var checkCmd = new MySqlCommand(
                "SELECT countryId FROM country WHERE country = @country",
                conn, trans);

            checkCmd.Parameters.AddWithValue("@country", countryName.Trim());

            var existing = checkCmd.ExecuteScalar();
            if (existing != null)
                return Convert.ToInt32(existing);

            using var insertCmd = new MySqlCommand(
                @"INSERT INTO country (country, createDate, createdBy, lastUpdate, lastUpdateBy)
                  VALUES (@country, NOW(), @user, NOW(), @user);
                  SELECT LAST_INSERT_ID();",
                conn, trans);

            insertCmd.Parameters.AddWithValue("@country", countryName.Trim());
            insertCmd.Parameters.AddWithValue("@user", username);

            return Convert.ToInt32(insertCmd.ExecuteScalar());
        }

        public List<Country> GetAllCountries() {
            var countries = new List<Country>();

            using (var conn = Database.GetConnection()) {
                using (var cmd = new MySqlCommand(
                    "SELECT countryId, country FROM country ORDER BY country",
                    conn))
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        countries.Add(new Country {
                            CountryId = reader.GetInt32("countryId"),
                            Name = reader.GetString("country")
                        });
                    }
                }
            }

            return countries;
        }
    }
}