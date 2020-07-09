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
    public class AssignmentsController : BaseController
    {
        // GET: Assignments
        public ActionResult Assignment1()
        {
            return View();
        }
        public ActionResult Assignment2()
        {
            return View();
        }
        public ActionResult Assignment3()
        {
            return View();
        }
        public ActionResult Discussion()
        {
            return View();
        }

        public ActionResult SelectCourse()
        {
            String userId = User.Identity.GetUserId();

            var currentInstructorEnrollments = currentEnrollments.Select(c => c.Course).ToList();

            return View(currentInstructorEnrollments);
        }

        public ActionResult AddAssignment()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddAssignment(int id, Assignment model)
        {
            var selectedCourseId = id;

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var selectedCourse = context.Courses.Where(x => x.CourseId == selectedCourseId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var assignment = new Assignment
                {
                    AssignmentId = model.AssignmentId,
                    Course = selectedCourse,
                    Points = model.Points,
                    AssignmentTitle = model.AssignmentTitle,
                    AssignmentDescription = model.AssignmentDescription,
                    DueDate = model.DueDate,
                    SubmissionType = model.SubmissionType
                };

                context.Assignments.Add(assignment);
                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ViewAssignments(int id)
        {
            var selectedCourseId = id;
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var selectedCourse = context.Courses.Where(x => x.CourseId == selectedCourseId).Include(x => x.Assignments).FirstOrDefault();
            var currentAssignments = context.Assignments.Where(x => x.Course.CourseId == selectedCourseId).ToList();

            return View(currentAssignments);
        }

        public ActionResult ViewAllAssignments()
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var userId = User.Identity.GetUserId();

            // query enrollments for a list of all enrollments and include assignments
            var currentEnrollmentsWithAssignments = context.Enrollments.Where(x => x.User.Id == userId).Include(x => x.User).Include(x => x.Course).Include("Course.Assignments").ToList();
            
            // get allAssignments in a list to pass to AllAssignmentsViewModel
            var allAssignments = currentEnrollmentsWithAssignments.Select(x => x.Course).SelectMany(y => y.Assignments).ToList();

            // set ViewModel list to defined list above
            var allAssignmentsViewModel = new AllAssignmentsViewModel()
            {
                AllAssignments = allAssignments
            };

            return View(allAssignmentsViewModel);
        }

        public ActionResult AssignmentSubmission(int assignmentId)
        {
            
            var selectedAssignmentId = assignmentId;
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var currentAssignment = context.Assignments.Where(x => x.AssignmentId == selectedAssignmentId).FirstOrDefault();

            var submitAssignmentModel = new SubmitAssignmentModel()
            {
                currentAssignment = currentAssignment,
                SubmssionText = ""
                
            };

            return View(submitAssignmentModel);
        }

        //private IEnumerable<SelectListItem> GetCourses()
        //{
        //    String userId = User.Identity.GetUserId();

        //    var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

        //    var currentInstructorEnrollments = context.Enrollments.Include(x => x.User).Include(c => c.Course).Where(s => s.User.Id == userId).ToList();

        //    var instructorCourses = currentEnrollments.Select(x => x.Course).Select(x => new SelectListItem
        //                                                                                        {
        //                                                                                            Value = x.CourseId.ToString(),
        //                                                                                            Text = x.CourseName
        //                                                                                        });

        //    return new SelectList(instructorCourses, "Value", "Text");
        //}
    }
}