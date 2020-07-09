namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TotalCreditHours_to_Accounting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accountings", "TotalCreditHours", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accountings", "TotalCreditHours");
        }
    }
}
