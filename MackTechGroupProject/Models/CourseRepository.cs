using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MackTechGroupProject.Models
{
    public class CourseRepository
    {

        private static List<Course> courses = new List<Course>();

        public static IEnumerable<Course> Courses
        {
            get
            {
                return courses;
            }
        }

        public static void AddCourse(Course course)
        {
            courses.Add(course);
        }

    }
}