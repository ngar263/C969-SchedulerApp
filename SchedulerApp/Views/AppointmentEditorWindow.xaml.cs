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
        public AppointmentEditorWindow(User user, Appointment appointment = null!) {
            InitializeComponent();
            _user = user;

            if (appointment == null) {
                Appointment = new Appointment { UserId = _user.UserId };
                dpDate.SelectedDate = DateTime.Now.Date;
            } else {
                Appointment = appointment;
                dpDate.SelectedDate = TimeZoneInfo.ConvertTimeFromUtc(appointment.StartUtc, TimeZoneInfo.Local).Date;
                var startLocal = TimeZoneInfo.ConvertTimeFromUtc(appointment.StartUtc, TimeZoneInfo.Local);
                var endLocal = TimeZoneInfo.ConvertTimeFromUtc(appointment.EndUtc, TimeZoneInfo.Local);
                txtStartTime.Text = startLocal.ToString("HH:mm");
                txtEndTime.Text = endLocal.ToString("HH:mm");
                txtTitle.Text = appointment.Title;
                txtDescription.Text = appointment.Description;
                txtType.Text = appointment.Type;
                txtCustomerId.Text = appointment.CustomerId.ToString();
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            if (!dpDate.SelectedDate.HasValue) { MessageBox.Show("Select a date."); return; }
            if (!TimeSpan.TryParseExact(txtStartTime.Text, "hh\\:mm", CultureInfo.InvariantCulture, out var startTs) &&
                !TimeSpan.TryParse(txtStartTime.Text, out startTs)) {
                MessageBox.Show("Invalid start time.");
                return;
            }
            if (!TimeSpan.TryParseExact(txtEndTime.Text, "hh\\:mm", CultureInfo.InvariantCulture, out var endTs) &&
                !TimeSpan.TryParse(txtStartTime.Text, out endTs)) {
                MessageBox.Show("Invalid end time.");
                return;
            }

            if (!int.TryParse(txtCustomerId.Text, out var customerId)) { MessageBox.Show("Invalid customerId."); return; }

            var date = dpDate.SelectedDate.Value.Date;
            var startLocal = date.Add(startTs);
            var endLocal = date.Add(endTs);

            Appointment.CustomerId = customerId;
            Appointment.UserId = _user.UserId;
            Appointment.Title = txtTitle.Text.Trim();
            Appointment.Description = txtDescription.Text.Trim();
            Appointment.Type = txtType.Text.Trim();
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
