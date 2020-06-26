using MackTechGroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;

namespace MackTechGroupProject.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            String userId = User.Identity.GetUserId();

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var currentUser = context.Users.Where(x => x.Id == userId).FirstOrDefault();

            var currentEnrollments = context.Enrollments.Where(x => x.Student.Id == userId).Include(x => x.Student).Include(x => x.Course).ToList();

            return View(currentEnrollments);
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