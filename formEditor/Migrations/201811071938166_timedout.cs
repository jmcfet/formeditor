namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timedout : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blocks", "Warning", c => c.Boolean(nullable: false));
            AddColumn("dbo.Blocks", "TimedOut", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blocks", "TimedOut");
            DropColumn("dbo.Blocks", "Warning");
        }
    }
}
