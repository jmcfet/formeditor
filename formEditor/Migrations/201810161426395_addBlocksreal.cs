namespace formEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBlocksreal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        timer = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.FormEntries", "Block_Id", c => c.Int());
            CreateIndex("dbo.FormEntries", "Block_Id");
            AddForeignKey("dbo.FormEntries", "Block_Id", "dbo.Blocks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FormEntries", "Block_Id", "dbo.Blocks");
            DropIndex("dbo.FormEntries", new[] { "Block_Id" });
            DropColumn("dbo.FormEntries", "Block_Id");
            DropTable("dbo.Blocks");
        }
    }
}
