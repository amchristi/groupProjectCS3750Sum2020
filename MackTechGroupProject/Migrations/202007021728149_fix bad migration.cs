namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixbadmigration : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Enrollments", new[] { "Course_CourseID" });
            CreateTable(
                "dbo.Assignments",
                c => new
                    {
                        AssignmentId = c.Int(nullable: false, identity: true),
                        Points = c.Int(nullable: false),
                        Score = c.Int(nullable: false),
                        AssignmentTitle = c.String(nullable: false),
                        AssignmentDescription = c.String(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        SubmissionType = c.String(nullable: false),
                        Course_CourseId = c.Int(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AssignmentId)
                .ForeignKey("dbo.Courses", t => t.Course_CourseId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.Course_CourseId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateIndex("dbo.Enrollments", "Course_CourseId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assignments", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Assignments", "Course_CourseId", "dbo.Courses");
            DropIndex("dbo.Enrollments", new[] { "Course_CourseId" });
            DropIndex("dbo.Assignments", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Assignments", new[] { "Course_CourseId" });
            DropTable("dbo.Assignments");
            CreateIndex("dbo.Enrollments", "Course_CourseID");
        }
    }
}
