namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additemresponse : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.itemResponses",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        start = c.DateTime(nullable: false),
                        Name = c.String(),
                        linenum = c.Int(nullable: false),
                        var1 = c.String(),
                        var2 = c.String(),
                        var3 = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.itemResponses");
        }
    }
}
