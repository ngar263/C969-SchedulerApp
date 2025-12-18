using SchedulerApp.Data;
using SchedulerApp.Services;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using SchedulerApp.Models;
using SchedulerApp.Resources;
using System.Windows.Controls;

namespace SchedulerApp.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;
        private readonly AppointmentService _appointmentService;
        public LoginWindow()
        {
            InitializeComponent();
            LoadLocalizedText();

            var userDao = new UserDao();
            _authService = new AuthService(userDao);

            var appointmentDao = new AppointmentDao();
            _appointmentService = new AppointmentService(appointmentDao);          
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e) {
            txtStatus.Text = "";
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
                txtStatus.Text = SchedulerApp.Resources.Resources.Error_EmptyFields.Trim();
                return;
            }

            var user = _authService.Authenticate(username, password);

            if (user == null) {
                txtStatus.Text = SchedulerApp.Resources.Resources.Error_BadCredentials;
                return;
            }

            if (user != null) {
                var upcoming = _appointmentService.GetUpcomingAppointmentsWithinMinutes(user.UserId, 15);
                if (upcoming != null && upcoming.Count > 0) {
                    var appointment = upcoming[0];
                    var startLocal = TimeHelper.ConvertUtcToLocal(appointment.StartUtc);
                    MessageBox.Show($"{SchedulerApp.Resources.Resources.Alert_AppointmentIn15.Trim()}\n{appointment.Title} at {startLocal}", "Appointment Alert");
                }
            }

            var main = new MainWindow(user!);
            main.Show();
            this.Close();
        }

        public void SetLanguage(string cultureCode) {
            var culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            LoadLocalizedText();
        }

        private void LoadLocalizedText() {
            if (lblUsername == null) return;
            lblUsername.Content = SchedulerApp.Resources.Resources.Login_Username.Trim();
            lblPassword.Content = SchedulerApp.Resources.Resources.Login_Password.Trim();
            btnLogin.Content = SchedulerApp.Resources.Resources.Login_Button.Trim();
            txtStatus.Text = $"{SchedulerApp.Resources.Resources.Timezone.Trim()}: {TimeZoneInfo.Local.StandardName.Trim()}" ?? "";
        }

        private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var language = (ComboBoxItem)LanguageSelector.SelectedItem;
            string cultureCode = language.Tag.ToString()!;
            SetLanguage(cultureCode);
        }
    }
}
