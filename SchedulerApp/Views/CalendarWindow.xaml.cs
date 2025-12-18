using K4os.Compression.LZ4.Encoders;
using SchedulerApp.Data;
using SchedulerApp.Models;
using SchedulerApp.Services;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for CalendarWindow.xaml
    /// </summary>
    public partial class CalendarWindow : Window {
        private readonly AppointmentService _appointmentService;
        private readonly User _user;
        public CalendarWindow(User user) {
            InitializeComponent();
            _user = user;
            _appointmentService = new AppointmentService(new AppointmentDao());
        }

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e) {
            var date = calendar.SelectedDate ?? DateTime.Now.Date;
            var appointments = _appointmentService.GetAppointmentsForUserDay(_user.UserId, date);
            lbAppointments.Items.Clear();
            foreach (var appointment in appointments) {
                var startLocal = TimeHelper.ConvertUtcToLocal(appointment.StartUtc);
                lbAppointments.Items.Add($"{startLocal:yyyy-MM-dd HH:mm} - {appointment.Title} (Customer {appointment.CustomerId})");
            }
        }
    }
}
