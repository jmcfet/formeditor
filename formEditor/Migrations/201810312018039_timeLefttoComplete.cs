namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timeLefttoComplete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blocks", "timeLefttoComplete", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blocks", "timeLefttoComplete");
        }
    }
}
