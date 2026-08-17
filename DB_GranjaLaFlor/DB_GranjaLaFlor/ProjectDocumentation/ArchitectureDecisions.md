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

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

Ecuaciones:

| Módulo                 | Nombre del cálculo            | Columna donde se guarda                               | Tablas involucradas                   | Atributos utilizados                                      | Ecuación                                                                                 |
| ---------------------- | ----------------------------- | ----------------------------------------------------- | ------------------------------------- | --------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| **Income Concentrate** | Ingreso en kilos              | `income_kilos`                                        | `income_concentrates`                 | `income_quintals`                                         | `income_kilos = income_quintals × 46`                                                    |
| **Income Concentrate** | Concentrado acumulado         | `income_accumulated`                                  | `income_concentrates`                 | `previous_income_accumulated`, `income_kilos`             | `income_accumulated = previous_income_accumulated + income_kilos`                        |
| **Daily Check**        | Mortalidad diaria total       | `total_daily_mortality`                               | `daily_checks`                        | `natural_mortality`, `select_quantity`                    | `total_daily_mortality = natural_mortality + select_quantity`                            |
| **Daily Check**        | Mortalidad acumulada          | `accumulated_mortality`                               | `daily_checks`                        | `previous_accumulated_mortality`, `total_daily_mortality` | `accumulated_mortality = previous_accumulated_mortality + total_daily_mortality`         |
| **Daily Check**        | Saldo de aves                 | `daily_bird_balance`                                  | `broods`, `daily_checks`              | `brood_bird_initial_num`, `accumulated_mortality`         | `daily_bird_balance = brood_bird_initial_num − accumulated_mortality`                    |
| **Daily Check**        | Consumo en kilos              | `consumption_kilos`                                   | `daily_checks`                        | `consumption_quintals`                                    | `consumption_kilos = consumption_quintals × 46`                                          |
| **Daily Check**        | Consumo acumulado             | `accumulated_consumption`                             | `daily_checks`                        | `previous_accumulated_consumption`, `consumption_kilos`   | `accumulated_consumption = previous_accumulated_consumption + consumption_kilos`         |
| **Daily Check**        | Saldo de concentrado          | `concentrate_balance`                                 | `income_concentrates`, `daily_checks` | `income_accumulated`, `accumulated_consumption`           | `concentrate_balance = income_accumulated − accumulated_consumption`                     |
| **Weekly Check**       | Cantidad de aves a pesar (2%) | *(puede ser calculado, no necesariamente almacenado)* | `daily_checks`                        | `daily_bird_balance`                                      | `sample_bird_quantity = CEILING(daily_bird_balance × 0.02)`                              |
| **Weekly Check**       | Peso promedio semanal         | `average_weekly_weight`                               | `weekly_checks`                       | `total_bird_weight`, `sample_bird_quantity`               | `average_weekly_weight = total_bird_weight ÷ sample_bird_quantity`                       |
| **Weekly Check**       | Consumo real semanal          | `weekly_real_consumption`                             | `daily_checks`                        | `consumption_kilos` de los 7 controles diarios            | `weekly_real_consumption = Σ consumption_kilos`                                          |
| **Weekly Check**       | Diferencia de consumo         | `weekly_consumption_difference`                       | `weekly_checks`, `expected_values`    | `weekly_real_consumption`, `expected_consumption`         | `weekly_consumption_difference = weekly_real_consumption − expected_consumption`         |
| **Weekly Check**       | Diferencia de peso            | `weekly_weight_difference`                            | `weekly_checks`, `expected_values`    | `average_weekly_weight`, `expected_weight`                | `weekly_weight_difference = average_weekly_weight − expected_weight`                     |
| **Weekly Check**       | Conversión real               | `weekly_real_conversion`                              | `weekly_checks`                       | **Pendiente de definir**                                  | **Pendiente de definir**                                                                 |
| **Weekly Check**       | Diferencia de conversión      | `weekly_conversion_difference`                        | `weekly_checks`, `expected_values`    | `weekly_real_conversion`, `expected_conversion`           | `weekly_conversion_difference = weekly_real_conversion − expected_conversion`            |
| **Weekly Check**       | Mortalidad real semanal       | `weekly_real_mortality`                               | `daily_checks`                        | `total_daily_mortality` de la semana                      | `weekly_real_mortality = Σ total_daily_mortality` *(o porcentaje, pendiente de definir)* |
| **Weekly Check**       | Diferencia de mortalidad      | `weekly_mortality_difference`                         | `weekly_checks`, `expected_values`    | `weekly_real_mortality`, `expected_mortality`             | `weekly_real_mortality − expected_mortality`                                             |


++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

Income Concentrate:

| Nombre del cálculo    | Columna donde se guarda | Tablas involucradas   | Atributos utilizados                          | Ecuación                                                          |
| --------------------- | ----------------------- | --------------------- | --------------------------------------------- | ----------------------------------------------------------------- |
| Ingreso en kilos      | `income_kilos`          | `income_concentrates` | `income_quintals`                             | `income_kilos = income_quintals × 46`                             |
| Concentrado acumulado | `income_accumulated`    | `income_concentrates` | `previous_income_accumulated`, `income_kilos` | `income_accumulated = previous_income_accumulated + income_kilos` |

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

Daily Check

| Nombre del cálculo      | Columna donde se guarda   | Tablas involucradas                   | Atributos utilizados                                      | Ecuación                                                                         |
| ----------------------- | ------------------------- | ------------------------------------- | --------------------------------------------------------- | -------------------------------------------------------------------------------- |
| Mortalidad diaria total | `total_daily_mortality`   | `daily_checks`                        | `natural_mortality`, `select_quantity`                    | `total_daily_mortality = natural_mortality + select_quantity`                    |
| Mortalidad acumulada    | `accumulated_mortality`   | `daily_checks`                        | `previous_accumulated_mortality`, `total_daily_mortality` | `accumulated_mortality = previous_accumulated_mortality + total_daily_mortality` |
| Saldo de aves           | `daily_bird_balance`      | `broods`, `daily_checks`              | `brood_bird_initial_num`, `accumulated_mortality`         | `daily_bird_balance = brood_bird_initial_num − accumulated_mortality`            |
| Consumo en kilos        | `consumption_kilos`       | `daily_checks`                        | `consumption_quintals`                                    | `consumption_kilos = consumption_quintals × 46`                                  |
| Consumo acumulado       | `accumulated_consumption` | `daily_checks`                        | `previous_accumulated_consumption`, `consumption_kilos`   | `accumulated_consumption = previous_accumulated_consumption + consumption_kilos` |
| Saldo de concentrado    | `concentrate_balance`     | `income_concentrates`, `daily_checks` | `income_accumulated`, `accumulated_consumption`           | `concentrate_balance = income_accumulated − accumulated_consumption`             |


++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


| Nombre del cálculo                  | Columna donde se guarda         | Tablas involucradas                | Atributos utilizados                              | Ecuación                                                                                                |
| ----------------------------------- | ------------------------------- | ---------------------------------- | ------------------------------------------------- | ------------------------------------------------------------------------------------------------------- |
| Cantidad de aves de la muestra (2%) | *(Calculado automáticamente)*   | `daily_checks`                     | `daily_bird_balance`                              | `sample_bird_quantity = CEILING(daily_bird_balance × 0.02)`                                             |
| Peso promedio semanal               | `average_weekly_weight`         | `weekly_checks`                    | `total_bird_weight`, `sample_bird_quantity`       | `average_weekly_weight = total_bird_weight ÷ sample_bird_quantity`                                      |
| Consumo real semanal                | `weekly_real_consumption`       | `daily_checks`                     | `consumption_kilos` de la semana                  | `weekly_real_consumption = Σ consumption_kilos`                                                         |
| Diferencia de consumo               | `weekly_consumption_difference` | `weekly_checks`, `expected_values` | `weekly_real_consumption`, `expected_consumption` | `weekly_consumption_difference = weekly_real_consumption − expected_consumption`                        |
| Diferencia de peso                  | `weekly_weight_difference`      | `weekly_checks`, `expected_values` | `average_weekly_weight`, `expected_weight`        | `weekly_weight_difference = average_weekly_weight − expected_weight`                                    |
| Conversión real                     | `weekly_real_conversion`        | `weekly_checks`                    | **Pendiente de definir**                          | **Pendiente**                                                                                           |
| Diferencia de conversión            | `weekly_conversion_difference`  | `weekly_checks`, `expected_values` | `weekly_real_conversion`, `expected_conversion`   | `weekly_conversion_difference = weekly_real_conversion − expected_conversion`                           |
| Mortalidad real semanal             | `weekly_real_mortality`         | `daily_checks`                     | `total_daily_mortality` de la semana              | `weekly_real_mortality = Σ total_daily_mortality` *(pendiente confirmar si será cantidad o porcentaje)* |
| Diferencia de mortalidad            | `weekly_mortality_difference`   | `weekly_checks`, `expected_values` | `weekly_real_mortality`, `expected_mortality`     | `weekly_mortality_difference = weekly_real_mortality − expected_mortality`                              |

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

Peso promedio semanal (Detalle)

| Concepto                           | Descripción                                                                                                 |
| ---------------------------------- | ----------------------------------------------------------------------------------------------------------- |
| **Frecuencia**                     | Cada **7 días** se realiza un control de peso.                                                              |
| **Población utilizada**            | Se toma la **población actual** (`daily_bird_balance`) proveniente del último **Daily Check** de la semana. |
| **Cantidad de aves a pesar**       | Se pesa el **2% de la población actual**.                                                                   |
| **Cantidad de aves de la muestra** | `sample_bird_quantity = CEILING(daily_bird_balance × 0.02)`                                                 |
| **Peso total de la muestra**       | Es la suma de los pesos (kg) de todas las aves pesadas. Este valor lo registra el usuario.                  |
| **Peso promedio semanal**          | `average_weekly_weight = total_bird_weight ÷ sample_bird_quantity`                                          |
| **Unidad del resultado**           | Kilogramos por ave (kg/ave).                                                                                |

Población actual: 10 000 aves

Muestra:
10 000 × 0.02 = 200 aves

Peso total de las 200 aves:
184 kg

Peso promedio semanal:
184 / 200 = 0.92 kg por ave

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

Tabla expected_values

| Columna                | Tipo            | Nulo | Llave               | Uso                                                              |
| ---------------------- | --------------- | ---: | ------------------- | ---------------------------------------------------------------- |
| `expected_value_id`    | `INT`           |   No | PK, autoincremental | Identificador interno del registro                               |
| `expected_value_week`  | `VARCHAR(20)`   |   No | —                   | Semana fija: `Semana 1` a `Semana 6`                             |
| `expected_consumption` | `DECIMAL(10,3)` |   No | —                   | Consumo esperado expresado en kilogramos con precisión de gramos |
| `expected_weight`      | `DECIMAL(10,3)` |   No | —                   | Peso esperado expresado en kilogramos con precisión de gramos    |
| `expected_conversion`  | `DECIMAL(10,2)` |   No | —                   | Conversión alimenticia esperada                                  |
| `expected_mortality`   | `DECIMAL(10,2)` |   No | —                   | Mortalidad acumulada esperada expresada como porcentaje          |

Datos iniciales

| ID | Semana   | Consumo esperado | Peso esperado | Conversión esperada | Mortalidad esperada |
| -: | -------- | ---------------: | ------------: | ------------------: | ------------------: |
|  1 | Semana 1 |       `0.170 kg` |    `0.200 kg` |              `0.85` |            `1.00 %` |
|  2 | Semana 2 |       `0.605 kg` |    `0.550 kg` |              `1.10` |            `1.60 %` |
|  3 | Semana 3 |       `1.322 kg` |    `1.100 kg` |              `1.20` |            `2.20 %` |
|  4 | Semana 4 |       `2.100 kg` |    `1.700 kg` |              `1.35` |            `2.80 %` |
|  5 | Semana 5 |       `3.455 kg` |    `2.350 kg` |              `1.47` |            `3.50 %` |
|  6 | Semana 6 |       `3.720 kg` |    `2.400 kg` |              `1.47` |            `4.00 %` |


| Valor almacenado | Significado                 |
| ---------------: | --------------------------- |
|          `0.170` | 170 gramos                  |
|          `0.605` | 605 gramos                  |
|          `1.100` | 1 kilogramo con 100 gramos  |
|          `1.322` | 1 kilogramo con 322 gramos  |
|          `2.350` | 2 kilogramos con 350 gramos |
|          `3.720` | 3 kilogramos con 720 gramos |


Reglas de negocio de ExpectedValues

| N.º | Regla de negocio                                                                                                            |
| --: | --------------------------------------------------------------------------------------------------------------------------- |
|   1 | El catálogo contiene exactamente seis registros, correspondientes a `Semana 1` hasta `Semana 6`.                            |
|   2 | Los seis registros se crean directamente mediante SQL antes de utilizar el módulo.                                          |
|   3 | La aplicación no permitirá crear nuevos registros de valores esperados.                                                     |
|   4 | La aplicación no permitirá eliminar, desactivar ni reactivar valores esperados.                                             |
|   5 | `ExpectedValueWeek` identifica la semana del catálogo y no puede modificarse desde la aplicación.                           |
|   6 | La semana se mostrará en `Index` y `Edit`, pero será únicamente informativa.                                                |
|   7 | El usuario podrá modificar únicamente consumo, peso, conversión y mortalidad esperados.                                     |
|   8 | `ExpectedConsumption` se almacena en kilogramos con tres decimales.                                                         |
|   9 | `ExpectedWeight` se almacena en kilogramos con tres decimales.                                                              |
|  10 | Consumo y peso deben ser mayores que cero.                                                                                  |
|  11 | `ExpectedConversion` se almacena con dos decimales y debe ser mayor que cero.                                               |
|  12 | `ExpectedMortality` representa un porcentaje, se almacena con dos decimales y debe estar entre `0.00` y `100.00`.           |
|  13 | La lógica y las validaciones de negocio se implementarán dentro de `ExpectedValueService`.                                  |
|  14 | El Controller solamente coordinará solicitudes HTTP, logs, mensajes y respuestas.                                           |
|  15 | El `Index` mostrará siempre los seis registros ordenados de `Semana 1` a `Semana 6`.                                        |
|  16 | El `Edit` buscará el registro mediante `ExpectedValueId`.                                                                   |
|  17 | `UpdateAsync()` no actualizará `ExpectedValueWeek`, aunque el valor sea enviado desde el formulario.                        |
|  18 | `UpdateAsync()` actualizará únicamente `ExpectedConsumption`, `ExpectedWeight`, `ExpectedConversion` y `ExpectedMortality`. |
|  19 | `WeeklyCheck` utilizará el registro de `expected_values` correspondiente a su misma semana.                                 |
|  20 | La relación con `WeeklyCheck` se realizará mediante `expected_value_id`.                                                    |
|  21 | No se agregarán por ahora restricciones de negocio como `UNIQUE` o `CHECK` directamente en la base de datos.                |
|  22 | Si en el futuro cambian los parámetros productivos, el usuario autorizado podrá actualizarlos desde `Edit`.                 |




++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

Tabla final weekly_checks

| Columna                         | Tipo final      | Nulo | Origen                       |
| ------------------------------- | --------------- | ---: | ---------------------------- |
| `weekly_check_id`               | `INT`           |   No | PK autoincremental           |
| `sample_bird_quantity`          | `INT`           |   No | Calculado                    |
| `total_bird_weight`             | `DECIMAL(10,3)` |   No | Usuario                      |
| `average_weekly_weight`         | `DECIMAL(10,3)` |   No | Calculado                    |
| `weekly_real_consumption`       | `DECIMAL(10,3)` |   No | Calculado                    |
| `weekly_expected_consumption`   | `DECIMAL(10,3)` |   No | Copiado de `expected_values` |
| `weekly_consumption_difference` | `DECIMAL(10,3)` |   No | Calculado                    |
| `weekly_expected_weight`        | `DECIMAL(10,3)` |   No | Copiado de `expected_values` |
| `weekly_weight_difference`      | `DECIMAL(10,3)` |   No | Calculado                    |
| `weekly_real_conversion`        | `DECIMAL(10,2)` |   No | Calculado                    |
| `weekly_expected_conversion`    | `DECIMAL(10,2)` |   No | Copiado de `expected_values` |
| `weekly_conversion_difference`  | `DECIMAL(10,2)` |   No | Calculado                    |
| `weekly_real_mortality`         | `DECIMAL(10,2)` |   No | Calculado                    |
| `weekly_expected_mortality`     | `DECIMAL(10,2)` |   No | Copiado de `expected_values` |
| `weekly_mortality_difference`   | `DECIMAL(10,2)` |   No | Calculado                    |
| `weekly_check_description`      | `VARCHAR(200)`  |   Sí | Usuario, opcional            |
| `weekly_check_state`            | `TINYINT(1)`    |   No | Sistema                      |
| `weekly_check_week`             | `VARCHAR(20)`   |   No | Usuario selecciona           |
| `brood_id`                      | `INT`           |   No | FK a `broods`                |
| `expected_value_id`             | `INT`           |   No | FK a `expected_values`       |


++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


Fórmulas definitivas para programar

| Campo                           | Fórmula o fuente                                               |
| ------------------------------- | -------------------------------------------------------------- |
| `sample_bird_quantity`          | `CEILING(final_daily_bird_balance × 0.02)`                     |
| `average_weekly_weight`         | `total_bird_weight / sample_bird_quantity`                     |
| `weekly_real_consumption`       | `final_accumulated_consumption / final_daily_bird_balance`     |
| `weekly_expected_consumption`   | `expected_values.expected_consumption`                         |
| `weekly_consumption_difference` | `weekly_real_consumption - weekly_expected_consumption`        |
| `weekly_expected_weight`        | `expected_values.expected_weight`                              |
| `weekly_weight_difference`      | `average_weekly_weight - weekly_expected_weight`               |
| `weekly_real_conversion`        | `weekly_real_consumption / average_weekly_weight`              |
| `weekly_expected_conversion`    | `expected_values.expected_conversion`                          |
| `weekly_conversion_difference`  | `weekly_real_conversion - weekly_expected_conversion`          |
| `weekly_real_mortality`         | `(final_accumulated_mortality / brood_bird_initial_num) × 100` |
| `weekly_expected_mortality`     | `expected_values.expected_mortality`                           |
| `weekly_mortality_difference`   | `weekly_real_mortality - weekly_expected_mortality`            |

| Columna                         | Tipo            | Función                  |
| ------------------------------- | --------------- | ------------------------ |
| `weekly_check_id`               | `INT` PK AI     | Identificador            |
| `sample_bird_quantity`          | `INT`           | 2 % del saldo actual     |
| `total_bird_weight`             | `DECIMAL(10,3)` | Ingresado por el usuario |
| `average_weekly_weight`         | `DECIMAL(10,3)` | Calculado                |
| `weekly_real_consumption`       | `DECIMAL(10,3)` | Calculado                |
| `weekly_expected_consumption`   | `DECIMAL(10,3)` | Copiado del catálogo     |
| `weekly_consumption_difference` | `DECIMAL(10,3)` | Real menos esperado      |
| `weekly_expected_weight`        | `DECIMAL(10,3)` | Copiado del catálogo     |
| `weekly_weight_difference`      | `DECIMAL(10,3)` | Real menos esperado      |
| `weekly_real_conversion`        | `DECIMAL(10,2)` | Calculado                |
| `weekly_expected_conversion`    | `DECIMAL(10,2)` | Copiado del catálogo     |
| `weekly_conversion_difference`  | `DECIMAL(10,2)` | Real menos esperado      |
| `weekly_real_mortality`         | `DECIMAL(10,2)` | Porcentaje calculado     |
| `weekly_expected_mortality`     | `DECIMAL(10,2)` | Copiado del catálogo     |
| `weekly_mortality_difference`   | `DECIMAL(10,2)` | Real menos esperado      |
| `weekly_check_description`      | `VARCHAR(200)`  | Opcional                 |
| `weekly_check_state`            | `TINYINT(1)`    | Estado lógico            |
| `weekly_check_week`             | `VARCHAR(20)`   | Semana 1 a Semana 6      |
| `brood_id`                      | `INT` FK        | Camada                   |
| `expected_value_id`             | `INT` FK        | Valores esperados usados |


| Campo                         | Fórmula                                             |
| ----------------------------- | --------------------------------------------------- |
| `SampleBirdQuantity`          | `Ceiling(finalBirdBalance × 0.02)`                  |
| `AverageWeeklyWeight`         | `TotalBirdWeight / SampleBirdQuantity`              |
| `WeeklyRealConsumption`       | `finalAccumulatedConsumption / finalBirdBalance`    |
| `WeeklyConsumptionDifference` | `WeeklyRealConsumption - WeeklyExpectedConsumption` |
| `WeeklyWeightDifference`      | `AverageWeeklyWeight - WeeklyExpectedWeight`        |
| `WeeklyRealConversion`        | `WeeklyRealConsumption / AverageWeeklyWeight`       |
| `WeeklyConversionDifference`  | `WeeklyRealConversion - WeeklyExpectedConversion`   |
| `WeeklyRealMortality`         | `(finalAccumulatedMortality / initialBirds) × 100`  |
| `WeeklyMortalityDifference`   | `WeeklyRealMortality - WeeklyExpectedMortality`     |





++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                                                        PESO PROMEDIO SEMANAL
                                                        
|      `Display(Name = ...)`      |       Atributo        |               Origen / ecuación                                              |
| ------------------------------- | --------------------- | ---------------------------------------------------------------------------- |
| `Cantidad Aves Muestra`         | `SampleBirdQuantity`  | `CEILING(FinalDailyBirdBalance × 0.02)`                                      |
| `Peso total de la muestra (kg)` | `TotalBirdWeight`     | Valor ingresado por el usuario después de pesar todas las aves de la muestra |
| `Peso promedio semanal (kg)`    | `AverageWeeklyWeight` | `TotalBirdWeight ÷ SampleBirdQuantity`                                       |


AverageWeeklyWeight
=
TotalBirdWeight
÷
SampleBirdQuantity










++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                                                        CONSUMO REAL / Conversion Real

FinalAccumulatedConsumption
            /
FinalDailyBirdBalance
            |
            v
WeeklyRealConsumption
            |
            +--------------------------+
            |                          |
            v                          v
WeeklyConsumptionDifference     WeeklyRealConversion
            ^                          |
            |                          v
WeeklyExpectedConsumption   WeeklyConversionDifference
                                       ^
                                       |
                             WeeklyExpectedConversion

AverageWeeklyWeight ---------> WeeklyRealConversion



+--------------------------------------+-------------------------------+--------------------------------------------------------------+
| DISPLAY                              | ATRIBUTO                      | ECUACION / ORIGEN                                            |
+--------------------------------------+-------------------------------+--------------------------------------------------------------+
| Consumo acumulado final (kg)         | FinalAccumulatedConsumption   | DailyCheck.AccumulatedConsumption del Dia 7                  |
+--------------------------------------+-------------------------------+--------------------------------------------------------------+
| Saldo actual aves                    | FinalDailyBirdBalance         | DailyCheck.DailyBirdBalance del Dia 7                        |
+--------------------------------------+-------------------------------+--------------------------------------------------------------+
| Consumo Real (kg)                    | WeeklyRealConsumption         | FinalAccumulatedConsumption / FinalDailyBirdBalance          |
|                                      |                               | Redondeo: 3 decimales                                        |
+--------------------------------------+-------------------------------+--------------------------------------------------------------+
| Consumo esperado (kg)                | WeeklyExpectedConsumption     | ExpectedValue.ExpectedConsumption                            |
|                                      |                               | Se obtiene segun WeeklyCheckWeek                             |
+--------------------------------------+-------------------------------+--------------------------------------------------------------+
| Diferencia Consumo (kg)              | WeeklyConsumptionDifference   | WeeklyRealConsumption - WeeklyExpectedConsumption            |
|                                      |                               | Redondeo: 3 decimales                                        |
+--------------------------------------+-------------------------------+--------------------------------------------------------------+
| Peso promedio semanal (kg)           | AverageWeeklyWeight           | TotalBirdWeight / SampleBirdQuantity                         |
|                                      |                               | Formula ya validada                                          |
+--------------------------------------+-------------------------------+--------------------------------------------------------------+
| Conversion Real                      | WeeklyRealConversion          | WeeklyRealConsumption / AverageWeeklyWeight                  |
|                                      |                               | Redondeo: 2 decimales                                        |
+--------------------------------------+-------------------------------+--------------------------------------------------------------+
| Conversion esperada                  | WeeklyExpectedConversion      | ExpectedValue.ExpectedConversion                             |
|                                      |                               | Se obtiene segun WeeklyCheckWeek                             |
+--------------------------------------+-------------------------------+--------------------------------------------------------------+
| Diferencia Conversion                | WeeklyConversionDifference    | WeeklyRealConversion - WeeklyExpectedConversion              |
|                                      |                               | Redondeo: 2 decimales                                        |
+--------------------------------------+-------------------------------+--------------------------------------------------------------+

WeeklyRealConsumption
=
FinalAccumulatedConsumption
/
FinalDailyBirdBalance

Consumo acumulado al Día 7 = 1,322 kg
Saldo final de aves         = 1,000 aves

WeeklyRealConsumption
= 1,322 / 1,000
= 1.322 kg por ave



++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


                                                       Mortalidad Semanal


 Brood
  |
  +--> BroodBirdInitialNum
  |       Cantidad inicial aves
  |
  |
DailyCheck - Dia 7
  |
  +--> FinalAccumulatedMortality
          Mortalidad acumulada final
                    |
                    |
                    v
       +-----------------------------+
       | WeeklyRealMortality         |
       |                             |
       | FinalAccumulatedMortality   |
       | -------------------------   | x 100
       | BroodBirdInitialNum         |
       +-----------------------------+
                    |
                    v
            Mortalidad Real (%)
                    |
                    |
ExpectedValue ------+
  |
  +--> WeeklyExpectedMortality
       = expectedValue.ExpectedMortality
                    |
                    v
       +-----------------------------+
       | WeeklyMortalityDifference   |
       |                             |
       | WeeklyRealMortality         |
       | - WeeklyExpectedMortality   |
       +-----------------------------+
                    |
                    v
       Diferencia Mortalidad (%)


+----------------------------------+-----------------------------+--------------------------------------------------------------------------+
| DISPLAY NAME                     | ATRIBUTO                    | ECUACION / FUENTE                                                        |
+----------------------------------+-----------------------------+--------------------------------------------------------------------------+
| Cantidad inicial aves            | BroodBirdInitialNum         | brood.BroodBirdInitialNum                                                |
+----------------------------------+-----------------------------+--------------------------------------------------------------------------+
| Mortalidad acumulada final       | FinalAccumulatedMortality   | finalDailyCheck.AccumulatedMortality                                     |
+----------------------------------+-----------------------------+--------------------------------------------------------------------------+
| Mortalidad esperada (%)          | WeeklyExpectedMortality     | expectedValue.ExpectedMortality                                          |
+----------------------------------+-----------------------------+--------------------------------------------------------------------------+
| Mortalidad Real (%)              | WeeklyRealMortality         | (FinalAccumulatedMortality / BroodBirdInitialNum) x 100                  |
+----------------------------------+-----------------------------+--------------------------------------------------------------------------+
| Diferencia Mortalidad (%)        | WeeklyMortalityDifference   | WeeklyRealMortality - WeeklyExpectedMortality                            |
+----------------------------------+-----------------------------+--------------------------------------------------------------------------+

WeeklyRealMortality
=
(FinalAccumulatedMortality / BroodBirdInitialNum) x 100

FinalAccumulatedMortality
=
finalDailyCheck.AccumulatedMortality


BroodBirdInitialNum
=
brood.BroodBirdInitialNum


++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                                                                                                                                                Formulas Finales 

| --------------------------------- | ----------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|                                   |                               |                                                                                                  FÓRMULAS O FUENTES                                                                                                                                |
| --------------------------------- | ----------------------------- |------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------- | --------------------------------------------------------------------------- |
| Display                           | Campo                         | DB                                                                                   | Entity / ViewModel                                                            | Variable local / Service                                                    |
| --------------------------------- | ----------------------------- | ------------------------------------------------------------------------------------ | ----------------------------------------------------------------------------- | --------------------------------------------------------------------------- |
| **Cantidad inicial aves**         | `BroodBirdInitialNum`         | `broods.brood_bird_initial_num`                                                      | `Brood.BroodBirdInitialNum` → `BroodBirdInitialNum`                           | `brood.BroodBirdInitialNum`                                                 |
| **Saldo actual aves**             | `FinalDailyBirdBalance`       | `daily_checks.daily_bird_balance`                                                    | `DailyCheck.DailyBirdBalance` → `FinalDailyBirdBalance`                       | `finalDailyCheck.DailyBirdBalance`                                          |
| **Consumo acumulado final (kg)**  | `FinalAccumulatedConsumption` | `daily_checks.accumulated_consumption`                                               | `DailyCheck.AccumulatedConsumption` → `FinalAccumulatedConsumption`           | `finalDailyCheck.AccumulatedConsumption`                                    |
| **Mortalidad acumulada final**    | `FinalAccumulatedMortality`   | `daily_checks.accumulated_mortality`                                                 | `DailyCheck.AccumulatedMortality` → `FinalAccumulatedMortality`               | `finalDailyCheck.AccumulatedMortality`                                      |
| **Cantidad Aves Muestra**         | `SampleBirdQuantity`          | `weekly_checks.sample_bird_quantity`                                                 | `WeeklyCheck.SampleBirdQuantity` → `SampleBirdQuantity`                       | `decimal.Ceiling(finalDailyCheck.DailyBirdBalance × SamplePercentage)`      |
| **Peso total de la muestra (kg)** | `TotalBirdWeight`             | `weekly_checks.total_bird_weight`                                                    | `WeeklyCheck.TotalBirdWeight` → `TotalBirdWeight`                             | `model.TotalBirdWeight`                                                     |
| **Peso promedio semanal (kg)**    | `AverageWeeklyWeight`         | `weekly_checks.average_weekly_weight`                                                | `WeeklyCheck.AverageWeeklyWeight` → `AverageWeeklyWeight`                     | `totalBirdWeight / sampleBirdQuantity`                                      |
| **Consumo esperado (kg)**         | `WeeklyExpectedConsumption`   | `expected_values.expected_consumption` → `weekly_checks.weekly_expected_consumption` | `ExpectedValue.ExpectedConsumption` → `WeeklyCheck.WeeklyExpectedConsumption` | `expectedValue.ExpectedConsumption`                                         |
| **Consumo Real (kg)**             | `WeeklyRealConsumption`       | `weekly_checks.weekly_real_consumption`                                              | `WeeklyCheck.WeeklyRealConsumption` → `WeeklyRealConsumption`                 | `finalDailyCheck.AccumulatedConsumption / finalDailyCheck.DailyBirdBalance` |
| **Diferencia Consumo (kg)**       | `WeeklyConsumptionDifference` | `weekly_checks.weekly_consumption_difference`                                        | `WeeklyCheck.WeeklyConsumptionDifference` → `WeeklyConsumptionDifference`     | `weeklyRealConsumption - expectedValue.ExpectedConsumption`                 |
| **Peso esperado (kg)**            | `WeeklyExpectedWeight`        | `expected_values.expected_weight` → `weekly_checks.weekly_expected_weight`           | `ExpectedValue.ExpectedWeight` → `WeeklyCheck.WeeklyExpectedWeight`           | `expectedValue.ExpectedWeight`                                              |
| **Diferencia de peso (kg)**       | `WeeklyWeightDifference`      | `weekly_checks.weekly_weight_difference`                                             | `WeeklyCheck.WeeklyWeightDifference` → `WeeklyWeightDifference`               | `averageWeeklyWeight - expectedValue.ExpectedWeight`                        |
| **Conversión esperada**           | `WeeklyExpectedConversion`    | `expected_values.expected_conversion` → `weekly_checks.weekly_expected_conversion`   | `ExpectedValue.ExpectedConversion` → `WeeklyCheck.WeeklyExpectedConversion`   | `expectedValue.ExpectedConversion`                                          |
| **Conversión Real**               | `WeeklyRealConversion`        | `weekly_checks.weekly_real_conversion`                                               | `WeeklyCheck.WeeklyRealConversion` → `WeeklyRealConversion`                   | `weeklyRealConsumption / averageWeeklyWeight`                               |
| **Diferencia Conversión**         | `WeeklyConversionDifference`  | `weekly_checks.weekly_conversion_difference`                                         | `WeeklyCheck.WeeklyConversionDifference` → `WeeklyConversionDifference`       | `weeklyRealConversion - expectedValue.ExpectedConversion`                   |
| **Mortalidad esperada (%)**       | `WeeklyExpectedMortality`     | `expected_values.expected_mortality` → `weekly_checks.weekly_expected_mortality`     | `ExpectedValue.ExpectedMortality` → `WeeklyCheck.WeeklyExpectedMortality`     | `expectedValue.ExpectedMortality`                                           |
| **Mortalidad Real (%)**           | `WeeklyRealMortality`         | `weekly_checks.weekly_real_mortality`                                                | `WeeklyCheck.WeeklyRealMortality` → `WeeklyRealMortality`                     | `(finalDailyCheck.AccumulatedMortality / brood.BroodBirdInitialNum) × 100`  |
| **Diferencia Mortalidad (%)**     | `WeeklyMortalityDifference`   | `weekly_checks.weekly_mortality_difference`                                          | `WeeklyCheck.WeeklyMortalityDifference` → `WeeklyMortalityDifference`         | `weeklyRealMortality - expectedValue.ExpectedMortality`                     |


DATOS FUENTE
─────────────────────────────────────────

daily_checks.accumulated_consumption
                │
                ▼
DailyCheck.AccumulatedConsumption
                │
                ▼
finalDailyCheck.AccumulatedConsumption
                │
                │
                │     daily_checks.daily_bird_balance
                │                   │
                │                   ▼
                │       DailyCheck.DailyBirdBalance
                │                   │
                │                   ▼
                │       finalDailyCheck.DailyBirdBalance
                │                   │
                └──────────┬────────┘
                           │
                           ▼
                 WeeklyRealConsumption
                           =
       finalDailyCheck.AccumulatedConsumption
       ──────────────────────────────────────
          finalDailyCheck.DailyBirdBalance
                           │
                           ▼
             WeeklyCheck.WeeklyRealConsumption
                           │
                           ▼
       weekly_checks.weekly_real_consumption


| Display                           | Campo                         | Fórmula o Fuente                                                            |
| --------------------------------- | ----------------------------- | --------------------------------------------------------------------------- |
| **Cantidad inicial aves**         | `BroodBirdInitialNum`         | `brood.BroodBirdInitialNum`                                                 |
| **Saldo actual aves**             | `FinalDailyBirdBalance`       | `finalDailyCheck.DailyBirdBalance`                                          |
| **Consumo acumulado final (kg)**  | `FinalAccumulatedConsumption` | `finalDailyCheck.AccumulatedConsumption`                                    |
| **Mortalidad acumulada final**    | `FinalAccumulatedMortality`   | `finalDailyCheck.AccumulatedMortality`                                      |
| **Cantidad Aves Muestra**         | `SampleBirdQuantity`          | `Ceiling(finalDailyCheck.DailyBirdBalance × 0.02)`                          |
| **Peso total de la muestra (kg)** | `TotalBirdWeight`             | `model.TotalBirdWeight` — ingresado por el usuario                          |
| **Peso promedio semanal (kg)**    | `AverageWeeklyWeight`         | `model.TotalBirdWeight / sampleBirdQuantity`                                |
| **Consumo esperado (kg)**         | `WeeklyExpectedConsumption`   | `expectedValue.ExpectedConsumption`                                         |
| **Consumo Real (kg)**             | `WeeklyRealConsumption`       | `finalDailyCheck.AccumulatedConsumption / finalDailyCheck.DailyBirdBalance` |
| **Diferencia Consumo (kg)**       | `WeeklyConsumptionDifference` | `weeklyRealConsumption - weeklyExpectedConsumption`                         |
| **Peso esperado (kg)**            | `WeeklyExpectedWeight`        | `expectedValue.ExpectedWeight`                                              |
| **Diferencia de peso (kg)**       | `WeeklyWeightDifference`      | `averageWeeklyWeight - weeklyExpectedWeight`                                |
| **Conversión esperada**           | `WeeklyExpectedConversion`    | `expectedValue.ExpectedConversion`                                          |
| **Conversión Real**               | `WeeklyRealConversion`        | `weeklyRealConsumption / averageWeeklyWeight`                               |
| **Diferencia Conversión**         | `WeeklyConversionDifference`  | `weeklyRealConversion - weeklyExpectedConversion`                           |
| **Mortalidad esperada (%)**       | `WeeklyExpectedMortality`     | `expectedValue.ExpectedMortality`                                           |
| **Mortalidad Real (%)**           | `WeeklyRealMortality`         | `(finalDailyCheck.AccumulatedMortality / brood.BroodBirdInitialNum) × 100`  |
| **Diferencia Mortalidad (%)**     | `WeeklyMortalityDifference`   | `weeklyRealMortality - weeklyExpectedMortality`                             |




++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


Models/ViewModels/BroodReport/
│
├── BroodReportFormViewModel
│      ↓
│    GENERAR
│
├── BroodReportListViewModel
│      ↓
│     INDEX
│
├── BroodReportGetByIdViewModel
│      ↓
│    DETAILS
│
├── BroodReportSnapshotViewModel
│      ↓
│   SNAPSHOT COMPLETO
│
├── BroodReportHeaderViewModel
│      ↓
│   ENCABEZADO PDF
│
├── BroodReportDailyRowViewModel
│      ↓
│   FILAS DIARIAS PDF
│
└── BroodReportWeeklyViewModel
       ↓
   CONTROL SEMANAL PDF




   BroodReport generado
        ↓
Snapshot guardado en brood_reports
        ↓
Usuario abre Details
        ↓
Presiona "Generar PDF"
        ↓
BroodReportsController
        ↓
BroodReportService
        ↓
Lee BroodReportData
        ↓
Deserializa Snapshot
        ↓
Construye PDF
        ↓
Controller devuelve archivo PDF
        ↓
Navegador
   ├── Visualizar
   ├── Imprimir
   └── Guardar


brood_reports
├── brood_report_id
├── report_number
├── generated_at
├── brood_report_version
├── brood_report_data      ← JSON histórico
└── brood_id

GeneratePdfAsync(id)
        ↓
Buscar BroodReport
        ↓
Obtener BroodReportData
        ↓
JsonSerializer.Deserialize
        ↓
BroodReportSnapshotViewModel
        ↓
Construir:
├── Header
├── 45 filas
├── 6 controles semanales
└── Footer vacío
        ↓
byte[]






++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++






++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++






++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++






++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++






++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++






++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



