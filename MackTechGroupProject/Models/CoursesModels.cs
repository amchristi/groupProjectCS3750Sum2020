using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MackTechGroupProject.Models
{
   /// <summary>
   /// This is the course model class that creates the scaffolding for the database
   /// </summary>
    public class Course
    {
        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Invalid Course ID")]
        [Display(Name = "Course ID")]
        public int CourseID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Invalid Instructor ID")]
        [Display(Name = "Instructor ID")]
        public int InstructorID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[0-4]{1}$", ErrorMessage = "Invalid Instructor ID")]
        [Display(Name = "Credit Hours")]
        public int CreditHours { get; set; }

        /*
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        [Display(Name = "Class Time")]
        public DateTime ClassTime { get; set; }
        */

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "Invalid Location")]
        [Display(Name = "Class Location")]
        public string ClassLocation { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^([1-9]|[1-9][0-9]|100)$", ErrorMessage = "Invalid Max Capacity")]
        [Display(Name = "Max Capacity")]
        public int MaxCapacity { get; set; }


    }

    /// <summary>
    /// For future use if needed
    /// </summary>
    //public class StudentCourse
    //{

    //}
}