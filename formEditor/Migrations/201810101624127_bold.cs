namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bold : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormEntries", "isBold", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormEntries", "isBold");
        }
    }
}
