using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MackTechGroupProject.BusinessLogic;
using System.Data.Entity;

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
        public void AddThirdCourse() //uses studentregtwocourses@test.com
        {
            //Q: can we add a course to existing enrollments

            //prep
            var _context = new MackTechGroupProject.Models.ApplicationDbContext();
            var sUserEmail = "StudentRegTwoCourses@test.com";
            var sUserId = "2eb4cb27-3c69-4bd5-9eb0-eb0dd4991a7f";
            int courseId = 6;
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
        public void DeleteThirdCourse() //uses studentregtwocourses@test.com
        {
            //Q: can we delete the third course add above

            //prep
            var _context = new MackTechGroupProject.Models.ApplicationDbContext();
            var sUserEmail = "StudentRegTwoCourses@test.com";
            var sUserId = "2eb4cb27-3c69-4bd5-9eb0-eb0dd4991a7f";
            int courseId = 6;
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
    }
}
