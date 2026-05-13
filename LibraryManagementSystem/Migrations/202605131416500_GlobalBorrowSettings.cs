namespace LibraryManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GlobalBorrowSettings : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Libraries", "BorrowSettings_Id", "dbo.BorrowSettings");
            DropIndex("dbo.Libraries", new[] { "BorrowSettings_Id" });
            DropColumn("dbo.Libraries", "BorrowSettings_Id");
            DropColumn("dbo.BorrowSettings", "LibraryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BorrowSettings", "LibraryId", c => c.Int(nullable: false));
            AddColumn("dbo.Libraries", "BorrowSettings_Id", c => c.Int());
            CreateIndex("dbo.Libraries", "BorrowSettings_Id");
            AddForeignKey("dbo.Libraries", "BorrowSettings_Id", "dbo.BorrowSettings", "Id");
        }
    }
}
