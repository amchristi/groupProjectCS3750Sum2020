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

            var currentEnrollmentsWithAssignments = context.Enrollments.Where(x => x.User.Id == userId).Include(x => x.User).Include(x => x.Course).Include("Course.Assignments").ToList();
            var allAssignments = currentEnrollmentsWithAssignments.Select(x => x.Course).SelectMany(y => y.Assignments).ToList();
            var allRelevantAssignments = allAssignments.Where(x => x.DueDate > DateTime.Now.Date).ToList();
            var allAssignmentsSorted = allRelevantAssignments.OrderBy(x => x.DueDate).ToList();

            var toDoListViewModel = new ToDoListViewModel()
            {
                currentAssignmentsView = allAssignmentsSorted,
                currentEnrollmentsView = currentEnrollmentsWithAssignments
            };

            return View(toDoListViewModel);
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