using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MackTechGroupProject.Models
{
   /// <summary>
   /// This is the course model class that creates the scaffolding for the database
   /// </summary>
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public int InstructorID { get; set; }
        public int CreditHours { get; set; }
    }

    /// <summary>
    /// For future use if needed
    /// </summary>
    //public class StudentCourse
    //{

    //}
}