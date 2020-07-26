namespace MackTechGroupProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_AssignmentAddedOn_column : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assignments", "AssignmentAddedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assignments", "AssignmentAddedOn");
        }
    }
}
