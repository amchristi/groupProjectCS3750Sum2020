using MackTechGroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Stripe;

namespace MackTechGroupProject.BusinessLogic
{
    public class RegistrationService
    {


        public static Boolean AddCourseService(string userId, int selectedCourseId, List<Enrollment> currentEnrollments, ApplicationDbContext context)
        {
            
            var currentStudent = context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var selectedCourse = context.Courses.Where(x => x.CourseId == selectedCourseId).FirstOrDefault();

            var studentEnrollment = new Enrollment
            {
                Course = selectedCourse,
                User = currentStudent
            };
            //check to see if the course already exists in student's currentEnrollments
            bool hasCourse = currentEnrollments.Any(x => x.Course.CourseId == selectedCourseId);

            if (!hasCourse)
            {
                context.Enrollments.Add(studentEnrollment);
                context.SaveChanges();

                currentEnrollments.Add(studentEnrollment);

                return true;
            }
            else
            {
                return false;
            }

        }

        public static Boolean DeleteCourseService(int selectedEnrollmentId, List<Enrollment> currentEnrollments, ApplicationDbContext context)
        {

            // query current enrollments list and delete selected course where the selectedCourseId == x.Course.CourseI
            var currentEnrollmentList = currentEnrollments.Where(x => x.EnrollmentId == selectedEnrollmentId).FirstOrDefault();

            //currentEnrollments.Remove(selectedCourse);
            currentEnrollments.Remove(currentEnrollmentList);

            //DELETE from database as well
            var currentEnrollmentDB = context.Enrollments.Where(x => x.EnrollmentId == selectedEnrollmentId).FirstOrDefault();
            context.Enrollments.Remove(currentEnrollmentDB);
            int result = context.SaveChanges();
            if(result > 0)
            {
                return true;
            } else
            {
                return false;
            }
            
        }

    }
}