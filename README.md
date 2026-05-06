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
       connectionString="Data Source=DESKTOP-B3B048I\SQLEXPRESS;Initial Catalog=LibraryManagementSystemDb;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Replace `DESKTOP-B3B048I` with your own machine name.Find it using 
### 5. Run the Database Migration

Open the **Package Manager Console** in Visual Studio:
- Go to **Tools → NuGet Package Manager → Package Manager Console**

Run the following command:

```
Update-Database
```

This will create the `LibraryManagementSystemDb` database on your SQL Server Express instance with all the tables.

## Project Structure

```
LibraryManagementSystem/
├── Controllers/        - MVC controllers (Admin and User namespaces)
├── Data/
│   └── ApplicationDbContext.cs   - database context
├── Models/             - Entity models (database tables)
├── ViewModels/         - ViewModels for passing data to Views
├── Views/              - Razor .cshtml view files
├── Content/            - CSS files
├── Scripts/            - JavaScript files
├── Migrations/         - EF6 migration files (do not edit manually)
└── Web.config          - App configuration (update connection string locally)
```

---

## Workflow

- Never commit directly to `main` — all changes go through Pull Requests
- Branch naming: `feature/{page}-{role}` (e.g. `feature/books-admin`)
- Frontend members own `.cshtml` and CSS only
- Backend members own Controllers and ViewModels only
- Only the team leader runs migrations

---

## Notes

- `Web.config` connection string must be updated to your local machine name
- The `Migrations/` folder is managed by the team leader - do not add or edit migration files manually

---

## Development Guide


Initial merge with main-branch is done. Admin branch now mirrors main branch

1. Commit to github

**If Admin team:**

Run: git checkout Admin
For commit/push to github repo:
Only run git push origin Admin. Do not push to main-branch

**If User team:**

Run:git checkout User
For commit/push to github repo: Only run git push origin User.

2. Development
Main development folders:
Backend:
- /Controller/Admin/ : where you write controllers for each views. For URL prefix we can avoid using the default MVC routing by defining [RoutePrefix(url/example)]
- /ViewModels/Admin/: Where you define the class you will be using to pass into the views and its attribute. Similar to a Model folder, but with more flexibility, allowing you to pass in data from multiple tables


Frontend:
- /Views/Admin: where you define the views for each pages. 
- /Content: store the CSS files. Try to maintain consistency by naming the files as AdminHome.css or AdminBooks.css