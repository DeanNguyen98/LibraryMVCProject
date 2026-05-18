namespace LibraryManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Bio = c.String(),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.BookAuthors",
                c => new
                    {
                        BookId = c.Int(nullable: false),
                        AuthorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BookId, t.AuthorId })
                .ForeignKey("dbo.Authors", t => t.AuthorId)
                .ForeignKey("dbo.Books", t => t.BookId)
                .Index(t => t.BookId)
                .Index(t => t.AuthorId);

            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LibraryId = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        Isbn = c.String(nullable: false, maxLength: 20),
                        Summary = c.String(),
                        CoverImageUrl = c.String(),
                        TotalCopies = c.Int(nullable: false),
                        AvailableCopies = c.Int(nullable: false),
                        BorrowedTimes = c.Int(nullable: false),
                        BorrowSettingsId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BorrowSettings", t => t.BorrowSettingsId)
                .ForeignKey("dbo.Libraries", t => t.LibraryId)
                .Index(t => t.LibraryId)
                .Index(t => t.Isbn, unique: true, name: "IX_Book_Isbn")
                .Index(t => t.BorrowSettingsId);

            CreateTable(
                "dbo.BookGenres",
                c => new
                    {
                        BookId = c.Int(nullable: false),
                        GenreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BookId, t.GenreId })
                .ForeignKey("dbo.Books", t => t.BookId)
                .ForeignKey("dbo.Genres", t => t.GenreId)
                .Index(t => t.BookId)
                .Index(t => t.GenreId);

            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_Genre_Name");

            CreateTable(
                "dbo.BorrowSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LibraryId = c.Int(nullable: false),
                        SettingName = c.String(nullable: false),
                        LoanDurationDays = c.Int(nullable: false),
                        RenewalLimit = c.Int(nullable: false),
                        OverdueFinePerDay = c.Decimal(nullable: false, precision: 10, scale: 2),
                        MaxBorrowableItems = c.Int(nullable: false),
                        Status = c.String(),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.BorrowTransactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                        LibraryId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        BorrowedAt = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        ReturnedAt = c.DateTime(),
                        RenewalsUsed = c.Int(nullable: false),
                        FineAmount = c.Decimal(nullable: false, precision: 10, scale: 2),
                        FinePaid = c.Decimal(nullable: false, precision: 10, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId)
                .ForeignKey("dbo.Libraries", t => t.LibraryId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.BookId)
                .Index(t => t.LibraryId);

            CreateTable(
                "dbo.Libraries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Location = c.String(nullable: false),
                        OperatingHours = c.String(),
                        ContactEmail = c.String(),
                        ContactPhone = c.String(),
                        Description = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Email = c.String(nullable: false, maxLength: 256),
                        PasswordHash = c.String(nullable: false),
                        Role = c.Int(nullable: false),
                        Phone = c.String(),
                        Address = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true, name: "IX_User_Email");

            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.BookId);

            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        ReservedAt = c.DateTime(nullable: false),
                        ExpiresAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.BookId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Reservations", "UserId", "dbo.Users");
            DropForeignKey("dbo.Reservations", "BookId", "dbo.Books");
            DropForeignKey("dbo.Feedbacks", "UserId", "dbo.Users");
            DropForeignKey("dbo.Feedbacks", "BookId", "dbo.Books");
            DropForeignKey("dbo.BorrowTransactions", "UserId", "dbo.Users");
            DropForeignKey("dbo.BorrowTransactions", "LibraryId", "dbo.Libraries");
            DropForeignKey("dbo.Books", "LibraryId", "dbo.Libraries");
            DropForeignKey("dbo.BorrowTransactions", "BookId", "dbo.Books");
            DropForeignKey("dbo.Books", "BorrowSettingsId", "dbo.BorrowSettings");
            DropForeignKey("dbo.BookGenres", "GenreId", "dbo.Genres");
            DropForeignKey("dbo.BookGenres", "BookId", "dbo.Books");
            DropForeignKey("dbo.BookAuthors", "BookId", "dbo.Books");
            DropForeignKey("dbo.BookAuthors", "AuthorId", "dbo.Authors");
            DropIndex("dbo.Reservations", new[] { "BookId" });
            DropIndex("dbo.Reservations", new[] { "UserId" });
            DropIndex("dbo.Feedbacks", new[] { "BookId" });
            DropIndex("dbo.Feedbacks", new[] { "UserId" });
            DropIndex("dbo.Users", "IX_User_Email");
            DropIndex("dbo.BorrowTransactions", new[] { "LibraryId" });
            DropIndex("dbo.BorrowTransactions", new[] { "BookId" });
            DropIndex("dbo.BorrowTransactions", new[] { "UserId" });
            DropIndex("dbo.Genres", "IX_Genre_Name");
            DropIndex("dbo.BookGenres", new[] { "GenreId" });
            DropIndex("dbo.BookGenres", new[] { "BookId" });
            DropIndex("dbo.Books", new[] { "BorrowSettingsId" });
            DropIndex("dbo.Books", "IX_Book_Isbn");
            DropIndex("dbo.Books", new[] { "LibraryId" });
            DropIndex("dbo.BookAuthors", new[] { "AuthorId" });
            DropIndex("dbo.BookAuthors", new[] { "BookId" });
            DropTable("dbo.Reservations");
            DropTable("dbo.Feedbacks");
            DropTable("dbo.Users");
            DropTable("dbo.Libraries");
            DropTable("dbo.BorrowTransactions");
            DropTable("dbo.BorrowSettings");
            DropTable("dbo.Genres");
            DropTable("dbo.BookGenres");
            DropTable("dbo.Books");
            DropTable("dbo.BookAuthors");
            DropTable("dbo.Authors");
        }
    }
}
