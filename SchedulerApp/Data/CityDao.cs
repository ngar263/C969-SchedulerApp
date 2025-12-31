using MySql.Data.MySqlClient;
using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Data {
    public class CityDao {
        public int GetOrCreateCity(
            string cityName,
            int countryId,
            string username,
            MySqlConnection conn,
            MySqlTransaction trans) {
            using var checkCmd = new MySqlCommand(
                @"SELECT cityId FROM city
                  WHERE city = @city AND countryId = @countryId",
                conn, trans);

            checkCmd.Parameters.AddWithValue("@city", cityName.Trim());
            checkCmd.Parameters.AddWithValue("@countryId", countryId);

            var existing = checkCmd.ExecuteScalar();
            if (existing != null)
                return Convert.ToInt32(existing);

            using var insertCmd = new MySqlCommand(
                @"INSERT INTO city (city, countryId, createDate, createdBy, lastUpdate, lastUpdateBy)
                  VALUES (@city, @countryId, NOW(), @user, NOW(), @user);
                  SELECT LAST_INSERT_ID();",
                conn, trans);

            insertCmd.Parameters.AddWithValue("@city", cityName.Trim());
            insertCmd.Parameters.AddWithValue("@countryId", countryId);
            insertCmd.Parameters.AddWithValue("@user", username);

            return Convert.ToInt32(insertCmd.ExecuteScalar());
        }
        public List<City> GetAllCities() {
            var cities = new List<City>();

            using var conn = Database.GetConnection();
            using var cmd = new MySqlCommand(@"
                SELECT c.cityId, c.city, co.country
                FROM city c
                JOIN country co ON c.countryId = co.countryId
                ORDER BY c.city;", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                cities.Add(new City {
                    CityId = reader.GetInt32("cityId"),
                    Name = reader.GetString("city"),
                    Country = reader.GetString("country")
                });
            }

            return cities;
        }
    }
}
