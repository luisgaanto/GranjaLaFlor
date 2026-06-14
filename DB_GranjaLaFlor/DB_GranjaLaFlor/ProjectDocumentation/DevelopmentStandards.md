
# Development Standards

## Project: Granja La Flor

### Purpose

This document defines the development standards, architecture, coding conventions, and best practices that must be followed throughout the Granja La Flor project. The objective is to maintain a consistent, secure, maintainable, and scalable codebase while following the official recommendations from Microsoft .NET, Entity Framework Core, and MySQL.

---

# 1. Project Architecture

The application follows the ASP.NET Core MVC architectural pattern with a Service Layer to separate business logic from presentation and data access.

```
User
    ?
    ?
Razor Views
    ?
    ?
Controllers
    ?
    ?
Services
    ?
    ?
ApplicationDbContext
    ?
    ?
Entity Framework Core
    ?
    ?
MySQL Database
```

External systems such as Temperature Monitoring and Early Stimulation servers are accessed through dedicated External Services.

```
Controller
    ?
    ?
Service
    ?
    ?
External Service
    ?
    ?
External API
```

---

# 2. Design Principles

The project follows the following software engineering principles:

* Separation of Concerns (SoC)
* Single Responsibility Principle (SRP)
* Dependency Injection (DI)
* Encapsulation
* Layered Architecture
* Soft Delete Strategy
* Secure by Design

---

# 3. Technology Stack

* ASP.NET Core MVC
* C#
* Razor Views
* Entity Framework Core
* LINQ
* Pomelo.EntityFrameworkCore.MySql
* MySQL 8
* Bootstrap

---

# 4. Folder Structure

```
Controllers
Data
    ??? Context
    ??? Configurations

ExternalServices
    ??? Interfaces
    ??? Implementations

Helpers

Models
    ??? Entities
    ??? ViewModels
    ??? Validations

Services

Views

wwwroot

ProjectDocumentation
```

---

# 5. Entity Design Standards

All entities must:

* Use singular class names.
* Map explicitly to database tables.
* Use DataAnnotations.
* Use PascalCase for C# properties.
* Map snake_case database columns using the Column attribute.

Example:

* Table: roles
* Entity: Role
* Property: RoleName
* Column: role_name

---

# 6. DataAnnotations

DataAnnotations are the primary configuration mechanism.

The following attributes should be used whenever applicable:

* Table
* Column
* Key
* Required
* Display
* StringLength
* Range
* EmailAddress
* Phone
* RegularExpression

Display attributes should be used to provide user-friendly labels for Razor views.

---

# 7. Fluent API

Fluent API is reserved only for configurations that cannot be represented using DataAnnotations, including:

* Complex relationships
* Composite indexes
* Cascade delete behavior
* Composite keys
* Advanced database configuration

DataAnnotations remain the preferred approach throughout the project.

---

# 8. Controllers

Controllers must remain lightweight.

Controllers are responsible only for:

* Receiving HTTP requests.
* Validating ModelState.
* Calling Services.
* Returning Views or HTTP responses.

Controllers must never access ApplicationDbContext directly.

---

# 9. Services

Business logic belongs exclusively inside Services.

Responsibilities include:

* Database operations
* Calculations
* Business rules
* Validations
* Communication with external systems

---

# 10. Database

The MySQL database is considered the source of truth.

Database tables are created manually.

Entity Framework Core is used only as the ORM.

Database scaffolding will not be used.

---

# 11. Security Standards

The project follows a secure-by-design approach.

Security measures include:

* HTTPS
* Authentication
* Role-based Authorization
* Password hashing using BCrypt
* Anti-Forgery Tokens
* Soft Delete
* Input validation
* Parameterized LINQ queries
* Secure communication with external APIs

---

# 12. Coding Standards

* PascalCase for classes, methods, and properties.
* camelCase for local variables.
* Meaningful names.
* Explicit access modifiers.
* private by default.
* public only when required.
* Async methods should end with Async.
* Avoid duplicated code.
* Keep methods focused on a single responsibility.

---

# 13. Documentation

Comments should explain why the code exists rather than what the code is doing.

Example:

Good:

```csharp
// Soft Delete preserves historical information by marking records as inactive.
```

Avoid:

```csharp
// Create variable.
```

---

# 14. Official Documentation

The project follows the official documentation from Microsoft and MySQL.

Primary references include:

* ASP.NET Core Fundamentals
* ASP.NET Core Best Practices
* ASP.NET Core Security
* Entity Framework Core Documentation
* C# Programming Guide
* .NET Coding Conventions
* MySQL 8.0 Reference Manual

All architectural and implementation decisions should remain aligned with these official sources whenever possible.



