namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class currentitem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blocks", "CurrentItem", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blocks", "CurrentItem");
        }
    }
}
