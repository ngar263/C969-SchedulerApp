using SchedulerApp.Models;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly User _currentUser;
        public MainWindow(User user) {
            InitializeComponent();
            _currentUser = user;
            Title = $"Scheduler - Logged in as {_currentUser.UserName}";
        }

        private void Customers_Click(object sender, RoutedEventArgs e) {
            var window = new CustomersWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        private void Appointments_Click(object sender, RoutedEventArgs e) {
            var window = new AppointmentsWindow(_currentUser);
            window.Owner = this;
            window.ShowDialog();
        }

        private void Calendar_Click(object sender, RoutedEventArgs e) {
            var window = new CalendarWindow(_currentUser);
            window.Owner = this;
            window.ShowDialog();
        }

        private void Reports_Click(object sender, RoutedEventArgs e) {
            var window = new ReportsWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        private void Lgoout_Click(object sender, RoutedEventArgs e) {
            var window = new LoginWindow();
            window.Show();
            this.Close();
        }
    }
}
