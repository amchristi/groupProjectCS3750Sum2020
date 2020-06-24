namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class course_model_update : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Courses", "InstructorName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Courses", "InstructorName", c => c.String(nullable: false));
        }
    }
}
