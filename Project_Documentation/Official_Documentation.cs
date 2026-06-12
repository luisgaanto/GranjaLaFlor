/*

  
1. ASP.NET Core Best Practices ⭐⭐⭐⭐⭐ (el más importante)

Este documento reúne las recomendaciones oficiales de Microsoft sobre rendimiento, arquitectura y confiabilidad.
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0&
ASP.NET Core Best Practices

Algunos temas que cubre:

Uso de async y await
Evitar consultas bloqueantes
Optimización de consultas a la base de datos
Caché
Manejo de colecciones grandes
Buenas prácticas de memoria
Logging
Escalabilidad y rendimiento

2. ASP.NET Core Fundamentals
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-10.0&
ASP.NET Core Fundamentals

Aquí Microsoft explica cómo debe estructurarse una aplicación ASP.NET Core:

Program.cs
Dependency Injection
Middleware
Configuración (appsettings.json)
Environments (Development, Production)
Logging
Seguridad

3. Entity Framework Core Documentation
https://learn.microsoft.com/en-us/ef/core/?
Entity Framework Core Documentation

Incluye buenas prácticas para:

Modelado de entidades
Relaciones
Migrations
Consultas con LINQ
Tracking vs NoTracking
Performance
Lazy Loading
Eager Loading (Include)
Transactions

4. Documentación general de .NET
https://learn.microsoft.com/en-us/dotnet/?
.NET Documentation

Es el portal principal para aprender:

C#
ASP.NET Core
EF Core
Minimal APIs
Web API
Testing
Seguridad
Arquitectura


5. C# Coding Conventions ⭐⭐⭐⭐⭐
Convenciones de código (oficial Microsoft)

C# Coding Conventions
https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions?
Este documento será nuestra guía para:

PascalCase
camelCase
Organización de archivos
Convenciones de nombres
Formato del código
Legibilidad
Comentarios
Espaciado
Indentación
Guía general de C#

C# Guide
https://learn.microsoft.com/en-us/dotnet/csharp/?

Incluye:

Características del lenguaje
Programación orientada a objetos
Genéricos
LINQ
Async/Await
Colecciones
Delegates
Events



6. EF Core Performance ⭐⭐⭐⭐⭐
Efficient Querying (la más importante)

EF Core Efficient Querying

https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying?

Esta será una de nuestras principales referencias.

Aplicaremos:

AsNoTracking()
Select()
Include()
ThenInclude()
Evitar N+1 Queries
Cargar únicamente los datos necesarios
Optimización de LINQ
Introducción al rendimiento de EF Core

EF Core Performance Overview
https://learn.microsoft.com/en-us/ef/core/performance/?
Aquí Microsoft explica:

Cómo EF Core genera SQL
Índices
Rendimiento
Optimización de consultas

7. MySQL Best Practices ⭐⭐⭐⭐⭐

MySQL no tiene una página llamada literalmente "Best Practices", pero sí documenta las recomendaciones oficiales en varias secciones.

Documentación oficial MySQL
https://dev.mysql.com/doc/?
MySQL Reference Manual

Es el manual oficial completo.

Optimizing Database Structure

Esta será nuestra guía para diseñar la base de datos.

Optimizing Database Structure
https://dev.mysql.com/doc/refman/9.6/en/optimizing-database-structure.html?
Incluye:

Diseño de tablas
Índices
Llaves
Rendimiento
Optimización del esquema


Arquitectura

Presentation Layer
│
├── Controllers
├── Views (Razor)
├── ViewModels
│
Business Layer
│
├── Services
├── Interfaces
├── Business Rules
├── Validations
│
Data Layer
│
├── DbContext
├── Entities (Models)
├── Configurations
│
Database
│
└── MySQL


REASONS REASONS REASONS REASONS REASONS REASONS REASONS REASONS REASONS REASONS REASONS REASONS REASONS REASONS 

DB_GranjaLaFlor
│
├── Controllers
│   ├── RolesController.cs
│   ├── UsersController.cs
│   ├── BroilerHousesController.cs
│   ├── BroodsController.cs
│   ├── IncomeConcentratesController.cs
│   ├── DailyChecksController.cs
│   ├── WeeklyChecksController.cs
│   ├── TemperatureController.cs
│   └── EarlyStimulationController.cs
│
├── Models
│   ├── Entities
│   │   ├── Role.cs
│   │   ├── User.cs
│   │   ├── BroilerHouse.cs
│   │   ├── Brood.cs
│   │   ├── DcDay.cs
│   │   ├── WcWeek.cs
│   │   ├── ExpectedValue.cs
│   │   ├── IncomeConcentrate.cs
│   │   ├── DailyCheck.cs
│   │   └── WeeklyCheck.cs
│   │
│   ├── ViewModels
│   │   ├── LoginViewModel.cs
│   │   ├── DailyCheckViewModel.cs
│   │   ├── WeeklyCheckViewModel.cs
│   │   └── TemperatureViewModel.cs
│   │
│   └── Validations
│
├── Data
│   ├── Context
│   │   └── ApplicationDbContext.cs
│   │
│   ├── Configurations
│   │   ├── RoleConfiguration.cs
│   │   ├── UserConfiguration.cs
│   │   ├── BroodConfiguration.cs
│   │   ├── DailyCheckConfiguration.cs
│   │   └── WeeklyCheckConfiguration.cs
│   │
│ 
│
├── Services
│   ├── RoleService.cs
│   ├── UserService.cs
│   ├── BroilerHouseService.cs
│   ├── BroodService.cs
│   ├── IncomeConcentrateService.cs
│   ├── DailyCheckService.cs
│   ├── WeeklyCheckService.cs
│   ├── TemperatureService.cs
│   └── EarlyStimulationService.cs
│
├── ExternalServices
│   ├── Interfaces
│   │   ├── ITemperatureApiClient.cs
│   │   └── ISpeakerApiClient.cs
│   │
│   └── Implementations
│       ├── TemperatureApiClient.cs
│       └── SpeakerApiClient.cs
│
├── Helpers
│   ├── PasswordHelper.cs
│   └── CalculationHelper.cs
│
├── Views
│   ├── Roles
│   ├── Users
│   ├── BroilerHouses
│   ├── Broods
│   ├── IncomeConcentrates
│   ├── DailyChecks
│   ├── WeeklyChecks
│   ├── Temperature
│   ├── EarlyStimulation
│   └── Shared
│
├── wwwroot
│   ├── css
│   ├── js
│   ├── images
│   └── lib
│
├── appsettings.json
├── appsettings.Development.json
└── Program.cs


Services:

Microsoft explains that classes should receive their dependencies via Dependency Injection.

Microsoft indicates that controllers should be kept lightweight and that heavy logic should not be placed within them.

Add layers to split up responsabilities. 

Based on Dependency Injection: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0

Best practices: https://learn.mBest Praicrosoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0

Common web application architectures: https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

eShopOnWeb: https://github.com/dotnet-architecture/eShopOnWeb

  Controller                           -->         RoleController
      ↓                                                  ↓        
   Service                             -->          RoleService
      ↓                                                  ↓  
ApplicationDbContext                   -->       ApplicationDbContext
      ↓
    MySQL                                       


  Database Seeding: to add data into DB automatically. 


DbContext: https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/?





Public or Private 

Principle of Least Privilege

Public: When there is a need to be called by other classes, controllers, models, etc..

Public: No need to be called by other parts of the project, only by the method that is belongs from. 

| Elemento                         | Modificador recomendado | Motivo                                      |
| -------------------------------- | ----------------------- | ------------------------------------------- |
| Controllers                      | `public`                | ASP.NET Core los descubre automáticamente.  |
| Services                         | `public`                | Son utilizados por los controladores.       |
| DbContext                        | `public`                | EF Core y DI deben acceder a él.            |
| Entidades (`Role`, `User`, etc.) | `public`                | EF Core realiza el mapeo.                   |
| Propiedades de entidades         | `public`                | EF Core necesita leer y escribir valores.   |
| `DbSet<>`                        | `public`                | Representan las tablas de la base de datos. |
| Constructor                      | `public`                | Lo utiliza Dependency Injection.            |
| Campos (`_context`)              | `private readonly`      | Encapsulación y seguridad.                  |
| Métodos auxiliares               | `private`               | Solo se usan dentro de la clase.            |
| Constantes internas              | `private const`         | Solo pertenecen a la clase.                 |



| Elemento            | Modificador        |
| ------------------- | ------------------ |
| Controllers         | `public`           |
| Services            | `public`           |
| DbContext           | `public`           |
| Entidades EF Core   | `public`           |
| Propiedades EF Core | `public`           |
| `DbSet<>`           | `public`           |
| Constructor         | `public`           |
| Campos (`_context`) | `private readonly` |
| Métodos auxiliares  | `private`          |
| Constantes          | `private const`    |


Reference for public and private: 
https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers?

https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/accessibility-levels?



SECURITY


                 Usuarios
                     │
                     ▼
             Authentication
                     │
                     ▼
              Authorization
                     │
                     ▼
Controllers ─────► Services ─────► External APIs
                     │                  │
                     ▼                  ▼
                 Entity Framework    HTTPS
                     │
                     ▼
                  MySQL




| Medida                          | Implementar | Momento                                        |
| ------------------------------- | :---------: | ---------------------------------------------- |
| HTTPS                           |      ✅      | Desde el primer despliegue                     |
| Password Hash (BCrypt)          |      ✅      | Módulo Usuarios                                |
| Authentication                  |      ✅      | Módulo Login                                   |
| Authorization por Roles         |      ✅      | Módulo Login                                   |
| Anti-Forgery (CSRF)             |      ✅      | Todos los formularios POST                     |
| Validación con Data Annotations |      ✅      | Todas las entidades                            |
| Validación en Services          |      ✅      | Toda la lógica de negocio                      |
| LINQ / EF Core                  |      ✅      | Todo el proyecto                               |
| Logging                         |      ✅      | Desde el inicio                                |
| APIs con `HttpClient` + HTTPS   |      ✅      | Módulos de Temperatura y Estimulación Temprana |



Referencias:

https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-10.0

https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-10.0

https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-10.0&

https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-10.0&


DataAnnotation:
https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.displayattribute?view=net-10.0
https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=net-10.0
https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-10.0


Fuent API:

https://learn.microsoft.com/en-us/ef/core/modeling/?
https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties?tabs=data-annotations%2Cwith-nrt






























*/