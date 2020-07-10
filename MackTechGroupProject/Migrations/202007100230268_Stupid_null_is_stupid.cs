namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stupid_null_is_stupid : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SubmissionGrades", "Grade", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SubmissionGrades", "Grade", c => c.Double(nullable: false));
        }
    }
}
