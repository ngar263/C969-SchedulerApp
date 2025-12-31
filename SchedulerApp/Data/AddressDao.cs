using MySql.Data.MySqlClient;
using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Data {
    public class AddressDao {
        public int CreateAddress(Address address, string username, MySqlConnection conn, MySqlTransaction trans) {
            using var cmd = new MySqlCommand(
                @"INSERT INTO address
                  (address, address2, cityId, postalCode, phone,
                   createDate, createdBy, lastUpdate, lastUpdateBy)
                  VALUES
                  (@address1, @address2, @cityId, @postalCode, @phone,
                   NOW(), @user, NOW(), @user);
                  SELECT LAST_INSERT_ID();",
                conn, trans);

            cmd.Parameters.AddWithValue("@address1", address.Address1.Trim());
            cmd.Parameters.AddWithValue("@address2", address.Address2?.Trim() ?? "");
            cmd.Parameters.AddWithValue("@cityId", address.CityId);
            cmd.Parameters.AddWithValue("@postalCode", address.PostalCode.Trim());
            cmd.Parameters.AddWithValue("@phone", address.Phone.Trim());
            cmd.Parameters.AddWithValue("@user", username);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
