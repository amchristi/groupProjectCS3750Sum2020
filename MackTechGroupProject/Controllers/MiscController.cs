using MackTechGroupProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MackTechGroupProject.Controllers
{
    public class MiscController : BaseController
    {
        // GET: Misc
        public ActionResult Registration()
        {
            return View();
        }
        public ActionResult Message()
        {
            return View();
        }
        public ActionResult Calendar()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            using(var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>())
            {
                var userId = User.Identity.GetUserId();
                List<CalendarEventViewModel> events = new List<CalendarEventViewModel>();

                if (this.User.IsInRole("Student")) {
                    // query enrollments for a list of all enrollments and include assignments
                    var currentEnrollmentsWithAssignments = context.Enrollments.Where(x => x.User.Id == userId).Include(x => x.User).Include(x => x.Course).Include("Course.Assignments").ToList();

                    // get allAssignments in a list to pass to AllAssignmentsViewModel
                    var allAssignments = currentEnrollmentsWithAssignments.Select(x => x.Course).SelectMany(y => y.Assignments).ToList();

                    foreach (Assignment a in allAssignments)
                    {
                        var relevantInfo = new CalendarEventViewModel
                        {
                            AssignmentTitle = a.AssignmentTitle,
                            AssignmentDescription = a.AssignmentDescription,
                            DueDate = a.DueDate,
                            Department = a.Course.Department,
                            CourseNumber = a.Course.CourseNumber
                        };
                        events.Add(relevantInfo);
                    }
                }
                if(this.User.IsInRole("Instructor"))
                {
                    var currentCoursesWithAssignments = context.Courses.Where(x => x.Instructor.Id == userId).Include("Assignments").ToList();
                    var allAssignments = currentCoursesWithAssignments.SelectMany(y => y.Assignments).ToList();


                    foreach (Assignment a in allAssignments)
                    {
                        var relevantInfo = new CalendarEventViewModel
                        {
                            AssignmentTitle = a.AssignmentTitle,
                            AssignmentDescription = a.AssignmentDescription,
                            DueDate = a.DueDate,
                            Department = a.Course.Department,
                            CourseNumber = a.Course.CourseNumber
                        };
                        events.Add(relevantInfo);
                    }


                }

                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public ActionResult NewEvent()
        {
            return View();
        }

        public ActionResult Account()
        {
            return View();
        }

        public ActionResult BillPay()
        {
            return View();
        }

    }
}