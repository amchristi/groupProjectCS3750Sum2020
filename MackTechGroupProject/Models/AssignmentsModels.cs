using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
}