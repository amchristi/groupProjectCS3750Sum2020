namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_SubmissionGrades : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubmissionGrades",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SubmissionDate = c.DateTime(nullable: false),
                        TextSubmission = c.String(),
                        FileSubmission = c.Binary(),
                        Grade = c.Double(nullable: false),
                        Assignment_AssignmentId = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Assignments", t => t.Assignment_AssignmentId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Assignment_AssignmentId)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubmissionGrades", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.SubmissionGrades", "Assignment_AssignmentId", "dbo.Assignments");
            DropIndex("dbo.SubmissionGrades", new[] { "User_Id" });
            DropIndex("dbo.SubmissionGrades", new[] { "Assignment_AssignmentId" });
            DropTable("dbo.SubmissionGrades");
        }
    }
}
