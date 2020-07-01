namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Redefined_AssignmentsModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Assignments", "Enrollments_EnrollmentId", "dbo.Enrollments");
            DropIndex("dbo.Assignments", new[] { "Enrollments_EnrollmentId" });
            AddColumn("dbo.Assignments", "Course_CourseID", c => c.Int());
            AddColumn("dbo.Assignments", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Assignments", "Course_CourseID");
            CreateIndex("dbo.Assignments", "ApplicationUser_Id");
            AddForeignKey("dbo.Assignments", "Course_CourseID", "dbo.Courses", "CourseID");
            AddForeignKey("dbo.Assignments", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Assignments", "Enrollments_EnrollmentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assignments", "Enrollments_EnrollmentId", c => c.Int());
            DropForeignKey("dbo.Assignments", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Assignments", "Course_CourseID", "dbo.Courses");
            DropIndex("dbo.Assignments", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Assignments", new[] { "Course_CourseID" });
            DropColumn("dbo.Assignments", "ApplicationUser_Id");
            DropColumn("dbo.Assignments", "Course_CourseID");
            CreateIndex("dbo.Assignments", "Enrollments_EnrollmentId");
            AddForeignKey("dbo.Assignments", "Enrollments_EnrollmentId", "dbo.Enrollments", "EnrollmentId");
        }
    }
}
