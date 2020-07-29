using MackTechGroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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