namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bCompleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormEntries", "bCompleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormEntries", "bCompleted");
        }
    }
}
