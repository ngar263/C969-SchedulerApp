using MySql.Data.MySqlClient;
using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchedulerApp.Data
{
    public class UserDao : IUserDao
    {
        public void CreateTestUser() {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                "INSERT INTO user (userName, password, active, createDate, createdBy, lastUpdate, lastUpdateBy) " +
                "VALUES ('test', 'test', 1, NOW(), 'test', NOW(), 'test');", conn)) {
                cmd.ExecuteNonQuery();
            }
        }

        public User GetByUsername(string username) {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT userId, userName, password, active FROM user WHERE userName = @userName", conn)) {
                cmd.Parameters.AddWithValue("@userName", username.Trim());
                try {
                    using (var reader = cmd.ExecuteReader()) {
                        if (!reader.Read()) return null;
                        return new User() {
                            UserId = Convert.ToInt32(reader["userId"]),
                            Username = reader["userName"].ToString()!,
                            Password = reader["password"].ToString()!,
                            Active = Convert.ToInt32(reader["active"]) == 1
                        };
                    } 
                }catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    return null!;
                }
            }
        }

        public List<User> GetAllUsers() {
            var list = new List<User>();

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT userId, userName, password, active FROM user", conn))
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    list.Add(new User {
                        UserId = Convert.ToInt32(reader["userId"]),
                        Username = reader["userName"].ToString()!,
                        Password = reader["password"].ToString()!,
                        Active = Convert.ToInt32(reader["active"]) == 1
                    });
                }
            }

            return list;
        }

    }
}
