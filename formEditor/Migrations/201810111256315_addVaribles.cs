namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVaribles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormEntries", "Var1", c => c.String());
            AddColumn("dbo.FormEntries", "var2", c => c.String());
            AddColumn("dbo.FormEntries", "var3", c => c.String());
            AddColumn("dbo.FormEntries", "var4", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormEntries", "var4");
            DropColumn("dbo.FormEntries", "var3");
            DropColumn("dbo.FormEntries", "var2");
            DropColumn("dbo.FormEntries", "Var1");
        }
    }
}
