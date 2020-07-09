namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Accounting_Table_Updated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accountings",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        PaymentDate = c.DateTime(nullable: false),
                        TotalBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accountings", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Accountings", new[] { "User_Id" });
            DropTable("dbo.Accountings");
        }
    }
}
