namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class linenum : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FormEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        type = c.Int(nullable: false),
                        label1 = c.String(),
                        label2 = c.String(),
                        checkbox1 = c.String(),
                        checkbox2 = c.String(),
                        textbox1 = c.String(),
                        textbox2 = c.String(),
                        textbox3 = c.String(),
                        textbox4 = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FormEntries");
        }
    }
}
