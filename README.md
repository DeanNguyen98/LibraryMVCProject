# Library Management System

ASP.NET MVC 5 (.NET Framework 4.7.2) web application for managing library books, members, borrowing, and reservations.

---

## Prerequisites

Make sure you have the following installed before setting up the project:

- [Visual Studio 2019 or later](https://visualstudio.microsoft.com/)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- .NET Framework 4.7.2

---

## Setup

### 1. Clone the Repository

```
git clone https://github.com/DeanNguyen98/LibraryMVCProject.git
```

### 2. Open the Solution

Open `LibraryManagementSystem.sln` in Visual Studio.

### 3. Restore NuGet Packages

Visual Studio should restore packages automatically. If not:

- Right-click the solution in Solution Explorer
- Select **Restore NuGet Packages**

### 4. Update the Connection String

Open `LibraryManagementSystem/Web.config` and find the `<connectionStrings>` section:

```xml
<connectionStrings>
  <add name="DefaultConnection"
       connectionString="Data Source=YOUR-MACHINE-NAME\SQLEXPRESS;Initial Catalog=LibraryManagementSystemDb;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Replace `YOUR-MACHINE-NAME` with your own machine name. You can find it by running `hostname` in Command Prompt.

### 5. Run the Database Migration

Open the **Package Manager Console** in Visual Studio:

- Go to **Tools → NuGet Package Manager → Package Manager Console**

Run the following command:

```
Update-Database
```

This creates the `LibraryManagementSystemDb` database on your local SQL Server Express with all required tables.

### 6. Run the Application

Press **F5** in Visual Studio (or click the green **IIS Express** play button). The app will open in your browser and redirect you to the sign in page automatically.

---

## Accounts

### Admin Account

For the purpose of this assignment, an admin account is created automatically every time the application starts up (if no admin exists yet). No manual database setup is required.

| Field    | Value               |
|----------|---------------------|
| Email    | admin@library.com   |
| Password | Admin@123           |

To sign in as admin, go to the sign in page and click **Login as Admin**, or navigate directly to `/Auth/AdminSignIn`.

### Member Account

To create a member account, go to `/Auth/SignIn` and click **Sign Up**. Fill in your name, email, and password — you will be logged in automatically after registering.

---

## Navigating the Application

### Admin Pages

| Page            | URL                    | Description                                      |
|-----------------|------------------------|--------------------------------------------------|
| Home            | `/Admin/Home`          | Dashboard with overview stats                    |
| Library         | `/Admin/Library`       | Manage library branches                          |
| Books           | `/Admin/Books`         | Add, edit, and delete books                      |
| Borrow Settings | `/Admin/BorrowSettings`| Configure loan duration, limits, and fine rates  |
| Transactions    | `/Admin/Transactions`  | View all active and overdue borrows across users  |
| Feedback        | `/Admin/Feedback`      | View member book reviews                         |

### Member Pages

| Page            | URL                  | Description                                      |
|-----------------|----------------------|--------------------------------------------------|
| Home            | `/User/Home`         | Landing page with trending books and stats       |
| Books           | `/User/Books`        | Browse, borrow, and reserve books                |
| My Transactions | `/User/Transactions` | View active borrows, overdue items, and history  |

---

## Project Structure

```
LibraryManagementSystem/
├── Controllers/
│   ├── Admin/          - Admin controllers (require Admin role)
│   ├── User/           - User/Member controllers (require Member role)
│   └── AuthController  - Sign in, sign up, sign out
├── Data/
│   └── ApplicationDbContext.cs   - EF6 database context
├── Helpers/
│   └── PasswordHelper.cs         - PBKDF2 password hashing and verification
├── Models/             - Entity models mapped to database tables
├── ViewModels/
│   ├── Admin/          - ViewModels for admin views
│   ├── User/           - ViewModels for user/member views
│   └── Auth/           - ViewModels for sign in and sign up forms
├── Views/
│   ├── Admin/          - Razor views for admin pages
│   ├── User/           - Razor views for member pages
│   ├── Auth/           - Sign in, sign up, and admin sign in pages
│   └── Shared/         - Layout and shared partials
├── Content/            - CSS files (one file per page)
├── Scripts/            - JavaScript and jQuery files
├── Migrations/         - EF6 migration files (managed by team leader only)
└── Web.config          - App configuration (update connection string locally)
```

---

## Team Workflow

- Never commit directly to `main` — all changes go through Pull Requests
- Branch naming: `feature/{page}-{role}` (e.g. `feature/books-admin`)
- Frontend members own `.cshtml` and CSS only
- Backend members own Controllers and ViewModels only
- Only the team leader runs migrations

### Branch Setup

**Admin team:**
```
git checkout Admin
git push origin Admin
```

**User team:**
```
git checkout User
git push origin User
```

Do not push directly to `main-branch`.

### Key Development Folders

**Backend:**
- `/Controllers/Admin/` — admin controllers. Use `[RoutePrefix("Admin/PageName")]` on each controller
- `/Controllers/User/` — member controllers. Use `[RoutePrefix("User/PageName")]` on each controller
- `/ViewModels/Admin/` — ViewModels for admin views
- `/ViewModels/User/` — ViewModels for member views

**Frontend:**
- `/Views/Admin/` — admin Razor views
- `/Views/User/` — member Razor views
- `/Content/` — CSS files, named per page (e.g. `AdminBooks.css`, `UserHome.css`)

---

## Notes

- `Web.config` connection string must be updated to your local machine name before running
- The `Migrations/` folder is managed by the team leader — do not add or edit migration files manually
- Passwords are hashed using PBKDF2 (100,000 iterations) via `Helpers/PasswordHelper.cs`
- The admin account seeder in `Global.asax.cs` only runs if no admin user exists, so it is safe to leave in place
