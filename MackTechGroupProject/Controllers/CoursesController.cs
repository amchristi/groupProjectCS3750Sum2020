using MackTechGroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace MackTechGroupProject.Controllers
{
    public class CoursesController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();

        //Add Course
        public ActionResult AddCourse()
        {
            return View();
        }

        // POST: /Courses/AddCourse
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddCourse(Course model)
        {
            String userId = User.Identity.GetUserId();

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var currentInstructor = context.Users.Where(x => x.Id == userId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    CourseID = model.CourseID,
                    CRN = model.CRN,
                    Department = model.Department,
                    CourseNumber = model.CourseNumber,
                    CourseName = model.CourseName,
                    Instructor = currentInstructor,
                    CreditHours = model.CreditHours,
                    ClassLocation = model.ClassLocation,
                    MaxCapacity = model.MaxCapacity
                };

                context.Courses.Add(course);
                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult CourseRegistration()
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            return View(context.Courses.ToList());
        }

        // GET: Courses
        public ActionResult CS3620()
        {
            return View();
        }

        public ActionResult CS3750()
        {
            return View();
        }

        public ActionResult MATH1220()
        {
            return View();
        }

        public ActionResult PHYS2120()
        {
            return View();
        }

        //**OTHER SECTIONS OF COURSES**

        public ActionResult Grades()
        {
            return View();
        }
    }
}