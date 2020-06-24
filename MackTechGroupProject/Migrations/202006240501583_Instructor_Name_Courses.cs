namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Instructor_Name_Courses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "InstructorName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "InstructorName");
        }
    }
}
