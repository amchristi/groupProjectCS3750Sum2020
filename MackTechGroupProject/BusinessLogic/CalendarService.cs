using MackTechGroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MackTechGroupProject.BusinessLogic
{
    public class CalendarService
    {

        public static Boolean CheckCalendarService(string userID, ApplicationDbContext context)
            {

                bool hasCalendar = context.Users.Any(x => x.Id == userID);

                if (hasCalendar)
                {
                    return true;
                }
                else
                {
                    return false;
                }

        }

    }

 }
