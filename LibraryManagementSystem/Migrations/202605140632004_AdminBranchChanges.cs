namespace LibraryManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminBranchChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "BorrowSettingsId", c => c.Int(nullable: false));
            AddColumn("dbo.BorrowSettings", "LibraryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Books", "BorrowSettingsId");
            AddForeignKey("dbo.Books", "BorrowSettingsId", "dbo.BorrowSettings", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Books", "BorrowSettingsId", "dbo.BorrowSettings");
            DropIndex("dbo.Books", new[] { "BorrowSettingsId" });
            DropColumn("dbo.BorrowSettings", "LibraryId");
            DropColumn("dbo.Books", "BorrowSettingsId");
        }
    }
}
