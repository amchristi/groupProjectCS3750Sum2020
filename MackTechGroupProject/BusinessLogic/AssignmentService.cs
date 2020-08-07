using MackTechGroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace MackTechGroupProject.BusinessLogic
{
    public class AssignmentService
    {
        public static Boolean AddAssignmentService(int selectedCourseId, Assignment assignment, ApplicationDbContext context)
        {
            var selectedCourse = context.Courses.Where(x => x.CourseId == selectedCourseId).FirstOrDefault();

            //check to see if the course already has assignment
            bool hasAssignment = context.Assignments.Any(x => x.AssignmentId == assignment.AssignmentId);

            if (!hasAssignment)
            {
                context.Assignments.Add(assignment);
                context.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }

        public static Boolean DeleteAssignmentService(int selectedAssignmentId, ApplicationDbContext context)
        {
            //DELETE from database
            var selectedAssignment = context.Assignments.Where(x => x.AssignmentId == selectedAssignmentId).FirstOrDefault();
            context.Assignments.Remove(selectedAssignment);
            int result = context.SaveChanges();
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Boolean HasCurrentSubmission(long selectedAssignmentId, string studentId, ApplicationDbContext context)
        {
            return (context.SubmissionGrades.Any(x => x.Assignment.AssignmentId == selectedAssignmentId && x.User.Id == studentId));
        }


        public static Boolean updateStudentGradeService(int selectedSubmissionId, double grade, ApplicationDbContext context)
        {

            var selectedSubmission = context.SubmissionGrades.Where(x => x.ID == selectedSubmissionId).FirstOrDefault();


            if (selectedSubmission.ID == selectedSubmissionId)
            {
                //Update selected Submission with new grade
                selectedSubmission.Grade = grade;
                selectedSubmission.GradeAddedOn = DateTime.Now;

                //save changes to database
                context.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }

        }

        public static Boolean submitAssignmentService(String userID, long selectedAssignmentId, SubmitAssignmentModel model, ApplicationDbContext context)
        {
            var currentStudent = context.Users.Where(x => x.Id == userID).FirstOrDefault();
            var File = model.File;
            var currentAssignment = context.Assignments.Where(x => x.AssignmentId == selectedAssignmentId).FirstOrDefault();

            bool hasSubmission = AssignmentService.HasCurrentSubmission(selectedAssignmentId, userID, context);

            if (hasSubmission)
            {
                var submissionToBeRemoved = context.SubmissionGrades.Where(x => x.Assignment.AssignmentId == selectedAssignmentId && x.User.Id == userID).FirstOrDefault();

                //if FILE, use above to query file path and delete file from content folder
                if (File != null)
                {
                    String filename = submissionToBeRemoved.FileSubmission;
                    //string path = HostingEnvironment.MapPath("~/Content/fileAssignments/");
                    var path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "MackTechGroupProject", "fileAssignments");
                    String fileSubmissionPath = path + filename;
                    System.IO.File.Delete(fileSubmissionPath);
                }

                //remove row from table
                context.SubmissionGrades.Remove(submissionToBeRemoved);
                context.SaveChanges();
            }


            if (File != null)
            {
                //string path = HostingEnvironment.MapPath("~/Content/fileAssignments/");
                var path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "MackTechGroupProject", "fileAssignments");
                if (!Directory.Exists(path))
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
                return true;
            }
            //method for unit testing
            else if (model.SubmissionText != null)
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
                context.SubmissionGrades.Add(submissionGrade);
                context.SaveChanges();
                return true;

            }
            else
            {
                return false;
            }
        }

        //deprecated
        public static Boolean submitTextAssignmentService(long selectedAssignmentId, SubmissionGrades submissionGrade, ApplicationDbContext context)
        {

            bool hasAssignment = context.Assignments.Any(x => x.AssignmentId == selectedAssignmentId);

            if (hasAssignment)
            {

                context.SubmissionGrades.Add(submissionGrade);

                //save changes to database
                context.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }

        }
    }

}