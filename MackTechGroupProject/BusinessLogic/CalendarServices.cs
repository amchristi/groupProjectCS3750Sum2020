using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MackTechGroupProject.Controllers;
using MackTechGroupProject.Models;

namespace MackTechGroupProject.BusinessLogic
{
    public class CalendarServices
    {
        public Boolean testCalendar(string userId, int selectedCourseId, List<CalendarEventViewModel> currentEnrollments, ApplicationDbContext context)
        {
            if(userId != null && context != null)
            {
                return true;
            }

            return false;
        }
    }
}