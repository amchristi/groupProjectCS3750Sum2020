namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubmissionGrades_File_Submission : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SubmissionGrades", "FileSubmission", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SubmissionGrades", "FileSubmission", c => c.Binary());
        }
    }
}
