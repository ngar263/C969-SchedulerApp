using SchedulerApp.Data;
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
    /// Interaction logic for ReportsWindow.xaml
    /// </summary>
    public partial class ReportsWindow : Window {
        private readonly AppointmentService _appointmentService;
        public ReportsWindow() {
            InitializeComponent();
            _appointmentService = new AppointmentService(new AppointmentDao());
            LoadReports();
        }
        private void LoadReports() {
            lbTypesByMonth.ItemsSource = _appointmentService.GetAppointmentTypesByMonthReport()
                .Select(x => $"Month: {((dynamic)x).Month}, Type: {((dynamic)x).Type ?? "None specified"}, Count: {((dynamic)x).Count}");
            var allAppointments = _appointmentService.GetAllAppointments();
            var userIds = allAppointments.Select(x =>  x.UserId).Distinct().ToList();
            var schedule = _appointmentService.GetSchedulePerUserReport(userIds);
            lbSchedulePerUser.ItemsSource = schedule.Select(u => $"User {((dynamic)u).UserName} has {((dynamic)u).Appointments.Count} appointments.");
            lbPerCustomer.ItemsSource = _appointmentService.GetAppointmentsPerCustomerReport()
                .Select(x => $"Customer {((dynamic)x).CustomerName}, Count: {((dynamic)x).Count}, Next: {((((dynamic)x).NextAppointmentUtc != default(DateTime)) ? TimeHelper.ConvertUtcToLocal(((dynamic)x).NextAppointmentUtc).ToString("g") : "N/A")}");
        }
    }
}
