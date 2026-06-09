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

*/