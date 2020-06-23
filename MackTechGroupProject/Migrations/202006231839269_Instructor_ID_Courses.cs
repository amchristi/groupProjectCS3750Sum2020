namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Instructor_ID_Courses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "Instructor_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Courses", "Instructor_Id");
            AddForeignKey("dbo.Courses", "Instructor_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            DropColumn("dbo.Courses", "InstructorID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courses", "InstructorID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Courses", "Instructor_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Courses", new[] { "Instructor_Id" });
            DropColumn("dbo.Courses", "Instructor_Id");
        }
    }
}
