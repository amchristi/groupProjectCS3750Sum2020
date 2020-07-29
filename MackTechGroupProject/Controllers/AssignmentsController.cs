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
            var File = model.File;
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var currentStudent = context.Users.Where(x => x.Id == userID).FirstOrDefault();

            
            var selectedAssignmentId = Convert.ToInt64(Request.Form["asID"]);


            var currentAssignment = context.Assignments.Where(x => x.AssignmentId == selectedAssignmentId).FirstOrDefault();


            if (File != null)
            {
                string path = Server.MapPath("~/Content/fileAssignments/");
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Guid g = Guid.NewGuid();
                String filename = System.IO.Path.GetFileName(File.FileName);
                String fileSubmissionPath = path + filename + "$" + g;
                File.SaveAs(fileSubmissionPath);

                SubmissionGrades submissionGrade = new SubmissionGrades()
                {
                    User = currentStudent,
                    Assignment = currentAssignment,
                    SubmissionDate = DateTime.Now,
                    TextSubmission = null,
                    FileSubmission = filename + "$" + g,
                    Grade = null
                };

                context.SubmissionGrades.Add(submissionGrade);
                context.SaveChanges();
            }
            //method for unit testing
            if (model.SubmissionText != null)
            {
                SubmissionGrades submissionGrade = new SubmissionGrades()
                {
                    User = currentStudent,
                    Assignment = currentAssignment,
                    SubmissionDate = DateTime.Now,
                    TextSubmission = model.SubmissionText,
                    FileSubmission = null,
                    Grade = null
                };

                Boolean result = AssignmentService.submitTextAssignmentService(selectedAssignmentId, submissionGrade, context);

                context.SubmissionGrades.Add(submissionGrade);
                context.SaveChanges();
            }

            //Working method
            //if (model.SubmissionText != null)
            //{
            //    SubmissionGrades submissionGrade = new SubmissionGrades()
            //    {
            //        User = currentStudent,
            //        Assignment = currentAssignment,
            //        SubmissionDate = DateTime.Now,
            //        TextSubmission = model.SubmissionText,
            //        FileSubmission = null,
            //        Grade = null
            //    };
            //    context.SubmissionGrades.Add(submissionGrade);
            //    context.SaveChanges();
            //}
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

            var mostRecentSubmissionPerStudent = allSubmissionsOfSelected.GroupBy(x => x.User).Select(x => x.FirstOrDefault(y => y.ID == x.Max(z => z.ID))).OrderBy(x => x.User.Id).ToList();

            //set ViewModel list to defined list above
            var gradeSubmittedAssignmentsViewModel = new gradeSubmittedAssignmentsViewModel()
            {
                SubmittedAssignments = mostRecentSubmissionPerStudent
            };

            return View(gradeSubmittedAssignmentsViewModel);
        }

        public ActionResult ViewGrades(int id)
        {
            var userId = User.Identity.GetUserId();
            var selectedCourseId = id;

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //TODO: change this to only get the most current submission's grade
            var studentGrades = context.SubmissionGrades.Include(x => x.User).Include(x => x.Assignment).Include("Assignment.Course")
                                        .Where(x => x.Assignment.Course.CourseId == selectedCourseId && x.User.Id == userId).ToList();

            var studentGradesViewModel = new StudentGradesViewModel()
            {
                StudentGrades = studentGrades
            };

            //to calculate total get a sum off all score and divide by sum of all assignmnet.course.points
            ViewBag.Total = 89.5;

            return View(studentGradesViewModel);
        }

        public ActionResult StudentsStatisticsInstructor(int id)
        {
            var selectedAssignmentId = id;
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //Select the specific assignment from assignments Table
            var selectedAssignment = context.Assignments.Where(x => x.AssignmentId == selectedAssignmentId).Include(x => x.Course).FirstOrDefault();

            //Get a list of submitted assignments from the SubmissionGrades Table based on the specific assignment
            var allSubmissionGrades = context.SubmissionGrades.Include(x => x.Assignment).Include(x => x.User).Include(x => x.User).ToList();

            var allSubmissionsOfSelected = allSubmissionGrades.Where(x => x.Assignment == selectedAssignment).ToList();

            // get most recent submission per student 
            var mostRecentSubmissionPerStudent = allSubmissionsOfSelected.Where(w => w.Grade != null).GroupBy(x => x.User).Select(x => x.FirstOrDefault(y => y.ID == x.Max(z => z.ID))).OrderBy(x => x.User.Id).ToList();

            // used for calculating percentages
            var assignmentPointTotal = mostRecentSubmissionPerStudent.FirstOrDefault().Assignment.Points;
            var userIds = mostRecentSubmissionPerStudent.Select(x => x.User).Select(y => y.Id).ToList();

            // set percentages per user
            foreach (var userId in userIds)
            {
                // get user's grade based off userId
                var userGrade = mostRecentSubmissionPerStudent.Where(x => x.User.Id == userId).FirstOrDefault().Grade;

                // set Percentage of user using usergrade
                mostRecentSubmissionPerStudent.Where(x => x.User.Id == userId).FirstOrDefault().Percentage = Convert.ToDecimal(userGrade) / assignmentPointTotal;
            }

            // list of all scores using mostRecentSubmissions including count of student that received scored 0-59, 60-69, etc.
            var zeroToSixty = mostRecentSubmissionPerStudent.Where(x=> x.Percentage < Convert.ToDecimal(.6)).Count();
            var sixtyToSeventy = mostRecentSubmissionPerStudent.Where(x => x.Percentage >= Convert.ToDecimal(.6) && x.Percentage < Convert.ToDecimal(.7)).Count();
            var seventyToEighty = mostRecentSubmissionPerStudent.Where(x => x.Percentage >= Convert.ToDecimal(.7) && x.Percentage < Convert.ToDecimal(.8)).Count();
            var eightyToNinety = mostRecentSubmissionPerStudent.Where(x => x.Percentage >= Convert.ToDecimal(.8) && x.Percentage < Convert.ToDecimal(.9)).Count();
            var ninetyToOneHundred = mostRecentSubmissionPerStudent.Where(x => x.Percentage >= Convert.ToDecimal(.9)).Count();

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
                SubmittedAssignments = mostRecentSubmissionPerStudent
            };

            return View(gradeSubmittedAssignmentsViewModel);
        }

        public ActionResult StudentsStatisticsStudent(int id)
        {
            var selectedAssignmentId = id;
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //Select the specific assignment from assignments Table
            var selectedAssignment = context.Assignments.Where(x => x.AssignmentId == selectedAssignmentId).Include(x => x.Course).FirstOrDefault();

            //Get a list of submitted assignments from the SubmissionGrades Table based on the specific assignment
            var allSubmissionGrades = context.SubmissionGrades.Include(x => x.Assignment).Include(x => x.User).Include(x => x.User).ToList();

            var allSubmissionsOfSelected = allSubmissionGrades.Where(x => x.Assignment == selectedAssignment).ToList();

            // get most recent submission per student 
            var mostRecentSubmissionPerStudent = allSubmissionsOfSelected.Where(w => w.Grade != null).GroupBy(x => x.User).Select(x => x.FirstOrDefault(y => y.ID == x.Max(z => z.ID))).OrderBy(x => x.User.Id).ToList();

            // used for calculating percentages
            var assignmentPointTotal = mostRecentSubmissionPerStudent.FirstOrDefault().Assignment.Points;
            var userIds = mostRecentSubmissionPerStudent.Select(x => x.User).Select(y => y.Id).ToList();

            // set percentages per user
            foreach (var userId in userIds)
            {
                // get user's grade based off userId
                var userGrade = mostRecentSubmissionPerStudent.Where(x => x.User.Id == userId).FirstOrDefault().Grade;

                // set Percentage of user using usergrade
                mostRecentSubmissionPerStudent.Where(x => x.User.Id == userId).FirstOrDefault().Percentage = Convert.ToDecimal(userGrade) / assignmentPointTotal;
            }

            // list of all scores using mostRecentSubmissions including count of student that received scored 0-59, 60-69, etc.
            var zeroToSixty = mostRecentSubmissionPerStudent.Where(x => x.Percentage < Convert.ToDecimal(.6)).Count();
            var sixtyToSeventy = mostRecentSubmissionPerStudent.Where(x => x.Percentage >= Convert.ToDecimal(.6) && x.Percentage < Convert.ToDecimal(.7)).Count();
            var seventyToEighty = mostRecentSubmissionPerStudent.Where(x => x.Percentage >= Convert.ToDecimal(.7) && x.Percentage < Convert.ToDecimal(.8)).Count();
            var eightyToNinety = mostRecentSubmissionPerStudent.Where(x => x.Percentage >= Convert.ToDecimal(.8) && x.Percentage < Convert.ToDecimal(.9)).Count();
            var ninetyToOneHundred = mostRecentSubmissionPerStudent.Where(x => x.Percentage >= Convert.ToDecimal(.9)).Count();

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
                SubmittedAssignments = mostRecentSubmissionPerStudent
            };

            return View(gradeSubmittedAssignmentsViewModel);
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
            if (submissionType.Equals("File-Upload")) {
                string filePathOriginal = selectedSubmission.Select(x => x.FileSubmission).FirstOrDefault();
                string[] words = filePathOriginal.Split('$');
                string fileDisplayName = words[0];
                ViewBag.displayFile = fileDisplayName;
            }

            return View(StudentSubmissionViewModel);
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
