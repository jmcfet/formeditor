namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class selected : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blocks", "Selected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blocks", "Selected");
        }
    }
}
