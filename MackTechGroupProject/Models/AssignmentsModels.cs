using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MackTechGroupProject.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }

        public Course Course { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public int Points { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Assignment Title")]
        public string AssignmentTitle { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Assignment Description")]
        public string AssignmentDescription { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Due Date")]
        [DisplayFormat(DataFormatString="{0:MM/dd/yyyy}", ApplyFormatInEditMode =true)]
        public DateTime DueDate { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "What type of Submission: ")]
        public string SubmissionType { get; set; }

        public DateTime? AssignmentAddedOn { get; set; }

        [NotMapped]
        public double? Score { get; set; }
    }

    public class AddAssignmentViewModel
    {
        // Display Attribute will appear in the Html.LabelFor
        [Display(Name = "User Role")]
        public int SelectedCourseId { get; set; }
        public IEnumerable<SelectListItem> InstructorCourses { get; set; }
    }

    public class SubmitAssignmentModel
    {
        [Display(Name = "Text Submission")]
        [DataType(DataType.MultilineText)]
        public string SubmissionText { get; set; }

        public Assignment currentAssignment { get; set; }

        [Display(Name = "File Submission")]
        public HttpPostedFileBase File { get; set; }

        public int assignmentID { get; set; }

    }

    public class CalendarEventViewModel
    {
        public string AssignmentTitle { get; set; }
        public string AssignmentDescription { get; set; }
        public DateTime DueDate { get; set; }
        public string Department { get; set; }
        public int CourseNumber { get; set; }
    }

    public class ToDoListViewModel
    {
        public List<Enrollment> currentEnrollmentsView { get; set; }
        public List<Assignment> currentAssignmentsView { get; set; }
    }

    public class AllAssignmentsViewModel
    {
        public List<Assignment> AllAssignments { get; set; }
    }

    public class SubmissionGrades
    {
        public int ID { get; set; }
        public ApplicationUser User { get; set; }
        public Assignment Assignment { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string TextSubmission { get; set; }
        public string FileSubmission { get; set; }
        public double? Grade { get; set; }
        public DateTime? GradeAddedOn { get; set; }
        
        [NotMapped]
        public decimal? Percentage { get; set; }
    }

    public class gradeSubmittedAssignmentsViewModel
    {
        public List<SubmissionGrades> SubmittedAssignments { get; set; }
    }

    public class StudentSubmissionViewModel
    {
        public List<SubmissionGrades> SelectedStudentSubmission { get; set; }
    }

    public class StudentGradesViewModel
    {
        public List<SubmissionGrades> StudentGrades { get; set; }
        public List<Assignment> CourseAssignments { get; set; }
    }

    public class StudentNotificationViewModel
    {
        public string AssignmentTitle { get; set; }
        public double Grade { get; set; }
        public int Points { get; set; }
        public DateTime? DueDate { get; set; }
        public string Department { get; set; }
        public int CourseNumber { get; set; }
    }
}