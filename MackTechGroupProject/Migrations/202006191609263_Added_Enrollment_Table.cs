namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Enrollment_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Enrollments",
                c => new
                    {
                        EnrollmentId = c.Int(nullable: false, identity: true),
                        Course_CourseID = c.Int(),
                        Student_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.EnrollmentId)
                .ForeignKey("dbo.Courses", t => t.Course_CourseID)
                .ForeignKey("dbo.AspNetUsers", t => t.Student_Id)
                .Index(t => t.Course_CourseID)
                .Index(t => t.Student_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Enrollments", "Student_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Enrollments", "Course_CourseID", "dbo.Courses");
            DropIndex("dbo.Enrollments", new[] { "Student_Id" });
            DropIndex("dbo.Enrollments", new[] { "Course_CourseID" });
            DropTable("dbo.Enrollments");
        }
    }
}
