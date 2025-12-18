using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchedulerApp.Data
{
    public static class Database
    {
        private static string _connectionString = "Server=127.0.0.1;Port=3306;Database=client_schedule;User ID=sqlUser;Password=Passw0rd!;SslMode=None;";

        public static MySqlConnection GetConnection() {
            try {
                var conn = new MySqlConnection(_connectionString);
                conn.Open();
                return conn;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return null!;
            }
        }
    }
}
