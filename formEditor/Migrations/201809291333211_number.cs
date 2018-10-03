namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class number : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormEntries", "linenum", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormEntries", "linenum");
        }
    }
}
