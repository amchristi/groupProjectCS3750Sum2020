namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Course_Database_Change : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Courses", "Instructor_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Courses", new[] { "Instructor_Id" });
            AlterColumn("dbo.Courses", "Instructor_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Courses", "Instructor_Id");
            AddForeignKey("dbo.Courses", "Instructor_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "Instructor_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Courses", new[] { "Instructor_Id" });
            AlterColumn("dbo.Courses", "Instructor_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Courses", "Instructor_Id");
            AddForeignKey("dbo.Courses", "Instructor_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
