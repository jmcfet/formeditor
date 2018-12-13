namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addusertoitemresponse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.itemResponses", "userName", c => c.String());
            AddColumn("dbo.itemResponses", "type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.itemResponses", "type");
            DropColumn("dbo.itemResponses", "userName");
        }
    }
}
