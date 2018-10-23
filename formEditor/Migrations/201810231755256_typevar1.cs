namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class typevar1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormEntries", "var1Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormEntries", "var1Type");
        }
    }
}
