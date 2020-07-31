using MackTechGroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MackTechGroupProject.BusinessLogic
{
    public class CalendarService
    {

        public static Boolean ChecklCalendarService(string userID, Assignment assignment, ApplicationDbContext context)
            {

                bool hasCalendar = context.Users.Any(x => x.Id == userID);

                if (hasCalendar)
                {
                    context.Assignments.Add(assignment);

                    //save changes to database
                    context.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }

        }

    }

 }
