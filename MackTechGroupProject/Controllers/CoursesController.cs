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
    public class CoursesController : BaseController
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
            String userId = User.Identity.GetUserId();

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var currentInstructor = context.Users.Where(x => x.Id == userId).FirstOrDefault();

            string instructorName = currentInstructor.FirstName + " " + currentInstructor.LastName;

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
                    InstructorName = instructorName,
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

            var allCourses = context.Courses.ToList();

            return View(allCourses);
        }

        public ActionResult RegisterForCourse(int id)
        {
            String userId = User.Identity.GetUserId();
            var selectedCourseId = id;

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var currentStudent = context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var selectedCourse = context.Courses.Where(x => x.CourseID == selectedCourseId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var studentEnrollment = new Enrollment
                {
                    Course = selectedCourse,
                    Student = currentStudent
                };

                var currentEnrollments = context.Enrollments.Include(x => x.Student).Include(c => c.Course).Where(s => s.Student.Id == userId).ToList();

                //check to see if the course already exists in student's currentEnrollments
                bool hasCourse = currentEnrollments.Any(x => x.Course.CourseID == id);

                if (!hasCourse)
                {
                    context.Enrollments.Add(studentEnrollment);
                    context.SaveChanges();

                    return RedirectToAction("StudentAccount", "Courses");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error: You are already registered for this course";
                    return RedirectToAction("CourseRegistration", "Courses");
                }
            }

            //don't want to get this far
            return RedirectToAction("CourseRegistration", "Courses");
        }

        public ActionResult StudentAccount()
        {
            String userId = User.Identity.GetUserId();

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            // gets a list of enrollments for current student
            var currentEnrollments = context.Enrollments.Include(x => x.Student).Include(c => c.Course).Where(s => s.Student.Id == userId).ToList();

            var totalCreditHours = currentEnrollments.Sum(x => x.Course.CreditHours);
            var totalCost = totalCreditHours * 240;
            var formattedCost = totalCost.ToString("C0");

            ViewBag.TotalCreditHours = totalCreditHours;
            ViewBag.TotalFormattedCost = formattedCost;

            return View(currentEnrollments);
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