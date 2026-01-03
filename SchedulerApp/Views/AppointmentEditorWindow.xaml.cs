using SchedulerApp.Data;
using SchedulerApp.Models;
using SchedulerApp.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SchedulerApp.Views {
    /// <summary>
    /// Interaction logic for AppointmentEditorWindow.xaml
    /// </summary>
    public partial class AppointmentEditorWindow : Window {
        public Appointment Appointment { get; set; }
        private readonly User _user;
        private readonly List<Customer> _customers;
        public AppointmentEditorWindow(User user, List<Customer> customers, Appointment appointment = null!) {
            InitializeComponent();
            _user = user;
            _customers = customers;
            cmbCustomer.ItemsSource = _customers;
            if (appointment == null) {
                Appointment = new Appointment { UserId = _user.UserId };
                cmbCustomer.SelectedIndex = -1;
                var selectedDate = DateTime.Now.Date;
                dpDate.SelectedDate = selectedDate;
                var eastern = TimeHelper.GetEasternTimeZone();
                var estStart = DateTime.SpecifyKind(selectedDate.AddHours(9), DateTimeKind.Unspecified);
                var estEnd = DateTime.SpecifyKind(selectedDate.AddHours(10), DateTimeKind.Unspecified);
                var localStart = TimeZoneInfo.ConvertTime(estStart, eastern, TimeZoneInfo.Local);
                var localEnd = TimeZoneInfo.ConvertTime(estEnd, eastern, TimeZoneInfo.Local);
                txtStartTime.Text = localStart.ToString("hh:mm");
                txtEndTime.Text = localEnd.ToString("hh:mm");
            } else {
                Appointment = appointment;
                var startLocal = TimeHelper.ConvertUtcToLocal(appointment.StartUtc);
                var endLocal = TimeHelper.ConvertUtcToLocal(appointment.EndUtc);
                cmbStartAmPm.SelectedIndex = startLocal.Hour >= 12 ? 1 : 0;
                cmbEndAmPm.SelectedIndex = startLocal.Hour >= 12 ? 1 : 0;
                dpDate.SelectedDate = startLocal.Date;
                txtStartTime.Text = startLocal.ToString("hh:mm");
                txtEndTime.Text = endLocal.ToString("hh:mm");
                txtTitle.Text = appointment.Title;
                txtDescription.Text = appointment.Description;
                txtLocation.Text = appointment.Location;
                txtContact.Text = appointment.Contact;
                txtType.Text = appointment.Type;
                cmbCustomer.SelectedItem = _customers.FirstOrDefault(c => c.CustomerId == appointment.CustomerId);
            }
        }

        bool TryParseTime(string text, ComboBox ampmBox, out TimeSpan time) {
            time = default;

            if (!TimeSpan.TryParseExact(text, "hh\\:mm", CultureInfo.InvariantCulture, out time) &&
                !TimeSpan.TryParse(text, out time))
                return false;

            var ampm = ((ComboBoxItem)ampmBox.SelectedItem).Content.ToString();

            if (ampm == "PM" && time.Hours < 12)
                time = time.Add(TimeSpan.FromHours(12));
            else if (ampm == "AM" && time.Hours == 12)
                time = time.Subtract(TimeSpan.FromHours(12));

            return true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            if (!dpDate.SelectedDate.HasValue) { MessageBox.Show("Select a date."); return; }
            if (!TryParseTime(txtStartTime.Text, cmbStartAmPm, out var startTs)) {
                MessageBox.Show("Invalid start time.");
                return;
            }

            if (!TryParseTime(txtEndTime.Text, cmbEndAmPm, out var endTs)) {
                MessageBox.Show("Invalid end time.");
                return;
            }

            if (endTs <= startTs) {
                MessageBox.Show("End time must be after start time.");
                return;
            }

            if (cmbCustomer.SelectedItem is not Customer selectedCustomer) {
                MessageBox.Show("Please select a customer.");
                return;
            }

            var date = dpDate.SelectedDate.Value.Date;
            var startLocal = date.Add(startTs);
            var endLocal = date.Add(endTs);
            var phoneNumber = txtContact.Text.Trim();
            if (!string.IsNullOrEmpty(phoneNumber)) {
                if (!ValidationService.ValidateAppointmentPhoneNumber(phoneNumber)) {
                    MessageBox.Show("Phone number may contain only numbers and dashes.");
                    return;
                }
            }

            Appointment.CustomerId = selectedCustomer.CustomerId;
            Appointment.UserId = _user.UserId;
            Appointment.Title = txtTitle.Text.Trim() == string.Empty ? null : txtTitle.Text.Trim();
            Appointment.Description = txtDescription.Text.Trim() == string.Empty ? null : txtDescription.Text.Trim();
            Appointment.Type = txtType.Text.Trim() == string.Empty ? null : txtType.Text.Trim();
            Appointment.Contact = phoneNumber;
            Appointment.Location = txtLocation.Text.Trim() == string.Empty ? null : txtLocation.Text.Trim();
            Appointment.StartUtc = TimeZoneInfo.ConvertTimeToUtc(startLocal);
            Appointment.EndUtc = TimeZoneInfo.ConvertTimeToUtc(endLocal);

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
            this.Close();
        }
    }
}
