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


        //method to determine if an instructor is already teaching this same class
        public static Boolean hasCourse(Course course, string userId, ApplicationDbContext context)
        {
            //Does this instructor already teach this class? 
            return (context.Courses.Any(x => x.CourseName == course.CourseName && x.CourseNumber == course.CourseNumber
                                                                               && x.Instructor.Id == userId));
        }

        //If the instructor is already teaching this class. Remove the current record. 
        public static Boolean DeleteDuplicateCourse(Course course, string userId, ApplicationDbContext context)
        {

            //if enrollment exists
            var hasEnrollment = context.Enrollments.Any(x => x.Course.CourseName == course.CourseName && x.User.Id == userId);
            if (hasEnrollment)
            {
                var EnrollmentToBeRemoved = context.Enrollments.Where(x => x.Course == course && x.User.Id == userId).FirstOrDefault();
                context.Enrollments.Remove(EnrollmentToBeRemoved);
                context.SaveChanges();
            }

            var CourseToBeRemoved = context.Courses.Where(x => x.CourseName == course.CourseName && x.CourseNumber == course.CourseNumber
                                                                               && x.Instructor.Id == userId).FirstOrDefault();

            context.Courses.Remove(CourseToBeRemoved);
            var removed = context.SaveChanges();

            if (removed > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //Allow a instructor to create a new class for students to take. 
        public static Boolean addNewCourse(Course course, ApplicationDbContext context)
        {
            
            context.Courses.Add(course);
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

    }
}