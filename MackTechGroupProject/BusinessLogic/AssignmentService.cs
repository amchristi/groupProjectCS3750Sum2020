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
    }
}