namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class morelabels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormEntries", "label3", c => c.String());
            AddColumn("dbo.FormEntries", "label4", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormEntries", "label4");
            DropColumn("dbo.FormEntries", "label3");
        }
    }
}
