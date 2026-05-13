namespace LibraryManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBorrowSettingsIdFromBooks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Books", "BorrowSettingsId", "dbo.BorrowSettings");
            DropIndex("dbo.Books", new[] { "BorrowSettingsId" });
            AddColumn("dbo.Libraries", "BorrowSettings_Id", c => c.Int());
            CreateIndex("dbo.Libraries", "BorrowSettings_Id");
            AddForeignKey("dbo.Libraries", "BorrowSettings_Id", "dbo.BorrowSettings", "Id");
            DropColumn("dbo.Books", "BorrowSettingsId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Books", "BorrowSettingsId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Libraries", "BorrowSettings_Id", "dbo.BorrowSettings");
            DropIndex("dbo.Libraries", new[] { "BorrowSettings_Id" });
            DropColumn("dbo.Libraries", "BorrowSettings_Id");
            CreateIndex("dbo.Books", "BorrowSettingsId");
            AddForeignKey("dbo.Books", "BorrowSettingsId", "dbo.BorrowSettings", "Id");
        }
    }
}
