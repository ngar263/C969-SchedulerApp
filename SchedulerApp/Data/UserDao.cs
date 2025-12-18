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
        public User GetByUsername(string username) {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand("SELECT userId, userName, password, active FROM user WHERE userName = @u", conn)) {
                cmd.Parameters.AddWithValue("@u", username.Trim());
                try {
                    using (var reader = cmd.ExecuteReader()) {
                        if (!reader.Read()) return null;
                        return new User() {
                            UserId = Convert.ToInt32(reader["userId"]),
                            UserName = reader["userName"].ToString()!,
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
    }
}
