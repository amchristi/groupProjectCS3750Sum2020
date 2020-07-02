using Microsoft.VisualStudio.TestTools.UnitTesting;
using MackTechGroupProject.Controllers;

namespace MackTechGroupProject.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestStudentRegisterAndDropThirdCourse() //using StudentRegTwoCourses@test.com
        {

            //get student user that is currently enrolled in two courses
            var controller = new CoursesController();
            var result = controller.RegisterForCourse(7);
            Assert.AreEqual("StudentAccount", result);
            
            //register for a 3rd course
            //show that it was successful
            //drop 3rd class
            //show that it was successfully dropped

        }
    }
}
