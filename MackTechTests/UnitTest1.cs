using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MackTechTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var _context = new MackTechGroupProject.Models.ApplicationDbContext();
            var y = _context.Assignments.Where(x => x.Points > 10);
            System.Diagnostics.Debug.WriteLine(y.Count());
            Assert.IsTrue(y.Count() > 0);

        }
    }
}
