namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assignment_Table_Added : DbMigration
    {
        public override void Up()
        {
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
                        Course_CourseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssignmentId)
                .ForeignKey("dbo.Courses", t => t.Course_CourseID, cascadeDelete: true)
                .Index(t => t.Course_CourseID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assignments", "Course_CourseID", "dbo.Courses");
            DropIndex("dbo.Assignments", new[] { "Course_CourseID" });
            DropTable("dbo.Assignments");
        }
    }
}
