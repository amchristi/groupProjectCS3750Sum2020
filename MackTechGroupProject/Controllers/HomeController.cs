using MackTechGroupProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MackTechGroupProject.Controllers
{
    public class HomeController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var userId = User.Identity.GetUserId();

            //student - get all assignments from enrollments
            var currentEnrollmentsWithAssignments = context.Enrollments.Where(x => x.User.Id == userId).Include(x => x.User).Include(x => x.Course).Include("Course.Assignments").ToList();
            
            //student - get all upcoming assignments and sort them by due date
            var allAssignments = currentEnrollmentsWithAssignments.Select(x => x.Course).SelectMany(y => y.Assignments).ToList();
            var allRelevantAssignments = allAssignments.Where(x => x.DueDate > DateTime.Now.Date).ToList();
            var allAssignmentsSorted = allRelevantAssignments.OrderBy(x => x.DueDate).ToList();

            //instructor - get all courses
            var currentInstructor = context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var currentInstructorCourses = context.Courses.Where(x => x.Instructor.Id == currentInstructor.Id).ToList();

            //instructor - get all upcoming assignments from those courses and sort them by due date
            var allInstructorAssignments = currentInstructorCourses.SelectMany(y => y.Assignments).ToList();
            var allRelevantInstructorAssignments = allInstructorAssignments.Where(x => x.DueDate > DateTime.Now.Date).ToList();
            var allRelevantInstructorAssignmentsSorted = allRelevantInstructorAssignments.OrderBy(x => x.DueDate).ToList();

            var toDoListViewModel = new ToDoListViewModel()
            {
                currentAssignmentsView = allAssignmentsSorted,
                currentEnrollmentsView = currentEnrollmentsWithAssignments,
                currentCoursesView = currentInstructorCourses,
                currentInstructorAssignmentsView = allRelevantInstructorAssignmentsSorted
            };

            return View(toDoListViewModel);
        }

        public JsonResult GetNotifications()
        {
            var userID = User.Identity.GetUserId();
            var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;
            NotificationComponents NC = new NotificationComponents();
            var list = NC.GetNotificationData(notificationRegisterTime, userID);
            //update session here for get only newly graded assignments (notification)
            Session["Lastupdate"] = DateTime.Now;
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

  

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}