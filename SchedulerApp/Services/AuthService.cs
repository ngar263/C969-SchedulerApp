using SchedulerApp.Data;
using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Services
{
    public class AuthService
    {
        private readonly IUserDao _userDao;
        private readonly string _loginHistoryFile;

        public AuthService(IUserDao userDao, string loginHistoryFile = "Login_History.txt") {
            _userDao = userDao;
            _loginHistoryFile = loginHistoryFile;
        }

        public User Authenticate(string username, string password) {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return null;

            var user = _userDao.GetByUsername(username);
            if (user == null) return null;
            if (user.Password != password) return null;
            LogLogin(username, success: true);
            return user;
        }

        public async void CheckForTestUserAsync() {
            var user = _userDao.GetByUsername("test");
            if (user == null) _userDao.CreateTestUser();
            return;
        }

        private void LogLogin(string username, bool success) {
            try {
                var line = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC | {username} | Success={success}{Environment.NewLine}";
                File.AppendAllText(_loginHistoryFile, line);
            } catch {

            }
        }
    }
}
