
# Development Standards

## Project: Granja La Flor

### Purpose

This document defines the development standards, architecture, coding conventions, and best practices that must be followed throughout the Granja La Flor project. The objective is to maintain a consistent, secure, maintainable, and scalable codebase while following the official recommendations from Microsoft .NET, Entity Framework Core, and MySQL.

---

# 1. Project Architecture

The application follows the ASP.NET Core MVC architectural pattern with a Service Layer to separate business logic from presentation and data access.

```
User
    │
    ▼
Razor Views
    │
    ▼
Controllers
    │
    ▼
Services
    │
    ▼
ApplicationDbContext
    │
    ▼
Entity Framework Core
    │
    ▼
MySQL Database
```

External systems such as Temperature Monitoring and Early Stimulation servers are accessed through dedicated External Services.

```
Controller
    │
    ▼
Service
    │
    ▼
External Service
    │
    ▼
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
    ├── Context
    └── Configurations

ExternalServices
    ├── Interfaces
    └── Implementations

Helpers

Models
    ├── Entities
    ├── ViewModels
    └── Validations

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



++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


DevelopmentStandards.md
1. Project Structure

Purpose

The project follows a layered architecture based on the ASP.NET Core MVC framework. 
Each layer has a well-defined responsibility to promote maintainability, readability,
and scalability throughout the application.

The project shall be organized using the following directory structure:

Controllers/
Data/
    Context/
Models/
    Entities/
    ViewModels/      (Only when required)
ProjectDocumentation/
Services/
Views/
wwwroot/

Responsibilities:

1. Controllers
Receive and process HTTP requests.
Validate incoming data.
Delegate business logic to the corresponding Service.
Return Razor Views or Redirect responses.
Must not contain business logic.

2. Services

Implement business rules.
Communicate with the database through ApplicationDbContext.
Must not return Views

3. Models/Entities

Represent database tables.
Contain Data Annotations for validation.
Remain independent of presentation logic.

4. Models/ViewModels
Represent data required exclusively by a View.
Shall only be created when the View requires information different from the Entity model (e.g., Login, Dashboard, Reports).

5. Views

Display information to the user.
Contain presentation logic only.
Must not implement business logic or database access.

6. wwwroot

Stores static resources such as:

CSS
JavaScript
Images
Fonts
Benefits

This organization provides:

Clear separation of responsibilities.
Lower coupling between components.
Improved maintainability.
Easier scalability.
Consistent project organization.


+++   +++   +++   +++   +++   +++

Official References:
ASP.NET Core MVC Overview

https://learn.microsoft.com/aspnet/core/mvc/overview

ASP.NET Core Project Structure

https://learn.microsoft.com/aspnet/core/mvc/overview

2. Naming Conventions

Purpose

To ensure consistency, readability, and maintainability throughout the project, all source code shall follow 
a standardized naming convention based on the Microsoft C# Coding Conventions and ASP.NET Core development guidelines.

Using consistent naming conventions improves code readability, facilitates collaboration, and simplifies long-term maintenance.



Official References
C# Coding Conventions

https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions

Common C# Code Conventions

https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names

ASP.NET Core MVC Overview

https://learn.microsoft.com/aspnet/core/mvc/overview


3. Controllers

Purpose

Controllers are responsible for handling HTTP requests and acting as the communication layer between the user interface and
the business logic. Their primary responsibility is to coordinate the execution flow without implementing business rules.

This project follows the Controller implementation recommended by ASP.NET Core MVC, where Controllers remain lightweight and 
delegate business operations to the corresponding Service layer.

Standard

Every Controller shall inherit from the ASP.NET Core Controller base class.

Example:

public class RolesController : Controller
{
}

Action Methods

Action methods shall clearly represent the requested operation.

Example:

Index()
Details()
Create()
Edit()
Delete()
Inactive()
Activate()

Whenever an Action performs asynchronous operations, it shall:

Return Task<IActionResult>.
Use the async keyword.
Await asynchronous Service methods.

Example:

public async Task<IActionResult> Index()
{
    var activeRoles = await _roleService.GetAllActiveAsync();

    return View(activeRoles);
}


HTTP Verbs

Controllers shall explicitly indicate the supported HTTP method.

Examples:

[HttpGet]
[HttpPost]

Actions responsible for modifying data shall only accept HTTP POST requests.

Anti-Forgery Protection

Every POST Action shall include anti-forgery validation.

Example:

[HttpPost]
[ValidateAntiForgeryToken]

This protects the application against Cross-Site Request Forgery (CSRF) attacks.

Model Validation

Controllers shall validate incoming data before calling the Service layer.

Example:

if (!ModelState.IsValid)
{
    return View(role);
}

Business operations shall only continue when ModelState.IsValid evaluates to true.

Logging

Controllers shall log relevant events using ILogger<T>.

Recommended events include:

Entering an Action.
Validation failures.
Successful operations.
Entity not found.
Unexpected exceptions.

Example:

_logger.LogInformation(
    "Entering Index().");
User Notifications

Controllers shall use TempData to display notifications after a successful redirect.

Example:

TempData["SuccessMessage"] =
    "Role created successfully.";

Unexpected errors shall generate an error notification.

Example:

TempData["ErrorMessage"] =
    "An unexpected error occurred while processing the request.";
Exception Handling

Business operations that interact with external resources (such as the database) shall be enclosed within try/catch blocks.

Example:

try
{
    await _roleService.CreateAsync(role);

    TempData["SuccessMessage"] =
        "Role created successfully.";

    return RedirectToAction(nameof(Index));
}
catch (Exception ex)
{
    _logger.LogError(
        ex,
        "Unexpected error while creating Role.");

    TempData["ErrorMessage"] =
        "An unexpected error occurred.";

    return View(role);
}
Benefits

Following these standards provides:

Thin Controllers.
Separation of Concerns (SoC).
Centralized business logic.
Improved maintainability.
Consistent error handling.
Improved diagnostics through logging.
Better application security.
Official References
Controllers in ASP.NET Core MVC

https://learn.microsoft.com/aspnet/core/mvc/controllers/actions

Dependency Injection

https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection

Model Validation

https://learn.microsoft.com/aspnet/core/mvc/models/validation

Logging

https://learn.microsoft.com/aspnet/core/fundamentals/logging

Prevent Cross-Site Request Forgery (CSRF)

https://learn.microsoft.com/aspnet/core/security/anti-request-forgery

4. Services

Purpose

The Service Layer is responsible for implementing the application's business logic. It acts as an intermediary layer between the Controllers and the data access layer, ensuring that business rules remain centralized and independent from the presentation layer.

This project adopts a dedicated Service Layer to promote Separation of Concerns (SoC), reduce coupling, and improve the maintainability of the application.

Standard

Each application module shall implement its own Service class.

Examples:

RoleService
UserService
MortalityService
FeedService
WeeklyControlService

Every Service shall encapsulate the business rules related to a single functional module.

Responsibilities

Service classes shall be responsible for:

Implementing business rules.
Querying the database.
Creating, updating, activating, and deactivating entities.
Performing business validations.
Coordinating database operations.
Returning Entities or collections of Entities.

Service classes shall not:

Return Views.
Access TempData.
Generate HTTP responses.
Access HttpContext.
Contain Razor or UI logic.
Dependency Injection

Services shall receive their dependencies using Constructor Injection.

Example:

private readonly ApplicationDbContext _context;

public RoleService(ApplicationDbContext context)
{
    _context = context;
}

The required dependencies are automatically resolved through the ASP.NET Core Dependency Injection container.

Database Access

All communication with the database shall be performed through Entity Framework Core using ApplicationDbContext.

Example:

return await _context.Roles
    .AsNoTracking()
    .Where(role => role.RoleState)
    .ToListAsync();

Controllers shall never access ApplicationDbContext directly.

Query Methods

Methods that only retrieve information shall:

Be asynchronous.
Use LINQ Method Syntax.
Use AsNoTracking() whenever entities are not modified.
Return strongly typed objects.

Examples:

GetAllAsync()

GetAllActiveAsync()

GetAllInactiveAsync()

GetByIdAsync()
Command Methods

Methods that modify data shall:

Be asynchronous.
Validate business rules before saving.
Persist changes using Entity Framework Core.
Call SaveChangesAsync().

Examples:

CreateAsync()

UpdateAsync()

SoftDeleteAsync()

ActivateAsync()
Soft Delete

Physical deletion of records is prohibited.

Instead, inactive records shall be marked using the corresponding Boolean status property.

Example:

role.RoleState = false;

Services shall also provide methods to restore inactive records.

Asynchronous Programming

All database operations shall use asynchronous methods.

Examples:

await ToListAsync();

await FirstOrDefaultAsync();

await SaveChangesAsync();

This improves scalability by preventing unnecessary thread blocking while waiting for database operations to complete.

Separation of Responsibilities

The following responsibilities have been defined for the project.

Layer	Responsibility
Controller	HTTP Requests and Responses
Service	Business Logic
DbContext	Database Access
Entity Framework Core	ORM
MySQL	Data Storage

Each layer shall communicate only with the immediately adjacent layer.

Benefits

The implementation of a Service Layer provides:

Centralized business logic.
Lower coupling.
Higher maintainability.
Improved code reuse.
Simplified testing.
Cleaner Controllers.
Better scalability.
Official References
Dependency Injection in ASP.NET Core

https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection

Entity Framework Core

https://learn.microsoft.com/ef/core/

Asynchronous Programming with async and await

https://learn.microsoft.com/dotnet/csharp/programming-guide/concepts/async/

Separation of Concerns

https://learn.microsoft.com/azure/architecture/guide/architecture-styles/n-tier

5. Entity Framework Core


Purpose

Entity Framework Core (EF Core) is the Object-Relational Mapper (ORM) adopted by this project to manage communication between the application and the MySQL database.

EF Core provides object-oriented access to relational data, allowing developers to manipulate database records through C# objects instead of writing SQL statements directly.

Standard

The application shall use Entity Framework Core as the exclusive data access technology.

All database operations shall be performed through the application's ApplicationDbContext.

Direct SQL statements shall only be used when Entity Framework Core cannot efficiently support the required functionality.

ApplicationDbContext

The project shall define a single database context responsible for managing all Entity Framework operations.

Example:

public class ApplicationDbContext : DbContext
{
    public DbSet<Role> Roles { get; set; }

    public DbSet<User> Users { get; set; }

    ...
}

The DbContext shall be registered using Dependency Injection.

Example:

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));
Entity Configuration

Database tables shall be represented using Entity classes.

Example:

Role

User

Mortality

Feed

WeeklyControl

Each Entity shall contain:

Properties
Data Annotations
Navigation Properties (when required)

Business logic shall not be implemented inside Entity classes.

Query Standard

The project adopts LINQ Method Syntax as the standard query style.

Example:

return await _context.Roles
    .AsNoTracking()
    .Where(role => role.RoleState)
    .OrderBy(role => role.RoleName)
    .ToListAsync();

Query Syntax may be used only when Method Syntax cannot clearly express the required query.

Read Operations

Queries that retrieve information without modifying entities shall use:

AsNoTracking()

Example:

return await _context.Roles
    .AsNoTracking()
    .Where(role => role.RoleState)
    .ToListAsync();

Using AsNoTracking() improves performance by disabling Entity Framework's change tracking for read-only operations.

Create Operations

New entities shall be added using:

_context.Add(entity);

await _context.SaveChangesAsync();
Update Operations

Existing entities shall be modified using the tracked Entity instance and persisted through:

await _context.SaveChangesAsync();

Explicit calls to Update() should only be used when required.

Delete Operations

Physical deletion shall not be performed.

Instead, the project implements Soft Delete, updating the status property.

Example:

role.RoleState = false;

await _context.SaveChangesAsync();
Asynchronous Operations

All database operations shall use asynchronous methods.

Examples include:

ToListAsync()

FirstOrDefaultAsync()

FindAsync()

SaveChangesAsync()

AnyAsync()

Synchronous database operations should be avoided.

Change Tracking

Entity Framework Change Tracking shall only be enabled when modifications are required.

Read-only queries shall disable tracking using:

AsNoTracking()
Transactions

Whenever a business operation modifies multiple related entities, database transactions should be considered to guarantee data consistency.

Entity Framework Core transactions shall be preferred over manual SQL transactions whenever possible.

Benefits

Using Entity Framework Core provides:

Object-oriented data access.
Reduced SQL code.
Automatic parameterized queries.
Protection against SQL Injection.
Built-in Change Tracking.
LINQ support.
Database portability.
Integration with Dependency Injection.
Project Decisions

The following decisions have been adopted for this project:

Entity Framework Core is the exclusive ORM.
ApplicationDbContext is the only database context.
Controllers never access DbContext directly.
All queries shall be implemented inside the Service Layer.
LINQ Method Syntax is the standard query syntax.
Read-only queries shall use AsNoTracking().
All database operations shall be asynchronous.
Soft Delete is mandatory for CRUD modules.
Official References
Entity Framework Core Overview

https://learn.microsoft.com/ef/core/

DbContext

https://learn.microsoft.com/ef/core/dbcontext-configuration/

Tracking vs. No-Tracking Queries

https://learn.microsoft.com/ef/core/querying/tracking

Asynchronous Programming

https://learn.microsoft.com/ef/core/miscellaneous/async

LINQ

https://learn.microsoft.com/dotnet/csharp/programming-guide/concepts/linq/


6. Data Validation


Purpose

Data validation ensures that all information entered into the application complies with the business rules and data integrity requirements before being processed or persisted in the database.

This project adopts the validation mechanisms provided by ASP.NET Core MVC to perform both server-side and client-side validation using Data Annotations and ModelState.

Standard

All Entity classes shall define validation rules using Data Annotation attributes.

Controllers shall validate incoming models before executing any business operation.

Business logic shall only execute when the submitted model passes validation.

Data Annotations

Validation rules shall be implemented using Data Annotations whenever possible.

Common validation attributes include:

Required

StringLength

MaxLength

MinLength

Range

EmailAddress

Phone

DataType

Display

Example:

[Required]
[StringLength(30)]
public string RoleName { get; set; }
Database Consistency

Validation rules implemented in Entity classes should be consistent with the database schema.

Examples:

Database	Entity
VARCHAR(30)	StringLength(30)
NOT NULL	Required
UNIQUE	Business validation
BOOLEAN	bool

Whenever possible, validation shall be performed before attempting to save data into the database.

Server-side Validation

Controllers shall always validate the model before calling the Service layer.

Example:

if (!ModelState.IsValid)
{
    return View(role);
}

If validation fails:

The View shall be redisplayed.
Validation messages shall be shown.
No database operation shall be executed.
Client-side Validation

Client-side validation shall be enabled using the ASP.NET Core validation scripts.

Example:

@section Scripts
{
    @{
        await Html.RenderPartialAsync("_ValidationScriptsPartial");
    }
}

Client-side validation improves the user experience but shall never replace server-side validation.

Validation Messages

Validation messages shall be displayed using Tag Helpers.

Example:

<span asp-validation-for="RoleName"
      class="text-danger">
</span>

General validation errors shall be displayed using:

<div asp-validation-summary="ModelOnly"
     class="text-danger">
</div>
Business Validation

Some validation rules cannot be expressed using Data Annotations alone.

Examples include:

Duplicate role names.
Duplicate email addresses.
Referential integrity checks.
Active/Inactive status validation.

These rules shall be implemented inside the corresponding Service class.

Validation Flow

The project adopts the following validation sequence:

User Input
      │
      ▼
Client-side Validation
      │
      ▼
Controller
      │
      ▼
ModelState.IsValid
      │
      ▼
Service Layer
      │
      ▼
Entity Framework Core
      │
      ▼
Database

If validation fails at any stage, the request shall return to the corresponding View without modifying the database.

Benefits

The adopted validation strategy provides:

Improved data integrity.
Reduced invalid database operations.
Better user experience.
Consistent validation rules.
Protection against malformed input.
Reduced application errors.
Project Decisions

The following validation standards have been adopted for this project:

Data Annotations are the primary validation mechanism.
Validation rules shall match the database schema whenever possible.
Controllers shall always verify ModelState.IsValid.
Business validation belongs to the Service Layer.
Client-side validation complements but never replaces server-side validation.
Invalid data shall never reach the database.
Official References
Model Validation in ASP.NET Core

https://learn.microsoft.com/aspnet/core/mvc/models/validation

Model Binding

https://learn.microsoft.com/aspnet/core/mvc/models/model-binding

Data Annotations

https://learn.microsoft.com/dotnet/api/system.componentmodel.dataannotations



7. Logging

Purpose

Logging provides a mechanism to record significant events occurring during the execution of the application. These records facilitate troubleshooting, system monitoring, auditing, and software maintenance without affecting the application's business logic.

This project adopts the built-in logging infrastructure provided by ASP.NET Core through the ILogger<T> interface.

Standard

Every Controller shall implement ILogger<T> using Constructor Injection.

Example:

private readonly ILogger<RolesController> _logger;

public RolesController(
    RoleService roleService,
    ILogger<RolesController> logger)
{
    _roleService = roleService;
    _logger = logger;
}

Logging shall be implemented using the logging infrastructure provided by ASP.NET Core.

Responsibilities

Logging shall be used to record application events that are useful for diagnostics and monitoring.

Typical events include:

Entering a Controller Action.
Successful execution of an operation.
Validation failures.
Entity not found.
Unexpected exceptions.
Business rule violations.
Data modifications (Create, Update, Activate and Soft Delete).

Logging shall not be used to replace application logic or user notifications.

Logging Levels

The project adopts the logging levels defined by Microsoft.

Level	Purpose
Trace	Detailed diagnostic information. Rarely enabled in production.
Debug	Information useful during software development.
Information	Successful execution of application operations.
Warning	Unexpected situations that do not interrupt execution.
Error	Errors that prevent the current operation from completing successfully.
Critical	Failures that may stop the application or compromise system availability.

The appropriate logging level shall be selected according to the severity of the event.

Information Logging

Informational logs shall record the beginning and successful completion of relevant operations.

Example:

_logger.LogInformation(
    "Entering Index().");

Example:

_logger.LogInformation(
    "Role '{RoleName}' was created successfully.",
    role.RoleName);

Structured logging shall be preferred over string concatenation.

Warning Logging

Warnings shall be recorded when the requested operation cannot be completed due to expected conditions.

Example:

_logger.LogWarning(
    "Role with ID {RoleId} was not found.",
    id);

Typical scenarios include:

Record not found.
Duplicate data.
Validation failures.
Invalid application state.
Error Logging

Unexpected exceptions shall always be logged.

Example:

catch (Exception ex)
{
    _logger.LogError(
        ex,
        "Unexpected error while creating Role.");

    TempData["ErrorMessage"] =
        "An unexpected error occurred.";

    return View(role);
}

The complete Exception object shall always be passed to the logger.

Structured Logging

Structured logging shall be used whenever variable information is recorded.

Example:

_logger.LogInformation(
    "Updating Role {RoleId}.",
    id);

Avoid:

_logger.LogInformation(
    "Updating Role " + id);

Structured logging improves filtering, searching, and analysis within logging providers.

Sensitive Information

Logs shall never contain sensitive information.

Examples of prohibited data include:

Passwords.
Authentication tokens.
Connection strings.
Personal identification numbers.
Sensitive medical information.

Only information required for diagnostics shall be recorded.

Benefits

The logging strategy adopted by the project provides:

Improved diagnostics.
Easier troubleshooting.
Better monitoring.
Simplified maintenance.
Audit support.
Consistent event registration.
Native integration with ASP.NET Core.
Project Decisions

The following logging standards have been adopted:

Every Controller shall implement ILogger<T>.
Constructor Injection shall be used to obtain the logger.
Every CRUD operation shall generate Information logs.
Unexpected exceptions shall generate Error logs.
Structured logging shall be preferred over string concatenation.
Sensitive information shall never be recorded.
Official References
Logging in ASP.NET Core

https://learn.microsoft.com/aspnet/core/fundamentals/logging

ILogger Interface

https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger

Logging Fundamentals

https://learn.microsoft.com/dotnet/core/extensions/logging

8. Rzor Views

Purpose

Razor Views provide the presentation layer of the application by generating dynamic HTML pages using the ASP.NET Core Razor view engine.

The primary responsibility of a Razor View is to display information received from the Controller while maintaining a clear separation between presentation and business logic.

Standard

All user interfaces shall be implemented using ASP.NET Core Razor Views.

Views shall remain responsible only for presentation.

Business logic, database access, and business rule implementation are prohibited inside Razor Views.

View Structure

Each CRUD module shall provide a standardized set of Views.

Example:

Views/
└── Roles/
    ├── Index.cshtml
    ├── Create.cshtml
    ├── Edit.cshtml
    ├── Details.cshtml
    ├── Delete.cshtml
    ├── Inactive.cshtml
    └── Activate.cshtml

Additional Views may be created when required by the application's business processes.

Strongly Typed Views

Every View shall declare its corresponding Model using the @model directive.

Examples:

@model IEnumerable<DB_GranjaLaFlor.Models.Entities.Role>
@model DB_GranjaLaFlor.Models.Entities.Role

Using strongly typed Views improves compile-time validation, IntelliSense support, and maintainability.

Tag Helpers

The project adopts ASP.NET Core Tag Helpers as the standard mechanism for generating HTML elements.

Examples include:

asp-action

asp-controller

asp-route-id

asp-for

asp-validation-for

asp-validation-summary

Tag Helpers shall be preferred over manually constructed URLs whenever possible.

Forms

Forms shall be created using Tag Helpers.

Example:

<form asp-action="Create" method="post">

    @Html.AntiForgeryToken()

    ...

</form>

Every POST form shall include:

Anti-forgery protection.
Validation Summary.
Property validation messages.
Buttons

The project distinguishes between actions that modify data and actions that perform navigation.

Buttons shall follow the following standard:

Action	Standard
Create	Button (type="submit")
Update	Button (type="submit")
Activate	Button (type="submit")
Delete	Button (type="submit")
Navigation	Anchor (asp-action)

Example:

<button type="submit"
        class="btn btn-outline-info">
    Save
</button>
<a asp-action="Index"
   class="btn btn-outline-secondary">
    Back
</a>

This distinction follows the ASP.NET Core MVC recommendations for form submission and navigation.

Standard CRUD Layout

All CRUD Views shall maintain a consistent visual design.

The project adopts the following structure:

Container
    │
    ▼
Card
    │
    ▼
Card Header
    │
    ▼
Card Body
    │
    ▼
Form or Table
    │
    ▼
Action Buttons

This layout shall be consistently applied across every module.

Bootstrap Components

The following Bootstrap components shall be used throughout the application:

Cards
Tables
Forms
Alerts
Badges
Buttons
Dropdown Menus
Navigation Bar

Consistency across modules shall be prioritized.

Partial Views

Reusable interface components shall be implemented as Partial Views.

Current implementation:

_EventMessage.cshtml

Future reusable components shall follow the same pattern.

Validation

Views shall display validation errors using:

asp-validation-summary

asp-validation-for

Client-side validation scripts shall be loaded using:

_ValidationScriptsPartial
JavaScript

Reusable JavaScript functionality shall be implemented inside:

wwwroot/js/site.js

JavaScript shall not be embedded directly inside Razor Views unless the functionality is specific to that View.

Separation of Responsibilities

The project defines the following responsibilities:

Component	Responsibility
Controller	Request Processing
Razor View	Presentation
Service	Business Logic
JavaScript	Client-side Behavior
CSS	Visual Appearance

Each component shall remain focused on its assigned responsibility.

Benefits

The adopted Razor View strategy provides:

Consistent user interface.
Reusable components.
Better maintainability.
Improved readability.
Strong typing.
Separation of Concerns.
Reduced code duplication.
Project Decisions

The following standards have been adopted:

Razor Views shall remain presentation-only.
Controllers shall provide all required data.
Business logic shall never be implemented inside Views.
CRUD modules shall follow the standardized Card layout.
Tag Helpers shall be used whenever possible.
Partial Views shall be used for reusable UI components.
JavaScript shall be centralized in site.js.
Bootstrap Cards constitute the standard visual container for every CRUD page.
Official References
Razor Syntax

https://learn.microsoft.com/aspnet/core/mvc/views/razor

Razor Views

https://learn.microsoft.com/aspnet/core/mvc/views/overview

Tag Helpers

https://learn.microsoft.com/aspnet/core/mvc/views/tag-helpers/intro

Partial Views

https://learn.microsoft.com/aspnet/core/mvc/views/partial


9. User Notifications

Purpose

User Notifications provide immediate feedback after the execution of an application operation. Their purpose is to inform the user whether an action has been successfully completed or if an error has occurred.

This project adopts the notification mechanisms provided by ASP.NET Core MVC through TempData, combined with reusable Razor Partial Views and client-side JavaScript.

Standard

All notifications displayed after a redirect shall use TempData.

Notification rendering shall be centralized through a reusable Partial View.

The project currently implements:

_EventMessage.cshtml

All CRUD modules shall use the same notification mechanism.

TempData

TempData shall be used to transfer notification messages between two consecutive HTTP requests.

Example:

TempData["SuccessMessage"] =
    "Role created successfully.";
TempData["ErrorMessage"] =
    "An unexpected error occurred.";

TempData shall only be used for temporary information that survives a single redirect.

It shall not be used to store application state.

Notification Types

The project currently defines two standard notification types.

Success Notification

Displayed after a successful operation.

Examples:

Create
Update
Activate
Soft Delete
Error Notification

Displayed when an unexpected error prevents the operation from completing successfully.

Typical scenarios include:

Database exceptions.
Unexpected application errors.
Invalid operations.
Partial View

Notification rendering shall be centralized in:

Views/
└── Shared/
        _EventMessage.cshtml

The Partial View shall be responsible only for rendering the notification.

Business logic shall not be implemented inside the Partial View.

JavaScript

Automatic notification closing shall be implemented inside:

wwwroot/js/site.js

The project uses a dedicated CSS class to identify automatically dismissible notifications.

Example:

auto-close-alert

Example:

document.addEventListener("DOMContentLoaded", function () {

    const alerts =
        document.querySelectorAll(".auto-close-alert");

    alerts.forEach(function (alert) {

        setTimeout(function () {

            const bsAlert =
                bootstrap.Alert.getOrCreateInstance(alert);

            bsAlert.close();

        }, 4000);

    });

});

This behavior shall remain outside Razor Views in order to maintain Separation of Concerns.

Separation of Responsibilities

The notification mechanism is divided into three independent components.

Component	Responsibility
Controller	Stores the notification message in TempData
_EventMessage.cshtml	Renders the notification
site.js	Automatically closes the notification

Each component shall perform a single responsibility.

Notification Flow

The project follows the notification sequence below.

Controller
      │
      ▼
TempData
      │
      ▼
RedirectToAction()
      │
      ▼
View
      │
      ▼
_EventMessage.cshtml
      │
      ▼
Bootstrap Alert
      │
      ▼
site.js
      │
      ▼
Automatic Closing (4 seconds)
Benefits

The notification mechanism provides:

Immediate user feedback.
Consistent behavior across all modules.
Reduced duplicated code.
Reusable UI components.
Separation of Concerns.
Improved user experience.
Project Decisions

The following standards have been adopted:

TempData is the official notification mechanism.
Notifications shall always survive a Redirect.
Notification rendering shall be centralized in _EventMessage.cshtml.
JavaScript shall be centralized in site.js.
Notifications shall automatically disappear after four seconds.
Controllers shall never generate HTML notifications directly.
Official References
TempData

https://learn.microsoft.com/aspnet/core/fundamentals/app-state

Partial Views

https://learn.microsoft.com/aspnet/core/mvc/views/partial

Static Files

https://learn.microsoft.com/aspnet/core/fundamentals/static-files

JavaScript in ASP.NET Core

https://learn.microsoft.com/aspnet/core/client-side/javascript/


10. Soft Delete


Purpose

The purpose of the Soft Delete strategy is to preserve historical information while preventing permanent data loss.

Instead of physically removing records from the database, the application marks them as inactive by updating their status field. This approach allows previously deactivated records to be restored whenever required.

Standard

Physical deletion of records is prohibited throughout the application.

All CRUD modules shall implement Soft Delete by updating the corresponding status property.

Example:

role.RoleState = false;

await _context.SaveChangesAsync();

Every entity that supports CRUD operations shall include an Active/Inactive status property.

Entity Design

Entities implementing Soft Delete shall define a Boolean status property.

Example:

public bool RoleState { get; set; }

The default value shall represent an active record.

CRUD Behavior

The project adopts the following behavior.

Create

New records shall be created as Active.

Read

Only Active records shall be displayed by default.

Example:

return await _context.Roles
    .AsNoTracking()
    .Where(role => role.RoleState)
    .ToListAsync();
Update

Only Active records may be modified.

Delete

Deleting a record shall update its status to Inactive.

The record shall remain stored in the database.

Activate

Previously deactivated records may be restored by updating the status back to Active.

Example:

role.RoleState = true;

await _context.SaveChangesAsync();
User Interface

Every CRUD module implementing Soft Delete shall provide:

Active records list.
Inactive records list.
Activate View.
Delete (Deactivate) View.

The user shall always be able to review inactive records before restoring them.

Query Standard

Queries retrieving Active records shall explicitly filter by the status property.

Example:

.Where(role => role.RoleState)

Queries retrieving Inactive records shall use:

.Where(role => !role.RoleState)

The filtering condition shall be explicitly defined in every query.

Business Rules

The project adopts the following rules:

Active records shall be visible by default.
Inactive records shall only be displayed through dedicated Views.
Deactivated records remain available for auditing.
Reactivation shall preserve the original record.
Primary Keys shall never change during reactivation.
Benefits

The Soft Delete strategy provides:

Preservation of historical information.
Prevention of accidental data loss.
Easier auditing.
Record recovery.
Improved traceability.
Better business continuity.
Project Decisions

The following decisions have been adopted:

Physical deletion is prohibited.
Every CRUD module shall implement Active/Inactive status.
Every CRUD module shall implement an Inactive View.
Every CRUD module shall implement an Activate View.
Controllers shall never permanently remove records.
Services shall perform status updates instead of DELETE operations.
Official References
Saving Data - Entity Framework Core

https://learn.microsoft.com/ef/core/saving/

Updating Data

https://learn.microsoft.com/ef/core/saving/basic

Change Tracking

https://learn.microsoft.com/ef/core/change-tracking/


11. User Interface (UI) Standard

Purpose

The User Interface (UI) defines the visual identity and interaction standards adopted throughout the application.

The objective is to provide a consistent, intuitive, and maintainable user experience across every module of the system.

To achieve this, the project adopts Bootstrap 5 together with the Bootswatch Sandstone theme as the standard UI framework.

Standard

The application shall implement a consistent visual design across all modules.

Every new View shall follow the same layout, component hierarchy, spacing, color palette, and navigation structure.

Consistency shall always take precedence over customization.

UI Framework

The project adopts:

Bootstrap 5

+

Bootswatch Sandstone Theme

Bootstrap provides the responsive component library while Bootswatch Sandstone defines the application's visual identity.

Example:

<link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.0/dist/sandstone/bootstrap.min.css"
      rel="stylesheet" />
Layout

Every View shall inherit from:

Shared/_Layout.cshtml

The layout defines:

Navigation Bar
Footer
CSS references
JavaScript references
Shared components

Individual Views shall only define their own content.

Navigation

Navigation shall be implemented using the Bootstrap Navbar component.

Current navigation structure:

Profile

Management
    ├── Roles
    └── Users

Login

Dropdown menus shall be used whenever multiple modules belong to the same functional area.

CRUD Layout

All CRUD pages shall adopt the following visual hierarchy.

Container
        │
        ▼
Card
        │
        ▼
Card Header
        │
        ▼
Card Body
        │
        ▼
Content
        │
        ▼
Action Buttons

Cards represent the primary visual container throughout the application.

Cards

Bootstrap Cards shall be used as the standard container for:

Forms
Tables
Detail pages
Confirmation pages

Example:

Create

Edit

Details

Delete

Activate

Inactive

Index

Cards improve visual organization and provide a consistent user experience.

Tables

Data shall be presented using responsive Bootstrap Tables.

Example:

<div class="table-responsive">

    <table class="table table-hover table-bordered align-middle">

    </table>

</div>

Tables shall:

Be responsive.
Display aligned content.
Use Bootstrap styling.
Include action buttons.
Forms

Forms shall use Bootstrap Form Controls.

Example:

<form>

    <div class="mb-3">

        <label></label>

        <input class="form-control"/>

    </div>

</form>

Spacing shall be implemented using Bootstrap utility classes.

Buttons

The following color convention has been adopted.

Action	Bootstrap Style
Create	btn-outline-primary
Save	btn-outline-info
Edit	btn-outline-warning
Delete / Deactivate	btn-outline-danger
Activate	btn-outline-success
Navigation	btn-outline-secondary

This convention shall remain consistent across every module.

Status Indicators

Entity status shall be displayed using Bootstrap Badges.

Example:

Active

Inactive

Example:

<span class="badge bg-info">

    Active

</span>
<span class="badge bg-danger">

    Inactive

</span>

Badges provide immediate visual feedback.

Notifications

System notifications shall be displayed using Bootstrap Alerts.

Notifications shall be rendered through:

_EventMessage.cshtml

Bootstrap Alerts shall automatically disappear after four seconds.

Responsive Design

The application shall support multiple screen sizes.

Bootstrap Grid System shall be used whenever responsive layouts are required.

Responsive components shall be preferred over fixed-width layouts.

Consistency

Every new module shall maintain:

Same Card layout.
Same table structure.
Same button colors.
Same spacing.
Same navigation.
Same typography.
Same Bootstrap components.

Visual consistency shall always be prioritized over individual module customization.

Benefits

The adopted UI standards provide:

Consistent visual identity.
Better usability.
Improved readability.
Easier maintenance.
Faster development.
Reusable UI components.
Professional appearance.
Project Decisions

The following UI standards have been adopted:

Bootswatch Sandstone is the official application theme.
Bootstrap Cards are the standard CRUD container.
Every CRUD page shall follow the same layout.
Button colors are standardized by action type.
Bootstrap Tables shall be used for data presentation.
Bootstrap Badges represent entity status.
Navigation shall be centralized in _Layout.cshtml.
All modules shall preserve the same visual identity.
Official References
Bootstrap 5

https://getbootstrap.com/docs/5.3/

Bootswatch

https://bootswatch.com/

Bootstrap Components

https://getbootstrap.com/docs/5.3/components/

Layout in ASP.NET Core

https://learn.microsoft.com/aspnet/core/mvc/views/layout


12. Security Standards
Purpose

Security is a fundamental aspect of the application architecture. The objective of these standards is to protect the system against common web application vulnerabilities while ensuring the integrity, confidentiality, and reliability of application data.

This project adopts the security mechanisms provided by ASP.NET Core as the foundation for secure web application development.

Standard

Security shall be implemented using the mechanisms provided by ASP.NET Core whenever possible.

Custom security implementations shall only be introduced when the framework does not provide an equivalent solution.

Cross-Site Request Forgery (CSRF)

Every HTTP POST request shall be protected against Cross-Site Request Forgery attacks.

Controllers shall implement:

[ValidateAntiForgeryToken]

Forms shall include:

@Html.AntiForgeryToken()

The Anti-Forgery Token shall be automatically validated by ASP.NET Core before executing the Controller Action.

Model Validation

Every request that modifies application data shall validate the received model.

Example:

if (!ModelState.IsValid)
{
    return View(model);
}

Business operations shall never execute when model validation fails.

Data Validation

Validation shall be performed using Data Annotations.

Examples include:

Required
StringLength
EmailAddress
Range
DataType

Client-side validation shall complement, but never replace, server-side validation.

Parameterized Queries

Database operations shall be performed using Entity Framework Core.

Example:

return await _context.Roles
    .Where(role => role.RoleState)
    .ToListAsync();

Entity Framework Core automatically generates parameterized SQL statements, reducing the risk of SQL Injection attacks.

Manual SQL statements shall be avoided whenever possible.

Exception Handling

Unexpected exceptions shall be handled using try/catch.

Sensitive exception details shall never be displayed to end users.

Instead, Controllers shall:

Log the exception.
Display a generic error message.
Return the appropriate View.
Logging

Security-related events shall be recorded using ILogger<T>.

Examples include:

Unexpected exceptions.
Invalid operations.
Missing records.
Validation failures.

Sensitive information shall never be written to application logs.

Sensitive Information

The following information shall never appear in:

Logs
Error messages
User notifications
URLs

Examples include:

Passwords
Connection Strings
Authentication Tokens
Security Keys
Personal Identification Numbers

Only information required for diagnostics shall be logged.

Dependency Injection

Controllers shall receive their dependencies through Constructor Injection.

Manual object creation using the new operator shall be avoided for application Services.

Dependency Injection improves maintainability while reducing unnecessary coupling between application components.

Separation of Responsibilities

The project adopts the Separation of Concerns (SoC) principle as part of its security strategy.

Responsibilities are divided as follows:

Component	Responsibility
Controller	HTTP Processing
Service	Business Logic
DbContext	Data Access
View	User Interface

This separation minimizes the risk of introducing business logic into presentation components.

Benefits

The adopted security standards provide:

Protection against CSRF attacks.
Reduced SQL Injection risk.
Improved data validation.
Better exception handling.
Secure logging practices.
Lower coupling.
Improved maintainability.
Project Decisions

The following security decisions have been adopted:

Every POST request shall validate an Anti-Forgery Token.
Controllers shall always validate ModelState.
Entity Framework Core shall be used for all database operations.
Controllers shall never expose internal exception details.
Sensitive information shall never appear in logs.
Dependency Injection shall be used throughout the application.
Separation of Concerns shall be maintained across every layer.
Future Security Standards

The following security mechanisms will be incorporated during the implementation of the authentication module:

Authentication
Authorization
Password Hashing
Secure Authentication Cookies
Claims-Based Authorization
Session Management
Role-Based Authorization

These standards will be documented once the corresponding functionality is implemented.

Official References
Secure ASP.NET Core Applications

https://learn.microsoft.com/aspnet/core/security/

Anti-Forgery

https://learn.microsoft.com/aspnet/core/security/anti-request-forgery

Model Validation

https://learn.microsoft.com/aspnet/core/mvc/models/validation

Entity Framework Core Security

https://learn.microsoft.com/ef/core/

Logging

https://learn.microsoft.com/aspnet/core/fundamentals/logging







JavaScript


Partial Views

Dependency Injection

Authentication & Authorization (cuando implementemos Login)




Entity Property Order

Every Entity shall organize its properties using the following order:

Primary Key

↓

Business Properties

↓

Status Property

↓

Foreign Keys

↓

Navigation Properties









++++++++++++++++++++++++++++++++++++++++++++++++ Authenticatiion, authorization and session cookie: ++++++++++++++++++++++++++++++++++++++++++++++++

Login correcto

↓

SignInAsync()

↓

Cookie

↓

Redirect

↓

Usuario autenticado



+++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  


Usuario entra al sistema
        ↓
/Account/Login
        ↓
Ingresa correo y contraseña
        ↓
Login valida usuario activo + password hash
        ↓
SignInAsync crea cookie de autenticación
        ↓
RedirectToAction("Index", "Home")
        ↓
Home funciona como página inicial temporal

+++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  

Login
↓
Cookie creada
↓
Navbar muestra usuario
↓
Logout
↓
Cookie eliminada
↓
Navbar vuelve a mostrar Login

+++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  

| Módulo / Funcionalidad   | Propietarios | Administrador | Operario    |
| ------------------------ | ------------ | ------------- | ----------- |
| Dashboard                | Total        | Lectura       | Lectura     |
| Acceso y Usuarios        | Total        | Sin acceso    | Sin acceso  |
| Roles                    | CRUD         | Sin acceso    | Sin acceso  |
| Usuarios                 | CRUD         | Sin acceso    | Sin acceso  |
| Mortalidad               | CRUD         | Lectura       | CRUD        |
| Pesaje                   | CRUD         | Lectura       | CRUD        |
| Consumo de Alimento      | CRUD         | Lectura       | CRUD        |
| Control Diario           | CRUD         | Lectura       | CRUD        |
| Control Semanal          | CRUD         | Lectura       | CRUD        |
| Monitoreo de Temperatura | Total        | Lectura       | Lectura     |
| Estimulación Temprana    | Total        | Lectura       | Por definir |

+++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  +++++++  

