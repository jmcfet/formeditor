namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class expectedstarttimespan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blocks", "ExpectedStart", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blocks", "ExpectedStart");
        }
    }
}
