# 🐞 Bug & Issue Tracker – Backend Plan

A full-stack **Angular + .NET 8 + SQLite** project for tracking bugs, feature requests, feature requests, and improvements.

This document outlines the **database schema**, **REST API endpoints**, and **implementation plan** for the backend.

---

# 📂 1. Database Schema (SQLite)

## Users

```sql
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Role TEXT NOT NULL, -- Admin, Developer, Reporter
    AvatarUrl TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

---

## Projects

```sql
CREATE TABLE Projects (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

---

## ProjectMembers (Many-to-Many: Users ↔ Projects)

```sql
CREATE TABLE ProjectMembers (
    ProjectId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    PRIMARY KEY (ProjectId, UserId),
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
```

---

## Issues

```sql
CREATE TABLE Issues (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectId INTEGER NOT NULL,
    Title TEXT NOT NULL,
    Description TEXT,
    Priority TEXT NOT NULL, -- Low, Medium, High, Critical
    Status TEXT NOT NULL, -- Open, In Progress, Resolved, Closed
    Type TEXT NOT NULL, -- Bug, Feature, Improvement
    AssigneeId INTEGER,
    ReporterId INTEGER NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (AssigneeId) REFERENCES Users(Id),
    FOREIGN KEY (ReporterId) REFERENCES Users(Id)
);
```

---

## Comments

```sql
CREATE TABLE Comments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    IssueId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    Content TEXT NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (IssueId) REFERENCES Issues(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
```

---

## Attachments

```sql
CREATE TABLE Attachments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    IssueId INTEGER NOT NULL,
    FilePath TEXT NOT NULL,
    UploadedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (IssueId) REFERENCES Issues(Id) ON DELETE CASCADE
);
```

---

## AuditLogs

```sql
CREATE TABLE AuditLogs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EntityType TEXT NOT NULL,
    EntityId INTEGER NOT NULL,
    Action TEXT NOT NULL,
    OldValue TEXT,
    NewValue TEXT,
    ChangedBy INTEGER NOT NULL,
    ChangedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ChangedBy) REFERENCES Users(Id)
);
```

---

# 🌐 2. REST API Endpoints

## 🔐 Authentication

| Method | Endpoint             | Description                      |
| ------ | -------------------- | -------------------------------- |
| POST   | `/api/auth/register` | Register a new user              |
| POST   | `/api/auth/login`    | Authenticate user and return JWT |
| GET    | `/api/auth/me`       | Get current authenticated user   |

---

## 👤 Users

| Method | Endpoint          | Description                   |
| ------ | ----------------- | ----------------------------- |
| GET    | `/api/users`      | List all users _(Admin only)_ |
| GET    | `/api/users/{id}` | Get user details              |
| PUT    | `/api/users/{id}` | Update user profile           |
| DELETE | `/api/users/{id}` | Delete user _(Admin only)_    |

---

## 📁 Projects

| Method | Endpoint             | Description                   |
| ------ | -------------------- | ----------------------------- |
| GET    | `/api/projects`      | List all projects             |
| GET    | `/api/projects/{id}` | Get project details           |
| POST   | `/api/projects`      | Create project _(Admin only)_ |
| PUT    | `/api/projects/{id}` | Update project                |
| DELETE | `/api/projects/{id}` | Delete project                |

---

## 👥 Project Members

| Method | Endpoint                              | Description                |
| ------ | ------------------------------------- | -------------------------- |
| POST   | `/api/projects/{id}/members`          | Add member to project      |
| DELETE | `/api/projects/{id}/members/{userId}` | Remove member from project |

---

## 🐞 Issues

| Method | Endpoint                           | Description               |
| ------ | ---------------------------------- | ------------------------- |
| GET    | `/api/projects/{projectId}/issues` | List issues for a project |
| GET    | `/api/issues/{id}`                 | Get issue details         |
| POST   | `/api/projects/{projectId}/issues` | Create issue              |
| PUT    | `/api/issues/{id}`                 | Update issue              |
| DELETE | `/api/issues/{id}`                 | Delete issue              |

---

## 💬 Comments

| Method | Endpoint                         | Description    |
| ------ | -------------------------------- | -------------- |
| GET    | `/api/issues/{issueId}/comments` | List comments  |
| POST   | `/api/issues/{issueId}/comments` | Add comment    |
| DELETE | `/api/comments/{id}`             | Delete comment |

---

## 📎 Attachments

| Method | Endpoint                            | Description       |
| ------ | ----------------------------------- | ----------------- |
| POST   | `/api/issues/{issueId}/attachments` | Upload attachment |
| GET    | `/api/issues/{issueId}/attachments` | List attachments  |
| DELETE | `/api/attachments/{id}`             | Delete attachment |

---

## 📜 Audit Logs

| Method | Endpoint                             | Description                        |
| ------ | ------------------------------------ | ---------------------------------- |
| GET    | `/api/audit`                         | List all audit logs _(Admin only)_ |
| GET    | `/api/audit/{entityType}/{entityId}` | View audit history for an entity   |

---

# ⚙️ 3. Implementation Notes

### Entity Framework Core

- Use **Entity Framework Core** with a single `ApplicationDbContext`.
- Create a `DbSet<T>` for each entity:
  - Users
  - Projects
  - ProjectMembers
  - Issues
  - Comments
  - Attachments
  - AuditLogs

---

### Database Migrations

Create the initial migration:

```bash
dotnet ef migrations add InitialCreate
```

Apply the migration:

```bash
dotnet ef database update
```

---

### Authentication

Use JWT authentication:

- `Microsoft.AspNetCore.Authentication.JwtBearer`
- Password hashing using **ASP.NET Core Identity PasswordHasher**
- Role-based authorization
  - Admin
  - Developer
  - Reporter

---

### File Uploads

- Store uploaded files in:

```
/uploads
```

- Store only the file path in SQLite.

Example:

```
/uploads/screenshots/login-error.png
```

---

### Audit Logging

Automatically log:

- Entity Created
- Entity Updated
- Entity Deleted
- User who made the change
- Timestamp
- Old values
- New values

This can be implemented using:

- EF Core SaveChanges override
- Repository pattern
- Middleware
- Interceptors

---

# 🚀 Project Structure

```
BugTracker.Api
│
├── Controllers
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── ProjectsController.cs
│   ├── IssuesController.cs
│   ├── CommentsController.cs
│   ├── AttachmentsController.cs
│   └── AuditController.cs
│
├── Data
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
│
├── Models
│   ├── User.cs
│   ├── Project.cs
│   ├── ProjectMember.cs
│   ├── Issue.cs
│   ├── Comment.cs
│   ├── Attachment.cs
│   └── AuditLog.cs
│
├── DTOs
│
├── Services
│
├── Repositories
│
├── Middleware
│
├── uploads
│
└── Program.cs
```

---

# 🔒 Security

- JWT Authentication
- Role-Based Authorization
- Password Hashing
- Input Validation
- File Upload Validation
- CORS Configuration
- HTTPS Enforcement

---

# 📦 Technologies

| Technology            | Version |
| --------------------- | ------- |
| .NET                  | 8       |
| ASP.NET Core Web API  | 8       |
| Entity Framework Core | 8       |
| SQLite                | Latest  |
| Angular               | 20      |
| JWT Authentication    | ✔       |
| Swagger/OpenAPI       | ✔       |

---

# 📌 Next Steps

- [ ] Create EF Core model classes
- [ ] Configure `ApplicationDbContext`
- [ ] Generate the initial migration
- [ ] Seed default admin account
- [ ] Implement JWT authentication
- [ ] Build REST controllers
- [ ] Implement repository/service layer
- [ ] Add audit logging
- [ ] Configure Swagger
- [ ] Connect Angular frontend
- [ ] Add Docker support
- [ ] Deploy backend API

---

# 🎯 Future Enhancements

- Email notifications
- Issue labels/tags
- Search and filtering
- Issue history timeline
- User mentions (`@username`)
- Markdown support in comments
- Dark mode UI
- WebSocket real-time updates
- Azure Blob Storage / AWS S3 attachments
- CI/CD with GitHub Actions
- Unit & Integration Testing
- Redis caching
- PostgreSQL support
- Multi-tenant workspaces

```

```
