using MackTechGroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.IO;
using Newtonsoft.Json;
using MackTechGroupProject.BusinessLogic;
using Microsoft.Ajax.Utilities;

namespace MackTechGroupProject.Controllers
{
    public class AssignmentsController : BaseController
    {
        // GET: Assignments
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
            if (ModelState.IsValid)
            {
                var selectedCourseId = id;
                var Assignment = new Assignment();
                var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
                var selectedCourse = context.Courses.Where(x => x.CourseId == selectedCourseId).FirstOrDefault();

                var assignment = new Assignment
                {
                    AssignmentId = model.AssignmentId,
                    Course = selectedCourse,
                    Points = model.Points,
                    AssignmentTitle = model.AssignmentTitle,
                    AssignmentDescription = model.AssignmentDescription,
                    DueDate = model.DueDate.AddHours(23).AddMinutes(59),
                    SubmissionType = model.SubmissionType,
                    AssignmentAddedOn = DateTime.Now
                };

                Boolean result = AssignmentService.AddAssignmentService(selectedCourseId, assignment, context);

                if (result)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult DeleteAssignment(int id)
        {
            var userId = User.Identity.GetUserId();
            var selectedAssignmentId = id;

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            AssignmentService.DeleteAssignmentService(selectedAssignmentId, context);

            return RedirectToAction("Index", "Home");
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignmentSubmission(SubmitAssignmentModel model)
        {
            var userID = User.Identity.GetUserId();
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var selectedAssignmentId = Convert.ToInt64(Request.Form["asID"]);

            bool isUnitTest = false;

            Boolean result = AssignmentService.submitAssignmentService(userID, selectedAssignmentId, model, context, isUnitTest);

            return RedirectToAction("Index", "Home");
        }


        //New method passes a null assignment id to submission
        public ActionResult AssignmentSubmission(int assignmentId)
        {
            var selectedAssignmentId = assignmentId;
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var currentAssignment = context.Assignments.Where(x => x.AssignmentId == selectedAssignmentId).FirstOrDefault();

            var submitAssignmentModel = new SubmitAssignmentModel()
            {
                assignmentID = selectedAssignmentId,
                currentAssignment = currentAssignment,
                SubmissionText = ""
            };

            return View(submitAssignmentModel);
        }

        public ActionResult GradeAssignment(int id)
        {
            var selectedAssignmentId = id;
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //Select the specific assignment from assignments Table
            var selectedAssignment = context.Assignments.Where(x => x.AssignmentId == selectedAssignmentId).Include(x => x.Course).FirstOrDefault();

            //Get a list of submitted assignments from the SubmissionGrades Table based on the specific assignment
            //List<SubmissionGrades> submittedAssignments = context.SubmissionGrades.Where(x => x.Assignment == selectedAssignment).Include(x => x.User.Id).ToList();
            var allSubmissionGrades = context.SubmissionGrades.Include(x => x.Assignment).Include(x => x.User).Include(x => x.User).ToList();

            var allSubmissionsOfSelected = allSubmissionGrades.Where(x => x.Assignment == selectedAssignment).ToList();

            //set ViewModel list to defined list above
            var gradeSubmittedAssignmentsViewModel = new gradeSubmittedAssignmentsViewModel()
            {
                SubmittedAssignments = allSubmissionsOfSelected
            };

            return View(gradeSubmittedAssignmentsViewModel);
        }

        public ActionResult InstructorGradeBook(int id)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //Get instructor Id
            var instructorUserId = User.Identity.GetUserId();

            //Get course based on course Id
            var selectedCourseId = id;
            var selectedCourse = context.Courses.Where(x => x.CourseId == selectedCourseId).FirstOrDefault();


            //get list of student enrollments for class 
            var courseEnrollments = context.Enrollments.Where(x => x.Course.CourseId == selectedCourseId).Include(x => x.User).ToList();

            //get list of assignments for course
            var courseAssignments = context.Assignments.Where(x => x.Course.CourseId == selectedCourseId).ToList();


            //List of All student grades
            var studentGrades = context.SubmissionGrades.Include(x => x.User).Include(x => x.Assignment).Include("Assignment.Course")
                            .Where(x => x.Assignment.Course.CourseId == selectedCourseId).ToList();

            //filter the instructor out of the class roll
            var courseEnrollmentsStudents = courseEnrollments.Where(x => x.User.Id != instructorUserId).OrderBy(x => x.User.LastName).ToList();

            //get the total points for assignments for the selectedCourseId
            var courseAssignmentPointsTotal = courseAssignments.Where(x => x.Course.CourseId == selectedCourseId).Sum(x => x.Points);

            //get all userId's to use for foreach loop
            var userIds = courseEnrollmentsStudents.Select(x => x.User).Select(y => y.Id).ToList();

            // set percentages per user
            foreach (var userId in userIds)
            {
                // get user's grade based off userId
                var userGradesTotal = studentGrades.Where(x => x.User.Id == userId).Sum(x => x.Grade);

                // set Percentage of user using usergrade
                courseEnrollmentsStudents.Where(x => x.User.Id == userId).FirstOrDefault().User.Percentage = Convert.ToDecimal(userGradesTotal) / courseAssignmentPointsTotal;
            }

            var instructorGradeBookViewModel = new InstructorGradeBookViewModel()
            {
                ClassRoll = courseEnrollmentsStudents,
                CourseAssignments = courseAssignments,
                StudentGrades = studentGrades
            };

            return View(instructorGradeBookViewModel);
        }

        public ActionResult StudentsStatisticsInstructor(int id)
        {
            var selectedAssignmentId = id;
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //Select the specific assignment from assignments Table
            var selectedAssignment = context.Assignments.Where(x => x.AssignmentId == selectedAssignmentId).Include(x => x.Course).FirstOrDefault();

            //Get a list of submitted assignments from the SubmissionGrades Table based on the specific assignment
            var allSubmissionGrades = context.SubmissionGrades.Include(x => x.Assignment).Include(x => x.User).Include(x => x.User).ToList();

            var allSubmissionsOfSelected = allSubmissionGrades.Where(x => x.Assignment == selectedAssignment && x.Grade != null).ToList();

            if (allSubmissionsOfSelected.Count() != 0)
            {
                // used for calculating percentages
                var assignmentPointTotal = allSubmissionsOfSelected.FirstOrDefault().Assignment.Points;
                var userIds = allSubmissionsOfSelected.Select(x => x.User).Select(y => y.Id).ToList();

                // set percentages per user
                foreach (var userId in userIds)
                {
                    // get user's grade based off userId
                    var userGrade = allSubmissionsOfSelected.Where(x => x.User.Id == userId).FirstOrDefault().Grade;

                    // set Percentage of user using usergrade
                    allSubmissionsOfSelected.Where(x => x.User.Id == userId).FirstOrDefault().Percentage = Convert.ToDecimal(userGrade) / assignmentPointTotal;
                }

                // list of all scores using allSubmissionsOfSelected including count of student that received scored 0-59, 60-69, etc.
                var zeroToSixty = allSubmissionsOfSelected.Where(x => x.Percentage < Convert.ToDecimal(.6)).Count();
                var sixtyToSeventy = allSubmissionsOfSelected.Where(x => x.Percentage >= Convert.ToDecimal(.6) && x.Percentage < Convert.ToDecimal(.7)).Count();
                var seventyToEighty = allSubmissionsOfSelected.Where(x => x.Percentage >= Convert.ToDecimal(.7) && x.Percentage < Convert.ToDecimal(.8)).Count();
                var eightyToNinety = allSubmissionsOfSelected.Where(x => x.Percentage >= Convert.ToDecimal(.8) && x.Percentage < Convert.ToDecimal(.9)).Count();
                var ninetyToOneHundred = allSubmissionsOfSelected.Where(x => x.Percentage >= Convert.ToDecimal(.9)).Count();

                // save object for x and y values of chart/graph
                List<DataPoint> dataPoints = new List<DataPoint>
            {
                //(range, count of students within range)
                new DataPoint( "0-59%", zeroToSixty),
                new DataPoint( "60-69%", sixtyToSeventy),
                new DataPoint( "70-79%", seventyToEighty),
                new DataPoint( "80-89%", eightyToNinety),
                new DataPoint( "90-100%", ninetyToOneHundred)
            };

                // pass the data to canvasJS via Json
                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

                //set ViewModel list to defined list above
                var gradeSubmittedAssignmentsViewModel = new gradeSubmittedAssignmentsViewModel()
                {
                    SubmittedAssignments = allSubmissionsOfSelected
                };

                return View(gradeSubmittedAssignmentsViewModel);
            }

            else
            {
                TempData["ErrorMessage"] = "Error: Assignments not yet graded!";
                return RedirectToAction("GradeAssignment", new { id = selectedAssignmentId });
            }
        }

        public ActionResult ViewGrades(int id)
        {
            var userId = User.Identity.GetUserId();
            var selectedCourseId = id;

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            var courseAssignments = context.Assignments.Where(x => x.Course.CourseId == selectedCourseId).ToList();

            var studentGrades = context.SubmissionGrades.Include(x => x.User).Include(x => x.Assignment).Include("Assignment.Course")
                                        .Where(x => x.User.Id == userId && x.Assignment.Course.CourseId == selectedCourseId).ToList();

            var studentGradesViewModel = new StudentGradesViewModel()
            {
                StudentGrades = studentGrades,
                CourseAssignments = courseAssignments
            };

            //to calculate total get a sum off all score and divide by sum of all assignmnet.course.points
            var gradeTotal = studentGrades.Where(x => x.Grade != null).Sum(x => x.Grade);
            var pointsTotal = studentGrades.Where(x => x.Grade != null).Sum(x => x.Assignment.Points);

            var total = gradeTotal / pointsTotal;
            var formattedTotal = string.Format("{0:P2}", total);

            ViewBag.Total = formattedTotal;

            return View(studentGradesViewModel);
        }

        public ActionResult StudentsStatisticsStudent(int id)
        {
            var currentUserId = User.Identity.GetUserId();
            var selectedAssignmentId = id;
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //Get a list of submitted assignments from the SubmissionGrades Table based on the specific assignment
            var allSubmissionGradesOfAssignment = context.SubmissionGrades.Include(x => x.Assignment).Include(x => x.User).Include(x => x.User).Where(x => x.Assignment.AssignmentId == selectedAssignmentId).ToList();

            double currentUserScore = 0.0;

            //user score
            if (allSubmissionGradesOfAssignment.Any(x => x.User.Id == currentUserId && x.Assignment.AssignmentId == selectedAssignmentId))
            {
                currentUserScore = Convert.ToDouble(allSubmissionGradesOfAssignment.Where(x => x.User.Id == currentUserId && x.Assignment.AssignmentId == selectedAssignmentId).FirstOrDefault().Grade);
            }
            else
            {
                currentUserScore = Convert.ToDouble(0);
            }

            //average score
            double averageScore = Convert.ToDouble(allSubmissionGradesOfAssignment.Average(x => x.Grade));

            //low score
            var lowScore = Convert.ToDouble(allSubmissionGradesOfAssignment.Min(x => x.Grade));

            //high score
            var highScore = Convert.ToDouble(allSubmissionGradesOfAssignment.Max(x => x.Grade));

            // save object for x and y values of chart/graph
            List<DataPoint> dataPoints = new List<DataPoint>
            {
                //(range, count of students within range)
                new DataPoint( "Average", averageScore),
                new DataPoint( "Low", lowScore),
                new DataPoint( "High", highScore),
                new DataPoint( "Your Grade", currentUserScore)
            };

            // pass the data to canvasJS via Json
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            //set ViewModel list to defined list above
            var gradeSubmittedAssignmentsViewModel = new gradeSubmittedAssignmentsViewModel()
            {
                SubmittedAssignments = allSubmissionGradesOfAssignment
            };

            return View(gradeSubmittedAssignmentsViewModel);
        }

        public ActionResult FinalGradeStatisticsStudent(int id)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //Get instructor Id
            var studentUserId = User.Identity.GetUserId();

            //Get course based on course Id
            var selectedCourseId = id;

            //get list of student enrollments for class 
            var courseEnrollments = context.Enrollments.Where(x => x.Course.CourseId == selectedCourseId).Include(x => x.User).ToList();

            //get the instructor user id for filtering
            var instructorUserId = context.Enrollments.Where(x => x.Course.CourseId == selectedCourseId).Include(x => x.Course).FirstOrDefault().Course.Instructor.Id;

            //get list of assignments for course
            var courseAssignments = context.Assignments.Where(x => x.Course.CourseId == selectedCourseId).ToList();

            //List of All student grades
            var studentGrades = context.SubmissionGrades.Include(x => x.User).Include(x => x.Assignment).Include("Assignment.Course")
                            .Where(x => x.Assignment.Course.CourseId == selectedCourseId).ToList();

            //filter the instructor out of the class roll
            var courseEnrollmentsStudents = courseEnrollments.Where(x => x.User.Id != instructorUserId).OrderBy(x => x.User.LastName).ToList();

            //get the total points for assignments for the selectedCourseId
            var courseAssignmentPointsTotal = courseAssignments.Where(x => x.Course.CourseId == selectedCourseId).Sum(x => x.Points);

            //get all userId's to use for foreach loop
            var userIds = courseEnrollmentsStudents.Select(x => x.User).Select(y => y.Id).ToList();

            // set percentages per user
            foreach (var userId in userIds)
            {
                // get user's grade based off userId
                var userGradesTotal = studentGrades.Where(x => x.User.Id == userId).Sum(x => x.Grade);

                // set Percentage of user using usergrade
                courseEnrollmentsStudents.Where(x => x.User.Id == userId).FirstOrDefault().User.Percentage = Convert.ToDecimal(userGradesTotal) / courseAssignmentPointsTotal;
            }

            // list of all scores using courseEnrollmentsStudents including count of student that received scored 0-59, 60-69, etc.
            var zeroToSixty = courseEnrollmentsStudents.Where(x => x.User.Percentage < Convert.ToDecimal(.6)).Count();
            var sixtyToSeventy = courseEnrollmentsStudents.Where(x => x.User.Percentage >= Convert.ToDecimal(.6) && x.User.Percentage < Convert.ToDecimal(.7)).Count();
            var seventyToEighty = courseEnrollmentsStudents.Where(x => x.User.Percentage >= Convert.ToDecimal(.7) && x.User.Percentage < Convert.ToDecimal(.8)).Count();
            var eightyToNinety = courseEnrollmentsStudents.Where(x => x.User.Percentage >= Convert.ToDecimal(.8) && x.User.Percentage < Convert.ToDecimal(.9)).Count();
            var ninetyToOneHundred = courseEnrollmentsStudents.Where(x => x.User.Percentage >= Convert.ToDecimal(.9)).Count();

            var currentUserScore = courseEnrollmentsStudents.Where(x => x.User.Id == studentUserId).FirstOrDefault().User.Percentage;

            // save object for x and y values of chart/graph
            List<DataPoint> dataPoints = new List<DataPoint>
            {
                //(range, count of students within range)
                new DataPoint( "0-59%", zeroToSixty),
                new DataPoint( "60-69%", sixtyToSeventy),
                new DataPoint( "70-79%", seventyToEighty),
                new DataPoint( "80-89%", eightyToNinety),
                new DataPoint( "90-100%", ninetyToOneHundred)
            };

            // pass the data to canvasJS via Json
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            var formattedUserGrade = string.Format("{0:P2}", currentUserScore);

            // TODO: add a column for student's particular grade
            ViewBag.UserGrade = formattedUserGrade;

            //set ViewModel list to defined list above
            var ListOfEnrollmentsViewModel = new ListOfEnrollmentsViewModel()
            {
                Enrollments = courseEnrollmentsStudents
            };

            return View(ListOfEnrollmentsViewModel);
        }

        public ActionResult FinalGradeStatisticsInstructor(int id)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //Get instructor Id
            var instructorUserId = User.Identity.GetUserId();

            //Get course based on course Id
            var selectedCourseId = id;
            var selectedCourse = context.Courses.Where(x => x.CourseId == selectedCourseId).FirstOrDefault();

            //get list of student enrollments for class 
            var courseEnrollments = context.Enrollments.Where(x => x.Course.CourseId == selectedCourseId).Include(x => x.User).ToList();

            //get list of assignments for course
            var courseAssignments = context.Assignments.Where(x => x.Course.CourseId == selectedCourseId).ToList();


            //List of All student grades
            var studentGrades = context.SubmissionGrades.Include(x => x.User).Include(x => x.Assignment).Include("Assignment.Course")
                            .Where(x => x.Assignment.Course.CourseId == selectedCourseId).ToList();

            //filter the instructor out of the class roll
            var courseEnrollmentsStudents = courseEnrollments.Where(x => x.User.Id != instructorUserId).OrderBy(x => x.User.LastName).ToList();

            //get the total points for assignments for the selectedCourseId
            var courseAssignmentPointsTotal = courseAssignments.Where(x => x.Course.CourseId == selectedCourseId).Sum(x => x.Points);

            //get all userId's to use for foreach loop
            var userIds = courseEnrollmentsStudents.Select(x => x.User).Select(y => y.Id).ToList();

            // set percentages per user
            foreach (var userId in userIds)
            {
                // get user's grade based off userId
                var userGradesTotal = studentGrades.Where(x => x.User.Id == userId).Sum(x => x.Grade);

                // set Percentage of user using usergrade
                courseEnrollmentsStudents.Where(x => x.User.Id == userId).FirstOrDefault().User.Percentage = Convert.ToDecimal(userGradesTotal) / courseAssignmentPointsTotal;
            }

            // list of all scores using courseEnrollmentsStudents including count of student that received scored 0-59, 60-69, etc.
            var zeroToSixty = courseEnrollmentsStudents.Where(x => x.User.Percentage < Convert.ToDecimal(.6)).Count();
            var sixtyToSeventy = courseEnrollmentsStudents.Where(x => x.User.Percentage >= Convert.ToDecimal(.6) && x.User.Percentage < Convert.ToDecimal(.7)).Count();
            var seventyToEighty = courseEnrollmentsStudents.Where(x => x.User.Percentage >= Convert.ToDecimal(.7) && x.User.Percentage < Convert.ToDecimal(.8)).Count();
            var eightyToNinety = courseEnrollmentsStudents.Where(x => x.User.Percentage >= Convert.ToDecimal(.8) && x.User.Percentage < Convert.ToDecimal(.9)).Count();
            var ninetyToOneHundred = courseEnrollmentsStudents.Where(x => x.User.Percentage >= Convert.ToDecimal(.9)).Count();

            // save object for x and y values of chart/graph
            List<DataPoint> dataPoints = new List<DataPoint>
            {
                //(range, count of students within range)
                new DataPoint( "0-59%", zeroToSixty),
                new DataPoint( "60-69%", sixtyToSeventy),
                new DataPoint( "70-79%", seventyToEighty),
                new DataPoint( "80-89%", eightyToNinety),
                new DataPoint( "90-100%", ninetyToOneHundred)
            };

            // pass the data to canvasJS via Json
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            //set ViewModel list to defined list above
            var ListOfEnrollmentsViewModel = new ListOfEnrollmentsViewModel()
            {
                Enrollments = courseEnrollmentsStudents
            };

            return View(ListOfEnrollmentsViewModel);
        }

        public ActionResult StudentSubmission(int id)
        {
            var selectedSubmissionId = id;
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //Select the specific submission from submissions Table
            var selectedSubmission = context.SubmissionGrades.Where(x => x.ID == selectedSubmissionId).Include(x => x.Assignment).Include(x => x.User).ToList();

            var StudentSubmissionViewModel = new StudentSubmissionViewModel()
            {
                SelectedStudentSubmission = selectedSubmission,
            };

            var submissionType = selectedSubmission.Select(x => x.Assignment.SubmissionType).ToList();
            if (submissionType.Equals("File-Upload"))
            {
                string filePathOriginal = selectedSubmission.Select(x => x.FileSubmission).FirstOrDefault();
                string[] words = filePathOriginal.Split('$');
                string fileDisplayName = words[0];
                ViewBag.displayFile = fileDisplayName;
            }

            return View(StudentSubmissionViewModel);
        }

        //Editing method to create unit test
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StudentSubmission(int id, FormCollection formValues)
        {
            ApplicationDbContext context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //Get the current StudentSubmission for the instructor to grade
            var selectedSubmissionId = id;
            double grade = Convert.ToDouble(Request.Form["Grade"]);

            var selectedSubmission = context.SubmissionGrades.Where(x => x.ID == selectedSubmissionId).Include(x => x.Assignment).Include(x => x.User).ToList();

            Boolean result = AssignmentService.updateStudentGradeService(selectedSubmissionId, grade, context);

            if (result)
            {
                return RedirectToAction("GradeAssignment", new { id = selectedSubmission.FirstOrDefault().Assignment.AssignmentId });
            }
            else
            {
                return RedirectToAction("GradeAssignment", new { id = selectedSubmission.FirstOrDefault().Assignment.AssignmentId });
            }

        }

        public ActionResult DownloadSubmittedAssignemnt(string filePath)
        {
            ////Hopefully will be useful to preserve origial file name and type

            //var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            //var currentStudent = context.Users.Where(x => x.Id == userID).FirstOrDefault();


            //var selectedSubmissionId = Convert.ToInt64(Request.Form["asID"]);

            //var currentAssignment = context.SubmissionGrades.Where(x => x.ID == selectedSubmissionId).FirstOrDefault();

            //var name = currentAssignment.FileSubmission

            ////end


            string fullName = Server.MapPath("~/Content/fileAssignments/" + filePath);

            byte[] fileBytes = GetFile(fullName);

            string filePathOriginal = filePath;
            string[] words = filePathOriginal.Split('$');
            string fileDisplayName = words[0];


            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileDisplayName);

        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

        #region UnusedCode

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

        //Current working method
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult StudentSubmission(int id, FormCollection formValues)
        //{
        //    var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

        //    //Get the current StudentSubmission for the instructor to grade
        //    var selectedSubmissionId = id;
        //    var selectedSubmission = context.SubmissionGrades.Where(x => x.ID == selectedSubmissionId).Include(x => x.Assignment).Include(x => x.User).ToList();

        //    //Do we need the assignment Id?
        //    //var assignmentId = selectedSubmission.FirstOrDefault().Assignment.AssignmentId;


        //    //Update selected Submission with new grade

        //    selectedSubmission.FirstOrDefault().Grade = Convert.ToDouble(Request.Form["Grade"]);
        //    selectedSubmission.FirstOrDefault().GradeAddedOn = DateTime.Now;

        //    //save changes to database
        //    context.SaveChanges();

        //    //changed redirect to land on the list of students to grade, rather than the same assignemnt
        //    return RedirectToAction("GradeAssignment", new { id = selectedSubmission.FirstOrDefault().Assignment.AssignmentId });
        //}

        //public ActionResult SubmitStudentGrade(int id, double grade)
        //{
        //    var newGrade = grade;
        //    var selectedSubmissionId = id;
        //    var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

        //    var selectedSubmission = context.SubmissionGrades.Where(x => x.ID == selectedSubmissionId).Include(x => x.Assignment).Include(x => x.User).ToList();
        //    var assignmentId = selectedSubmission.FirstOrDefault().Assignment.AssignmentId;

        //    // if the submission exists in the database
        //    if (context.SubmissionGrades.Any(x => x.ID == selectedSubmissionId))
        //    {
        //        // set the grade to what the instructor types
        //        var submissionGrades = new SubmissionGrades()
        //        {
        //            Grade = newGrade
        //        };
        //        context.SaveChanges();
        //    }

        //    return RedirectToAction("GradeAssignment", "Assignment", new { id = assignmentId });
        //}

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
        #endregion

    }
}
