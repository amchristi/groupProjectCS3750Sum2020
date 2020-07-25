namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_GradeAddedOn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubmissionGrades", "GradeAddedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubmissionGrades", "GradeAddedOn");
        }
    }
}
