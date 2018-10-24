namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcheckbox : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormEntries", "checkbox1State", c => c.Boolean(nullable: false));
            AddColumn("dbo.FormEntries", "checkbox2State", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormEntries", "checkbox2State");
            DropColumn("dbo.FormEntries", "checkbox1State");
        }
    }
}
