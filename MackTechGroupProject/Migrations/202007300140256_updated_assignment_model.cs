namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updated_assignment_model : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Assignments", "Score");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assignments", "Score", c => c.Int(nullable: false));
        }
    }
}
