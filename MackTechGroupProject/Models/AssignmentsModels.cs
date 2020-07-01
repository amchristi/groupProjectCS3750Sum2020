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

        [DataType(DataType.Text)]
        public int Score { get; set; }

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
        public DateTime DueDate { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "What type of Submission: ")]
        public string SubmissionType { get; set; }
    }

    public class AddAssignmentViewModel
    {
        // Display Attribute will appear in the Html.LabelFor
        [Display(Name = "User Role")]
        public int SelectedCourseId { get; set; }
        public IEnumerable<SelectListItem> InstructorCourses { get; set; }
    }
}