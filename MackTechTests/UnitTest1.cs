using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MackTechGroupProject.BusinessLogic;
using System.Data.Entity;
using MackTechGroupProject.Models;

namespace MackTechTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1() //example test provided by Prof Christi
        {
            var _context = new MackTechGroupProject.Models.ApplicationDbContext();
            var y = _context.Assignments.Where(x => x.Points > 10);
            System.Diagnostics.Debug.WriteLine(y.Count());
            Assert.IsTrue(y.Count() > 0);

        }

        [TestMethod]
        public void RegisterThirdCourse() //uses studentregtwocourses@test.com
        {
            //Q: can we add a course to existing enrollments

            //prep
            var _context = new MackTechGroupProject.Models.ApplicationDbContext();
            var sUserEmail = "StudentRegTwoCourses@test.com";
            var sUserId = "2eb4cb27-3c69-4bd5-9eb0-eb0dd4991a7f";
            int courseId = 34;
            var currentEnrollments = _context.Enrollments.Where(x => x.User.Email == sUserEmail).Include(x => x.User).Include(x => x.Course).ToList();

            //perform operations
            Boolean result = RegistrationService.AddCourseService(sUserId, courseId, currentEnrollments, _context);

            //verify and interpret results
            Assert.IsTrue(result);

            var y = _context.Enrollments.Where(x => x.User.Email == sUserEmail);
            System.Diagnostics.Debug.WriteLine(y.Count());
            Assert.IsTrue(y.Count() == 3);
        }

        [TestMethod]
        public void DeleteThirdCourseRegistration() //uses studentregtwocourses@test.com
        {
            //Q: can we delete the third course add above

            //prep
            var _context = new MackTechGroupProject.Models.ApplicationDbContext();
            var sUserEmail = "StudentRegTwoCourses@test.com";
            var sUserId = "2eb4cb27-3c69-4bd5-9eb0-eb0dd4991a7f";
            int courseId = 34;
            var selectedEnrollment = _context.Enrollments.Where(x => x.User.Id == sUserId).Where(x => x.Course.CourseId == courseId).FirstOrDefault();
            var selectedEnrollmentId = selectedEnrollment.EnrollmentId;
            var currentEnrollments = _context.Enrollments.Where(x => x.User.Email == sUserEmail).Include(x => x.User).Include(x => x.Course).ToList();

            //perform operations
            Boolean result = RegistrationService.DeleteCourseService(selectedEnrollmentId, currentEnrollments, _context);

            //verify and interpret results
            Assert.IsTrue(result);

            var y = _context.Enrollments.Where(x => x.User.Email == sUserEmail);
            System.Diagnostics.Debug.WriteLine(y.Count());
            Assert.IsTrue(y.Count() == 2);
        }

        [TestMethod]
        public void AddAssignment()
        {
            //Q: can we add an assignment?

            //prep
            var _context = new MackTechGroupProject.Models.ApplicationDbContext();
            int courseId = 34;
            var selectedCourse = _context.Courses.Where(x => x.CourseId == courseId).FirstOrDefault();
            //Guid assignmentGuid = Guid.NewGuid();

            var assignment = new Assignment
            {
                Course = selectedCourse,
                Points = 100,
                AssignmentTitle = "Unit Test",
                AssignmentDescription = "Unit Test added course",
                DueDate = DateTime.Now,
                SubmissionType = "Text-Submission",
                AssignmentAddedOn = DateTime.Now
            };

            //perform operations
            Boolean result = AssignmentService.AddAssignmentService(courseId, assignment, _context);

            //verify and interpret results
            Assert.IsTrue(result);

            var y = _context.Assignments.Where(x => x.AssignmentId == assignment.AssignmentId);
            System.Diagnostics.Debug.WriteLine(y.Count());
            Assert.IsTrue(y.Count() == 1);
        }

        [TestMethod]
        public void DeleteAssignment() //uses studentregtwocourses@test.com
        {
            //Q: can we delete the assignment added from above?

            //prep
            var _context = new MackTechGroupProject.Models.ApplicationDbContext();
            //int courseId = 6;
            var selectedAssignment = _context.Assignments.Where(x => x.AssignmentTitle.Equals("Unit Test")).FirstOrDefault();

            //perform operations
            Boolean result = AssignmentService.DeleteAssignmentService(selectedAssignment.AssignmentId, _context);

            //verify and interpret results
            Assert.IsTrue(result);

            var y = _context.Assignments.Where(x => x.AssignmentTitle == selectedAssignment.AssignmentTitle);
            System.Diagnostics.Debug.WriteLine(y.Count());
            Assert.IsTrue(y.Count() == 0);
        }

        [TestMethod]
        public void updateStudentGrade() //TestStudentGradedSubmission@mail.univ.edu
        {
            //Q: can a teacher save or edit a grade?

            //prep
            var _context = new MackTechGroupProject.Models.ApplicationDbContext();
            int selectedSubmissionId = 594;
            double grade = 25;

            //perform operations
            Boolean result = AssignmentService.updateStudentGradeService(selectedSubmissionId, grade, _context);


            //verify and interpret results
            Assert.IsTrue(result);

            var y = _context.SubmissionGrades.Where(x => x.ID == selectedSubmissionId);

            Assert.IsTrue(y.FirstOrDefault().Grade == 25);
        }

        [TestMethod]
        public void submitTextAssignment() //TestStudentTextSubmission@mail.univ.edu
        {
            //Q: can a student submit a text assignment?

            //prep

            var _context = new MackTechGroupProject.Models.ApplicationDbContext();

            var sUserId = "7033fb11-e3e3-465a-831c-55a0dd215343"; //TestStudent TextSubmission
            var aAssignmentId = 148; //MATH 1040 - Assignment 1 - text submission assignment

            var currentAssignment = _context.Assignments.Where(x => x.AssignmentId == aAssignmentId).FirstOrDefault();
            var currentStudent = _context.Users.Where(x => x.Id == sUserId).FirstOrDefault();
            string text = "This is a unit Test.";

            //create a submissionGrade object
            SubmissionGrades submissionGrade = new SubmissionGrades()
            {
                User = currentStudent,
                Assignment = currentAssignment,
                SubmissionDate = DateTime.Now,
                TextSubmission = text,
                FileSubmission = null,
                Grade = null
            };

            SubmitAssignmentModel model = new SubmitAssignmentModel
            {
                SubmissionText = text,
                currentAssignment = currentAssignment,
                assignmentID = (int)aAssignmentId
            };

            //perform operations
            Boolean result = AssignmentService.submitAssignmentService(sUserId, aAssignmentId, model, _context);

            //perform operations
            //Boolean result = AssignmentService.submitTextAssignmentService(aAssignmentId, submissionGrade, _context);


            //verify and interpret results
            Assert.IsTrue(result);

            var y = _context.SubmissionGrades.Where(x => x.User.Id == sUserId).FirstOrDefault();

            Assert.IsTrue(y.TextSubmission.Equals(text));
        }

        [TestMethod]
        public void submitSecondTextAssignment()
        {

            // Q: does a second text submission for same assignment remove the first and add the second in its place?
            //using testsecondtextsubmission@mail.univ.edu who already has one text submission submitted

            var _context = new MackTechGroupProject.Models.ApplicationDbContext();

            var sUserId = "fd6deecf-8a00-4458-a1a5-0810c3213b67"; //TestStudent TextSubmission
            long aAssignmentId = 148; //MATH 1040 - Assignment 1 - text submission assignment

            var currentAssignment = _context.Assignments.Where(x => x.AssignmentId == aAssignmentId).FirstOrDefault();
            var currentStudent = _context.Users.Where(x => x.Id == sUserId).FirstOrDefault();
            string text = "This is a second text submission for same assignment.";

            SubmitAssignmentModel model = new SubmitAssignmentModel
            {
                SubmissionText = text,
                currentAssignment = currentAssignment,
                assignmentID = (int)aAssignmentId
            };

            //perform operations
            Boolean result = AssignmentService.submitAssignmentService(sUserId, aAssignmentId, model, _context);
            

            //verify and interpret results
            Assert.IsTrue(result);

            var yList = _context.SubmissionGrades.Where(x => x.User.Id == sUserId).Where(x => x.Assignment.AssignmentId == (int)aAssignmentId).ToList();
            var ySingle = _context.SubmissionGrades.Where(x => x.User.Id == sUserId).Where(x => x.Assignment.AssignmentId == (int)aAssignmentId).FirstOrDefault();
            Assert.IsTrue(yList.Count() == 1);
            Assert.IsTrue(ySingle.TextSubmission.Equals(text));
        }


        [TestMethod]
        public void InstructorCreateNewCourse() //TestInstructorCreateCourse@mail.univ.edu
        {
            //Q: can an instructor create a new course?

            //prep
            var _context = new MackTechGroupProject.Models.ApplicationDbContext();
            var InstructorId = "70575558-0756-4469-bab7-f1a0efbb327d";//TestInstructorCreateCourse@mail.univ.edu
            var currentInstructor = _context.Users.Where(x => x.Id == InstructorId).FirstOrDefault();
            var instructorName = currentInstructor.FirstName + " " + currentInstructor.LastName;

            //create a Course Object
            var course = new Course
            {
                Department = "Psych",
                CourseNumber = 3010,
                CourseName = "Advanced Psychology",
                Instructor = currentInstructor,
                InstructorName = instructorName,
                CreditHours = 3,
                ClassLocation = "WSU",
                MaxCapacity = 90
            };


            //Perform Operations

            //check if the class already exists
            var hasCourse = RegistrationService.hasCourse(course, InstructorId, _context);

            //If the class exists delete it
            if (hasCourse)
            {
                RegistrationService.DeleteDuplicateCourse(course, InstructorId, _context);
            }

            //check again if class exists
            var notCurrentCourse = RegistrationService.hasCourse(course, InstructorId, _context);

            //display result for class not existing
            Assert.IsFalse(notCurrentCourse);

            //Add Course using business logic class
            var CourseAdded = RegistrationService.addNewCourse(course, _context);

            //display result for sending course to business logic
            Assert.IsTrue(CourseAdded);

            //check database that course was actually added
            var added = _context.Courses.Any(x => x.CourseName == course.CourseName && x.CourseNumber == course.CourseNumber
                                                                               && x.Instructor.Id == InstructorId);
            Assert.IsTrue(added);

        }


        [TestMethod]
        public void testCalendarExists() //TestStudentCalendarTest@mail.univ.edu
        {
            //Q: can a student or instructor see the calendar

            //prep

            var _context = new MackTechGroupProject.Models.ApplicationDbContext();

            var sUserId = "4328fc0b-e442-4e24-8260-c7a7ec2402ba"; //TestStudentCalendarTest@mail.univ.edu
            var currentStudent = _context.Users.Where(x => x.Id == sUserId).FirstOrDefault();

            //perform operations
            Boolean result = CalendarService.CheckCalendarService(sUserId, _context);


            //verify and interpret results
            Assert.IsTrue(result);

            var y = _context.SubmissionGrades.Where(x => x.User.Id == sUserId);

            Assert.IsTrue(result);
        }

    }
}
