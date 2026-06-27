ArchitectureDecisions.md

AD-001 MVC Architecture

AD-002 Service Layer

AD-003 Dependency Injection

AD-004 Entity Framework Core

AD-005 Logging

AD-006 Soft Delete

AD-007 Partial Views

AD-008 Client-side JavaScript

AD-009 Bootswatch Sandstone

AD-010 ViewModels (cuando implementemos Login)

AD-011 Authentication

AD-012 Authorization

+++++++++++++++++++++++++++++++++++DECISION A FUTURO+++++++++++++++++++++++++++++++++++++++++++++
Lo correcto arquitectónicamente

La base de datos debería decir:

Role
-------
Propietarios

Administrador

Operario

Supervisor

Pero además:

Permission
-------------
AccessUsers

OperationalRead

OperationalWrite

MonitoringRead

MonitoringWrite

Y una tercera tabla:

RolePermission
--------------------------

RoleId    PermissionId

1         1

1         2

1         3

1         4

2         2

2         4

3         2

3         3

3         4

4         2

4         4

Entonces el Login hace esto:

Buscar Usuario

↓

Buscar Rol

↓

Buscar TODOS los permisos
que tiene ese Rol

↓

Crear Claims

↓

Login














Validations  ILogger


HTTP Request
      │
      ▼
RolesController
      │
      │  LogInformation:
      │  Entering Create()
      ▼
RoleService
      │
      │  LogInformation:
      │  Creating role...
      ▼
Entity Framework
      │
      ▼
MySQL
      ▲
      │
RoleService
      │
      │  LogInformation:
      │  Role created successfully.
      ▼
RolesController
      │
      │  LogInformation:
      │  Redirecting to Index().
      ▼
Response


CONSTRUCTORE BUILDER 


Browser
      │
      ▼
GET /Roles
      │
      ▼
Routing
      │
      ▼
Dependency Injection Container
      │
      ├──────────────► RoleService
      │
      ├──────────────► ILogger
      │
      ▼
Constructor
      │
      ▼
RolesController
      │
      ▼
Index()


