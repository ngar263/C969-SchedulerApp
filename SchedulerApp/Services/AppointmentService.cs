using SchedulerApp.Data;
using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Services
{
    public class AppointmentService
    {
        private readonly IAppointmentDao _appointmentDao;
        public AppointmentService(IAppointmentDao appointmentDao) => _appointmentDao = appointmentDao;

        public int AddAppointment(Appointment appointment, User user) {
            var startLocal = TimeHelper.ConvertUtcToLocal(appointment.StartUtc);
            var endLocal = TimeHelper.ConvertUtcToLocal(appointment.EndUtc);
            ValidationService.ValidateAppointmentTimes(startLocal, endLocal);

            var startEastern = TimeHelper.ConvertLocalToEastern(startLocal);
            var endEastern = TimeHelper.ConvertLocalToEastern(endLocal);

            if (startEastern.DayOfWeek == DayOfWeek.Sunday || startEastern.DayOfWeek == DayOfWeek.Saturday ||
                endEastern.DayOfWeek == DayOfWeek.Sunday || endEastern.DayOfWeek == DayOfWeek.Sunday)
                throw new InvalidOperationException("Appointments must be scheduled Monday through Friday EST.");

            var businessStart = TimeSpan.FromHours(9);
            var businessEnd = TimeSpan.FromHours(17);

            if (startLocal.TimeOfDay < businessStart || endLocal.TimeOfDay > businessEnd)
                throw new InvalidOperationException("Appointment must be scheduled during business hours."); 

            var startUtc = appointment.StartUtc.Kind == DateTimeKind.Utc ? appointment.StartUtc : TimeHelper.ConvertLocalToUtc(appointment.StartUtc);
            var endUtc = appointment.EndUtc.Kind == DateTimeKind.Utc ? appointment.EndUtc : TimeHelper.ConvertLocalToUtc(appointment.EndUtc);

            if (_appointmentDao.HasOverlappingAppointment(appointment.UserId, startUtc, endUtc, appointment.AppointmentId == 0 ? null : (int?)appointment.AppointmentId))
                throw new InvalidOperationException("This appointment overlaps with an already existing appointment.");

            appointment.StartUtc = startUtc;
            appointment.EndUtc = endUtc;

            try {
                return _appointmentDao.AddAppointment(appointment, user.Username);
            } catch (Exception ex) {
                throw new ApplicationException("Failed to add appointment", ex);
            }
        }

        public void UpdateAppointment(Appointment appointment, User user) {
            var startLocal = TimeHelper.ConvertUtcToLocal(appointment.StartUtc);
            var endLocal = TimeHelper.ConvertUtcToLocal(appointment.EndUtc);
            ValidationService.ValidateAppointmentTimes(startLocal, endLocal);
            var startEastern = TimeHelper.ConvertLocalToEastern(startLocal);
            var endEastern = TimeHelper.ConvertLocalToEastern(endLocal);
            if (startEastern.DayOfWeek == DayOfWeek.Sunday || startEastern.DayOfWeek == DayOfWeek.Saturday ||
                endEastern.DayOfWeek == DayOfWeek.Sunday || endEastern.DayOfWeek == DayOfWeek.Saturday) {
                throw new InvalidOperationException("Appointments must be scheduled Monday through Friday EST.");
            }
            var businessStart = TimeSpan.FromHours(9);
            var businessEnd = TimeSpan.FromHours(17);
            if (startEastern.TimeOfDay < businessStart || endEastern.TimeOfDay > businessEnd) {
                throw new InvalidOperationException("Appointments must be scheduled between 9:00 a.m.and 5:00 p.m. Eastern Time.");
            }

            var startUtc = TimeHelper.ConvertLocalToUtc(startLocal);
            var endUtc = TimeHelper.ConvertLocalToUtc(endLocal);

            if (_appointmentDao.HasOverlappingAppointment(appointment.UserId, startUtc, endUtc, appointment.AppointmentId)) {
                throw new InvalidOperationException("This appointment overlaps with an already existing appointment.");
            }

            appointment.StartUtc = startUtc;
            appointment.EndUtc = endUtc;

            try {
                _appointmentDao.UpdateAppointment(appointment, user.Username);
            } catch (Exception ex) {
                throw new ApplicationException("Failed to update appointment", ex);
            }
        }

        public void DeleteAppointment(int appointmentId) {
            try {
                _appointmentDao.DeleteAppointment(appointmentId);
            } catch (Exception ex) {
                throw new ApplicationException("Failed to delete appointment.", ex);
            }
        }

        public List<Appointment> GetAllAppointments() => _appointmentDao.GetAllAppointments();

        public List<Appointment> GetAppointmentsForUserDay(int userId, DateTime localDate) {
            var startLocal = localDate.Date;
            var endLocal = startLocal.AddDays(1);
            
            var startUtc = TimeHelper.ConvertLocalToUtc(startLocal);
            var endUtc = TimeHelper.ConvertLocalToUtc(endLocal);

            return _appointmentDao.GetAppointmentsForUserInRange(userId, startUtc, endUtc);
        } 

        public List<Appointment> GetUpcomingAppointmentsWithinMinutes(int userId, int minutes) {
            var nowUtc = DateTime.UtcNow;
            var futureUtc = nowUtc.AddMinutes(minutes);

            return _appointmentDao.GetAppointmentsForUserBetween(userId, nowUtc, futureUtc);
        }

        public IEnumerable<object> GetAppointmentTypesByMonthReport() {
            var appointments = GetAllAppointments();

            var q = appointments
                .GroupBy(a => new { Month = TimeHelper.ConvertUtcToLocal(a.StartUtc).Month, a.Type })
                .Select(g => new { Month = g.Key.Month, Type = g.Key.Type, Count = g.Count() })
                .OrderBy(x => x.Month)
                .ThenBy(x => x.Type);
            return q;
        }

        public IEnumerable<object> GetSchedulePerUserReport(IEnumerable<int> userIds) {
            var appointments = GetAllAppointments();
            var q = userIds.Select(uid => new {
                UserId = uid,
                Appointments = appointments.Where(a => a.UserId == uid).OrderBy(a => a.StartUtc).Select(a => new {
                    a.AppointmentId,
                    a.CustomerId,
                    a.CustomerName,
                    StartLocal = TimeHelper.ConvertUtcToLocal(a.StartUtc),
                    EndLocal = TimeHelper.ConvertUtcToLocal(a.EndUtc),
                    a.Type,
                    a.Title
                }).ToList()
            }).ToList();
            return q;
        }

        public IEnumerable<object> GetAppointmentsPerCustomerReport() {
            var appointments = GetAllAppointments();
            var q = appointments
                .GroupBy(a => new { a.CustomerId, a.CustomerName })
                .Select(g => new {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    Count = g.Count(),
                    NextAppointmentUtc = g
                        .Where(a => a.StartUtc > DateTime.UtcNow)
                        .OrderBy(a => a.StartUtc)
                        .Select(a => a.StartUtc)
                        .FirstOrDefault()
                });
            return q;
        }
    }
}
