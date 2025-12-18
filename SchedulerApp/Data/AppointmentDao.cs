using MySql.Data.MySqlClient;
using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Data
{
    public class AppointmentDao : IAppointmentDao {
        public int AddAppointment(Appointment apt) {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                @"INSERT INTO appointment (customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy)
                  VALUES (@customerId, @userId, @title, @description, @location, @contact, @type, @url, @start, @end, NOW(), 'system');
                  SELECT LAST_INSERT_ID();", conn)) 
            {
                cmd.Parameters.AddWithValue("@customerId", apt.CustomerId);
                cmd.Parameters.AddWithValue("@userId", apt.UserId);
                cmd.Parameters.AddWithValue("@title", apt.Title);
                cmd.Parameters.AddWithValue("@description", apt.Description);
                cmd.Parameters.AddWithValue("@location", apt.Location);
                cmd.Parameters.AddWithValue("@contact", apt.Contact);
                cmd.Parameters.AddWithValue("@type", apt.Type);
                cmd.Parameters.AddWithValue("@url", apt.Url);
                cmd.Parameters.AddWithValue("@start", apt.StartUtc);
                cmd.Parameters.AddWithValue("@end", apt.EndUtc);

                var id = Convert.ToInt32(cmd.ExecuteScalar());
                return id;
            }
        }

        public void UpdateAppointment(Appointment apt) {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                @"UPDATE appointment SET customerId=@customerId, userId=@userId, title=@title, description=@description,
                  location=@location, contact=@contact, type=@type, url=@url, start=@start, end=@end, lastUpdate=NOW()
                  WHERE appointmentId=@id", conn)) 
            {
                cmd.Parameters.AddWithValue("@customerId", apt.CustomerId);
                cmd.Parameters.AddWithValue("@userId", apt.UserId);
                cmd.Parameters.AddWithValue("@title", apt.Title);
                cmd.Parameters.AddWithValue("@description", apt.Description);
                cmd.Parameters.AddWithValue("@location", apt.Location);
                cmd.Parameters.AddWithValue("@contact", apt.Contact);
                cmd.Parameters.AddWithValue("@type", apt.Type);
                cmd.Parameters.AddWithValue("@url", apt.Url);
                cmd.Parameters.AddWithValue("@start", apt.StartUtc);
                cmd.Parameters.AddWithValue("@end", apt.EndUtc);
                cmd.Parameters.AddWithValue("@id", apt.AppointmentId);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteAppointment(int appointmentId) {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                "DELETE FROM appointment WHERE appointmentId = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", appointmentId);
                cmd.ExecuteNonQuery();
            }
        }

        public List<Appointment> GetAllAppointments() {
            var list = new List<Appointment>();
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                @"SELECT appointmentId, customerId, userId, title, description, location, contact, type, url, start, end 
                FROM appointment",
                conn)) {
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(MapReaderToAppointment(reader));
                    }
                }
            }
            return list;
        }

        private Appointment MapReaderToAppointment(MySqlDataReader reader) {
            return new Appointment {
                AppointmentId = Convert.ToInt32(reader["appointmentId"]),
                CustomerId = Convert.ToInt32(reader["customerId"]),
                UserId = Convert.ToInt32(reader["userid"]),
                Title = reader["title"].ToString()!,
                Description = reader["description"].ToString()!,
                Location = reader["location"].ToString()!,
                Contact = reader["contact"].ToString()!,
                Type = reader["type"].ToString()!,
                Url = reader["url"].ToString()!,
                StartUtc = Convert.ToDateTime(reader["start"]),
                EndUtc = Convert.ToDateTime(reader["end"])
            };
        }

        public List<Appointment> GetAppointmentsForUserBetween(int userId, DateTime startUtc, DateTime endUtc) {
            var list = new List<Appointment>();
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                @"SELECT appointmentId, customerId, userId, title, description, location, contact, type, url, start, end
                FROM appointment
                WHERE userId = @userId AND start >= @start AND start < @end",
                conn)) {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@start", startUtc);
                cmd.Parameters.AddWithValue("@end", endUtc);
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(MapReaderToAppointment(reader));
                    }
                }
            }
            return list;
        }

        public List<Appointment> GetAppointmentsForUserInRange(int userId, DateTime fromUtc, DateTime toUtc) {
            return GetAppointmentsForUserBetween(userId, fromUtc, toUtc);
        }

        public bool HasOverlappingAppointment(int userId, DateTime newStartUtc, DateTime newEndUtc, int? excludingAppointmentId = null) {
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(
                @"SELECT COUNT(*) FROM appointment
                  WHERE userId=@uid
                  AND (@newStart < `end` AND @newEnd > `start`)
                 " + (excludingAppointmentId.HasValue ? "AND appointmentId <> @exclude" : ""), conn)) {
                cmd.Parameters.AddWithValue("uid", userId);
                cmd.Parameters.AddWithValue("@newStart", newStartUtc);
                cmd.Parameters.AddWithValue("@newEnd", newEndUtc);
                if (excludingAppointmentId.HasValue) { cmd.Parameters.AddWithValue("@exclude", excludingAppointmentId.Value); }
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
    }
}
