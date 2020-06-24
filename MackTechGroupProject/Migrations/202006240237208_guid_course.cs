namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guid_course : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "CRN", c => c.Guid(nullable: false));
            AddColumn("dbo.Courses", "Department", c => c.String(nullable: false));
            AddColumn("dbo.Courses", "CourseNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "CourseNumber");
            DropColumn("dbo.Courses", "Department");
            DropColumn("dbo.Courses", "CRN");
        }
    }
}
