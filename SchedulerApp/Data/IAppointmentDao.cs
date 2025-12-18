using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Data
{
    public interface IAppointmentDao
    {
        int AddAppointment(Appointment apt);
        void UpdateAppointment(Appointment apt);
        void DeleteAppointment(int appointmentId);
        List<Appointment> GetAppointmentsForUserBetween(int userId, DateTime startUtc, DateTime endUtc);
        bool HasOverlappingAppointment(int userId, DateTime newStartUtc, DateTime newEndUtc, int? excludingAppointmentId = null);
        List<Appointment> GetAppointmentsForUserInRange(int userId, DateTime fromUtc, DateTime toUtc);
        List<Appointment> GetAllAppointments();
    }
}
