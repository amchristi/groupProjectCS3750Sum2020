namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnrollmentsAddedToAssignments : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Assignments", "Course_CourseID", "dbo.Courses");
            DropIndex("dbo.Assignments", new[] { "Course_CourseID" });
            AddColumn("dbo.Assignments", "Enrollments_EnrollmentId", c => c.Int());
            CreateIndex("dbo.Assignments", "Enrollments_EnrollmentId");
            AddForeignKey("dbo.Assignments", "Enrollments_EnrollmentId", "dbo.Enrollments", "EnrollmentId");
            DropColumn("dbo.Assignments", "Course_CourseID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assignments", "Course_CourseID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Assignments", "Enrollments_EnrollmentId", "dbo.Enrollments");
            DropIndex("dbo.Assignments", new[] { "Enrollments_EnrollmentId" });
            DropColumn("dbo.Assignments", "Enrollments_EnrollmentId");
            CreateIndex("dbo.Assignments", "Course_CourseID");
            AddForeignKey("dbo.Assignments", "Course_CourseID", "dbo.Courses", "CourseID", cascadeDelete: true);
        }
    }
}
