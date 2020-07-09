namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Accounting_Table_Added : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Assignments", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Assignments", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Assignments", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assignments", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Assignments", "ApplicationUser_Id");
            AddForeignKey("dbo.Assignments", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
