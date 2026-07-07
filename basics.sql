USE GranjaLaFlor;

CREATE SCHEMA `GranjaLaFlor`;

DESCRIBE broods;

SHOW COLUMNS FROM nombre_tabla;

SELECT *
FROM nombre_tabla;

ALTER TABLE nombre_tabla
ADD nueva_columna VARCHAR(100);

ALTER TABLE nombre_tabla
MODIFY columna VARCHAR(150);

ALTER TABLE nombre_tabla
RENAME COLUMN nombre_viejo TO nombre_nuevo;

ALTER TABLE nombre_tabla
DROP COLUMN nombre_columna;

UPDATE broods
SET brood_state = 1
WHERE brood_id = 5;

DELETE FROM broodss;

TRUNCATE TABLE broodss;

DROP TABLE broodss;

SELECT brood_name
FROM broods;

SHOW DATABASES;


SHOW TABLES;

