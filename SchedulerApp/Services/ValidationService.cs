using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SchedulerApp.Services
{
    public static class ValidationService
    {
        public static void ValidateCustomerInput(string name, string address, string phone, string cityName, string countryName) {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Customer name is required.");
            if (string.IsNullOrEmpty(address)) throw new ArgumentException("Address is required.");
            if (string.IsNullOrEmpty(phone)) throw new ArgumentException("Phone is required.");
            if (string.IsNullOrEmpty(cityName)) throw new ArgumentException("City is required.");
            if (string.IsNullOrEmpty(countryName)) throw new ArgumentException("Country is required.");

            phone = phone.Trim();
            if (!Regex.IsMatch(phone, @"^[0-9\-]+$")) throw new ArgumentException("Phone may contain only digits and dashes.");
        }

        public static void ValidateAppointmentTimes(DateTime startLocal, DateTime endLocal) {
            if (endLocal <= startLocal) throw new ArgumentException("End time must be after start time");
        }

        public static bool ValidateAppointmentPhoneNumber(string phone) {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(phone.Trim(), @"^[0-9-]+$");
        }
    }
}
