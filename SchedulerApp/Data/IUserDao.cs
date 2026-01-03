using SchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerApp.Data
{
    public interface IUserDao
    {
        User GetByUsername(string username);
        void CreateTestUser();
    }
}
