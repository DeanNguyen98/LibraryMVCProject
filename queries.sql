  -- Declare all variables upfront
  DECLARE @LibraryId INT;
  DECLARE @AuthorZafon INT, @AuthorIshiguro INT, @AuthorKalanithi INT, @AuthorEco INT;
  DECLARE @AuthorBackman INT, @AuthorAllende INT, @AuthorClarke INT, @AuthorOsman INT;
  DECLARE @GenreMystery INT, @GenreClassic INT, @GenreBiography INT;
  DECLARE @GenreHistorical INT, @GenreFiction INT, @GenreFantasy INT;
  DECLARE @BookShadow INT, @BookRemains INT, @BookBreath INT, @BookRose INT;
  DECLARE @BookOve INT, @BookSpirits INT, @BookPiranesi INT, @BookThursday INT;

  -- 1. Library
  INSERT INTO Libraries (Name, Location, OperatingHours, ContactEmail, ContactPhone, Description, CreatedAt)
  VALUES ('Sydney Library', 'Sydney, NSW', 'Mon-Fri 9am-6pm', 'library@sydney.edu.au', '02 9000 0000', 'Main campus
  library', GETUTCDATE());
  SET @LibraryId = SCOPE_IDENTITY();

  -- 2. Borrow Settings
  INSERT INTO BorrowSettings (SettingName, LoanDurationDays, RenewalLimit, OverdueFinePerDay,
  MaxBorrowableItems, Status, UpdatedAt)
  VALUES ('Standard', 14, 2, 0.10, 5, 'Active', GETUTCDATE());

  -- 3. Authors
  INSERT INTO Authors (Name, Bio) VALUES ('Carlos Ruiz Zafon', NULL);  SET @AuthorZafon = SCOPE_IDENTITY();
  INSERT INTO Authors (Name, Bio) VALUES ('Kazuo Ishiguro', NULL);     SET @AuthorIshiguro = SCOPE_IDENTITY();
  INSERT INTO Authors (Name, Bio) VALUES ('Paul Kalanithi', NULL);     SET @AuthorKalanithi = SCOPE_IDENTITY();
  INSERT INTO Authors (Name, Bio) VALUES ('Umberto Eco', NULL);        SET @AuthorEco = SCOPE_IDENTITY();
  INSERT INTO Authors (Name, Bio) VALUES ('Fredrik Backman', NULL);    SET @AuthorBackman = SCOPE_IDENTITY();
  INSERT INTO Authors (Name, Bio) VALUES ('Isabel Allende', NULL);     SET @AuthorAllende = SCOPE_IDENTITY();
  INSERT INTO Authors (Name, Bio) VALUES ('Susanna Clarke', NULL);     SET @AuthorClarke = SCOPE_IDENTITY();
  INSERT INTO Authors (Name, Bio) VALUES ('Richard Osman', NULL);      SET @AuthorOsman = SCOPE_IDENTITY();

  -- 4. Genres
  INSERT INTO Genres (Name) VALUES ('Mystery');           SET @GenreMystery = SCOPE_IDENTITY();
  INSERT INTO Genres (Name) VALUES ('Classic');           SET @GenreClassic = SCOPE_IDENTITY();
  INSERT INTO Genres (Name) VALUES ('Biography');         SET @GenreBiography = SCOPE_IDENTITY();
  INSERT INTO Genres (Name) VALUES ('Historical Fiction'); SET @GenreHistorical = SCOPE_IDENTITY();
  INSERT INTO Genres (Name) VALUES ('Fiction');           SET @GenreFiction = SCOPE_IDENTITY();
  INSERT INTO Genres (Name) VALUES ('Fantasy');           SET @GenreFantasy = SCOPE_IDENTITY();

  -- 5. Books
  INSERT INTO Books (LibraryId, Title, Isbn, Summary, TotalCopies, AvailableCopies, BorrowedTimes, CreatedAt)
  VALUES (@LibraryId, 'The Shadow of the Wind', '9781594200229', 'A boy discovers a mysterious novel in a secret library
   and becomes obsessed with finding its forgotten author.', 2, 2, 8, GETUTCDATE());
  SET @BookShadow = SCOPE_IDENTITY();

  INSERT INTO Books (LibraryId, Title, Isbn, Summary, TotalCopies, AvailableCopies, BorrowedTimes, CreatedAt)
  VALUES (@LibraryId, 'The Remains of the Day', '9780679731726', 'An English butler reflects on his years of service and
   the choices that shaped his life.', 2, 2, 5, GETUTCDATE());
  SET @BookRemains = SCOPE_IDENTITY();

  INSERT INTO Books (LibraryId, Title, Isbn, Summary, TotalCopies, AvailableCopies, BorrowedTimes, CreatedAt)
  VALUES (@LibraryId, 'When Breath Becomes Air', '9780812988406', 'A neurosurgeon faces his mortality after being
  diagnosed with terminal cancer.', 1, 0, 7, GETUTCDATE());
  SET @BookBreath = SCOPE_IDENTITY();

  INSERT INTO Books (LibraryId, Title, Isbn, Summary, TotalCopies, AvailableCopies, BorrowedTimes, CreatedAt)
  VALUES (@LibraryId, 'The Name of the Rose', '9780156001311', 'A medieval monk investigates a series of mysterious
  deaths at a remote Italian abbey.', 2, 2, 4, GETUTCDATE());
  SET @BookRose = SCOPE_IDENTITY();

  INSERT INTO Books (LibraryId, Title, Isbn, Summary, TotalCopies, AvailableCopies, BorrowedTimes, CreatedAt)
  VALUES (@LibraryId, 'A Man Called Ove', '9781476738024', 'A grumpy widower finds his solitary life disrupted by a
  boisterous new family next door.', 2, 0, 9, GETUTCDATE());
  SET @BookOve = SCOPE_IDENTITY();

  INSERT INTO Books (LibraryId, Title, Isbn, Summary, TotalCopies, AvailableCopies, BorrowedTimes, CreatedAt)
  VALUES (@LibraryId, 'The House of the Spirits', '9780553383805', 'Four generations of a Latin American family navigate
   love, politics, and supernatural forces.', 2, 2, 6, GETUTCDATE());
  SET @BookSpirits = SCOPE_IDENTITY();

  INSERT INTO Books (LibraryId, Title, Isbn, Summary, TotalCopies, AvailableCopies, BorrowedTimes, CreatedAt)
  VALUES (@LibraryId, 'Piranesi', '9781635575637', 'A man lives alone in a surreal house of infinite halls filled with
  statues and tidal seas.', 2, 2, 3, GETUTCDATE());
  SET @BookPiranesi = SCOPE_IDENTITY();

  INSERT INTO Books (LibraryId, Title, Isbn, Summary, TotalCopies, AvailableCopies, BorrowedTimes, CreatedAt)
  VALUES (@LibraryId, 'The Thursday Murder Club', '9781984880963', 'Four retirees in a quiet village meet weekly to
  investigate cold cases until a real murder occurs.', 2, 0, 11, GETUTCDATE());
  SET @BookThursday = SCOPE_IDENTITY();

  -- 6. BookAuthors
  INSERT INTO BookAuthors (BookId, AuthorId) VALUES (@BookShadow, @AuthorZafon);
  INSERT INTO BookAuthors (BookId, AuthorId) VALUES (@BookRemains, @AuthorIshiguro);
  INSERT INTO BookAuthors (BookId, AuthorId) VALUES (@BookBreath, @AuthorKalanithi);
  INSERT INTO BookAuthors (BookId, AuthorId) VALUES (@BookRose, @AuthorEco);
  INSERT INTO BookAuthors (BookId, AuthorId) VALUES (@BookOve, @AuthorBackman);
  INSERT INTO BookAuthors (BookId, AuthorId) VALUES (@BookSpirits, @AuthorAllende);
  INSERT INTO BookAuthors (BookId, AuthorId) VALUES (@BookPiranesi, @AuthorClarke);
  INSERT INTO BookAuthors (BookId, AuthorId) VALUES (@BookThursday, @AuthorOsman);

  -- 7. BookGenres
  INSERT INTO BookGenres (BookId, GenreId) VALUES (@BookShadow, @GenreMystery);
  INSERT INTO BookGenres (BookId, GenreId) VALUES (@BookRemains, @GenreClassic);
  INSERT INTO BookGenres (BookId, GenreId) VALUES (@BookBreath, @GenreBiography);
  INSERT INTO BookGenres (BookId, GenreId) VALUES (@BookRose, @GenreMystery);
  INSERT INTO BookGenres (BookId, GenreId) VALUES (@BookRose, @GenreHistorical);
  INSERT INTO BookGenres (BookId, GenreId) VALUES (@BookOve, @GenreFiction);
  INSERT INTO BookGenres (BookId, GenreId) VALUES (@BookSpirits, @GenreHistorical);
  INSERT INTO BookGenres (BookId, GenreId) VALUES (@BookPiranesi, @GenreFantasy);
  INSERT INTO BookGenres (BookId, GenreId) VALUES (@BookThursday, @GenreMystery);

  -- 8. BorrowTransactions for UserId = 3
  -- Active (Status 0 = Borrowed)
  INSERT INTO BorrowTransactions (UserId, BookId, LibraryId, Status, BorrowedAt, DueDate, ReturnedAt, RenewalsUsed,
  FineAmount, FinePaid)
  VALUES (3, @BookBreath, @LibraryId, 0, '2026-04-29', '2026-05-13', NULL, 0, 0.00, 0.00);

  INSERT INTO BorrowTransactions (UserId, BookId, LibraryId, Status, BorrowedAt, DueDate, ReturnedAt, RenewalsUsed,
  FineAmount, FinePaid)
  VALUES (3, @BookOve, @LibraryId, 0, '2026-05-01', '2026-05-15', NULL, 0, 0.00, 0.00);

  -- Overdue (Status 2 = Overdue)
  INSERT INTO BorrowTransactions (UserId, BookId, LibraryId, Status, BorrowedAt, DueDate, ReturnedAt, RenewalsUsed,
  FineAmount, FinePaid)
  VALUES (3, @BookThursday, @LibraryId, 2, '2026-04-21', '2026-05-05', NULL, 0, 0.80, 0.00);

  -- Returned history (Status 1 = Returned)
  INSERT INTO BorrowTransactions (UserId, BookId, LibraryId, Status, BorrowedAt, DueDate, ReturnedAt, RenewalsUsed,
  FineAmount, FinePaid)
  VALUES (3, @BookShadow, @LibraryId, 1, '2026-03-10', '2026-03-24', '2026-03-22', 0, 0.00, 0.00);

  INSERT INTO BorrowTransactions (UserId, BookId, LibraryId, Status, BorrowedAt, DueDate, ReturnedAt, RenewalsUsed,
  FineAmount, FinePaid)
  VALUES (3, @BookPiranesi, @LibraryId, 1, '2026-04-01', '2026-04-15', '2026-04-18', 0, 0.30, 0.30);
