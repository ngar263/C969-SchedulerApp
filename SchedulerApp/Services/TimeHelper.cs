using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Services
{
    public class TimeHelper
    {
        public static TimeZoneInfo GetEasternTimeZone() {
            try {
                return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }catch (TimeZoneNotFoundException exc) {
                try {
                    return TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
                }catch {
                    return TimeZoneInfo.Utc;
                }
            }
        }

        public static DateTime ConvertLocalToEastern(DateTime localDateTime) {
            var eastern = GetEasternTimeZone();
            return TimeZoneInfo.ConvertTime(localDateTime, eastern);
        }

        public static DateTime ConvertLocalToUtc(DateTime localDateTime) {
            if (localDateTime.Kind == DateTimeKind.Utc) return localDateTime;
            return localDateTime.ToUniversalTime();
        }

        public static DateTime ConvertUtcToLocal(DateTime utc) {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.Local);
        }
    }
}
