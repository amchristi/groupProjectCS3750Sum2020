using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MackTechGroupProject.Models
{
   /// <summary>
   /// This is the course model class that creates the scaffolding for the database
   /// </summary>
    public class Course
    {
        [Display(Name = "Course ID")]
        public int CourseId { get; set; }

        public Guid CRN { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Department { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name ="Course Number")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Invalid Course Number")]
        public int CourseNumber { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Display(Name = "Instructor ID")]
        public ApplicationUser Instructor { get; set; }

        [Display(Name = "Instructor Name")]
        public string InstructorName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[0-5]{1}$", ErrorMessage = "Invalid Number of Credit Hours")]
        [Display(Name = "Credit Hours")]
        public int CreditHours { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Invalid Location")]
        [Display(Name = "Class Location")]
        public string ClassLocation { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^([1-9]|[1-9][0-9]|100)$", ErrorMessage = "Invalid Max Capacity")]
        [Display(Name = "Max Capacity")]
        public int MaxCapacity { get; set; }

        /*
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        [Display(Name = "Class Time")]
        public DateTime ClassTime { get; set; }
        */

        public ICollection<Enrollment> Enrollments { get; set; }

        public ICollection<Assignment> Assignments { get; set; }
    }

    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public ApplicationUser User { get; set; }
        public Course Course { get; set; }
    }
}