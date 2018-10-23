namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class typevar2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormEntries", "var2Type", c => c.Int(nullable: false));
            AddColumn("dbo.FormEntries", "var3Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormEntries", "var3Type");
            DropColumn("dbo.FormEntries", "var2Type");
        }
    }
}
