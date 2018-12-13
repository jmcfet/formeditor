namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addstate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blocks", "state", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blocks", "state");
        }
    }
}
