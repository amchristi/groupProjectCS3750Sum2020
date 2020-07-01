namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_AddAssignmentViewModel : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Assignments", new[] { "Course_CourseID" });
            DropIndex("dbo.Enrollments", new[] { "Course_CourseID" });
            CreateIndex("dbo.Assignments", "Course_CourseId");
            CreateIndex("dbo.Enrollments", "Course_CourseId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Enrollments", new[] { "Course_CourseId" });
            DropIndex("dbo.Assignments", new[] { "Course_CourseId" });
            CreateIndex("dbo.Enrollments", "Course_CourseID");
            CreateIndex("dbo.Assignments", "Course_CourseID");
        }
    }
}
