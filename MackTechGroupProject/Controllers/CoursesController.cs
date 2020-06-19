using MackTechGroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MackTechGroupProject.Controllers
{
    public class CoursesController : Controller
    {
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
            if (ModelState.IsValid)
            {
                //do stuff
                var course = new Course
                {
                    CourseID = model.CourseID,
                    CourseName = model.CourseName,
                    InstructorID = model.InstructorID,
                    CreditHours = model.CreditHours,
                    ClassLocation = model.ClassLocation,
                    MaxCapacity = model.MaxCapacity
                };

                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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