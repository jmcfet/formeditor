namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class movecompletionfagstoblock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blocks", "bExpectedMessageSent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Blocks", "bWorkNotFinishedinTime", c => c.Boolean(nullable: false));
            AddColumn("dbo.Blocks", "bnMinuteWarningSent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Blocks", "bActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blocks", "bActive");
            DropColumn("dbo.Blocks", "bnMinuteWarningSent");
            DropColumn("dbo.Blocks", "bWorkNotFinishedinTime");
            DropColumn("dbo.Blocks", "bExpectedMessageSent");
        }
    }
}
