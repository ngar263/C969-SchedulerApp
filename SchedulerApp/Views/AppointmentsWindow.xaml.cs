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
    /// Interaction logic for AppointmentsWindow.xaml
    /// </summary>
    public partial class AppointmentsWindow : Window {
        private readonly AppointmentService _appointmentService;
        private readonly IAppointmentDao _appointmentDao;
        private readonly User _currentUser;
        public AppointmentsWindow(User user) {
            InitializeComponent();
            _currentUser = user;
            _appointmentDao = new AppointmentDao();
            _appointmentService = new AppointmentService(_appointmentDao);
            LoadAppointments();
        }

        private void LoadAppointments() {
            dgAppointments.ItemsSource = _appointmentService.GetAllAppointments();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            var editor = new AppointmentEditorWindow(_currentUser);
            if (editor.ShowDialog() == true) {
                try {
                    _appointmentService.AddAppointment(editor.Appointment);
                    LoadAppointments();
                } catch (Exception ex) {
                    MessageBox.Show("Error adding appointment." + ex.Message);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            if (dgAppointments.SelectedItem is Appointment appointment) {
                var editor = new AppointmentEditorWindow(_currentUser, appointment);
                if (editor.ShowDialog() == true) {
                    try {
                        _appointmentService.UpdateAppointment(appointment);
                        LoadAppointments();
                    } catch (Exception ex) {
                        MessageBox.Show("Error updating appointment." + ex.Message);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            if (dgAppointments.SelectedItem is Appointment appointment) {
                if (MessageBox.Show("Delete selected appointment?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                    try {
                        _appointmentDao.DeleteAppointment(appointment.AppointmentId);
                        LoadAppointments();
                    } catch (Exception ex) {
                        MessageBox.Show("Error deleting appointment." + ex.Message);
                    }
                }
            }
        }
    }
}
