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
        private static string _connectionString = "Server=127.0.0.1;Port=3306;Database=client_schedule;Uid=root;Pwd=password;";
        //private static string _connectionString = "Server=127.0.0.1;Port=3306;Database=client_schedule;Uid=sqlUser;Pwd=Passw0rd!;";

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
