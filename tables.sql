
CREATE DATABASE granja_la_flor;
USE granja_la_flor;

-- =========================================================
-- TABLE: roles
-- =========================================================

CREATE TABLE roles (
    role_id INT AUTO_INCREMENT PRIMARY KEY,
    role_name VARCHAR(30) NOT NULL UNIQUE,
    role_description VARCHAR(200),
    role_state TINYINT(1) NOT NULL DEFAULT 1
);

-- =========================================================
-- TABLE: users
-- =========================================================

CREATE TABLE users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    user_name VARCHAR(50) NOT NULL,
    user_email VARCHAR(50) NOT NULL UNIQUE,
    user_password VARCHAR(30) NOT NULL,
    user_description VARCHAR(150),
    user_state TINYINT(1) NOT NULL DEFAULT 1,
    role_id INT NOT NULL,

    FOREIGN KEY (role_id)
    REFERENCES roles(role_id)
);













-- =========================================================
-- TABLE: broiler_houses
-- =========================================================

CREATE TABLE broiler_houses (
    broiler_house_id INT AUTO_INCREMENT PRIMARY KEY,
    broiler_house_name VARCHAR(50) NOT NULL UNIQUE,
    broiler_house_description VARCHAR(200),
    broiler_house_state TINYINT(1) NOT NULL DEFAULT 1
);

-- =========================================================
-- TABLE: broods
-- =========================================================

CREATE TABLE broods (
    brood_id INT AUTO_INCREMENT PRIMARY KEY,
    brood_name VARCHAR(50) NOT NULL,
    brood_number INT NOT NULL,
    brood_date DATE NOT NULL,
    brood_bird_initial_num INT NOT NULL,
    brood_description VARCHAR(150),
    brood_state TINYINT(1) NOT NULL DEFAULT 1,
    broiler_house_id INT NOT NULL,

    FOREIGN KEY (broiler_house_id)
    REFERENCES broiler_houses(broiler_house_id)
);

-- =========================================================
-- TABLE: dc_days
-- =========================================================

CREATE TABLE dc_days (
    dc_day_id INT AUTO_INCREMENT PRIMARY KEY,
    dc_day_name VARCHAR(50) NOT NULL UNIQUE,
    dc_day_number INT NOT NULL,
    dc_day_state TINYINT(1) NOT NULL DEFAULT 1
);








-- =========================================================
-- TABLE: wc_weeks
-- =========================================================

CREATE TABLE wc_weeks (
    wc_week_id INT AUTO_INCREMENT PRIMARY KEY,
    wc_week_name VARCHAR(50) NOT NULL UNIQUE,
    wc_week_number INT NOT NULL,
    wc_week_state TINYINT(1) NOT NULL DEFAULT 1
);

-- =========================================================
-- TABLE: expected_values
-- =========================================================

CREATE TABLE expected_values (
    expected_value_id INT AUTO_INCREMENT PRIMARY KEY,
    week_number INT NOT NULL,
    expected_consumption DECIMAL(10,2) NOT NULL,
    expected_weight DECIMAL(10,2) NOT NULL,
    expected_conversion DECIMAL(10,2) NOT NULL,
    expected_mortality DECIMAL(10,2) NOT NULL,
    expected_value_state TINYINT(1) NOT NULL DEFAULT 1
);

-- =========================================================
-- TABLE: income_concentrates
-- =========================================================

CREATE TABLE income_concentrates (
    income_concentrate_id INT AUTO_INCREMENT PRIMARY KEY,
    income_concentrate_date DATE NOT NULL,
    income_quitants DECIMAL(10,2) NOT NULL,
    income_kilos DECIMAL(10,2) NOT NULL,
    income_accumulated DECIMAL(10,2) NOT NULL,
    income_description VARCHAR(200),
    income_state TINYINT(1) NOT NULL DEFAULT 1,
    brood_id INT NOT NULL,

    FOREIGN KEY (brood_id)
    REFERENCES broods(brood_id)
);





-- =========================================================
-- TABLE: daily_checks
-- =========================================================

CREATE TABLE daily_checks (
    daily_check_id INT AUTO_INCREMENT PRIMARY KEY,
    daily_check_date DATE NOT NULL,
    natural_mortality INT NOT NULL,
    select_quantity INT NOT NULL,
    total_daily_mortality INT NOT NULL,
    accumulated_mortality INT NOT NULL,
    daily_bird_balance INT NOT NULL,
    consumption_quintals DECIMAL(10,2) NOT NULL,
    consumption_kilos DECIMAL(10,2) NOT NULL,
    accumulated_consumption DECIMAL(10,2) NOT NULL,
    concentrate_balance DECIMAL(10,2) NOT NULL,
    daily_check_description VARCHAR(200),
    daily_check_state TINYINT(1) NOT NULL DEFAULT 1,
    brood_id INT NOT NULL,
    dc_day_id INT NOT NULL,
    income_concentrate_id INT NOT NULL,

    FOREIGN KEY (brood_id)
    REFERENCES broods(brood_id),

    FOREIGN KEY (dc_day_id)
    REFERENCES dc_days(dc_day_id),

    FOREIGN KEY (income_concentrate_id)
    REFERENCES income_concentrates(income_concentrate_id)
);















-- =========================================================
-- TABLE: weekly_checks
-- =========================================================

CREATE TABLE weekly_checks (
    weekly_check_id INT AUTO_INCREMENT PRIMARY KEY,
    sample_bird_quantity INT NOT NULL,
    total_bird_weight DECIMAL(10,2) NOT NULL,
    average_weekly_weight DECIMAL(10,2) NOT NULL,
    weekly_real_consumption DECIMAL(10,2) NOT NULL,
    weekly_consumption_difference DECIMAL(10,2) NOT NULL,
    weekly_expected_weight DECIMAL(10,2) NOT NULL,
    weekly_weight_difference DECIMAL(10,2) NOT NULL,
    weekly_real_conversion DECIMAL(10,2) NOT NULL,
    weekly_conversion_difference DECIMAL(10,2) NOT NULL,
    weekly_real_mortality DECIMAL(10,2) NOT NULL,
    weekly_mortality_difference DECIMAL(10,2) NOT NULL,
    weekly_check_description VARCHAR(200),
    weekly_check_state TINYINT(1) NOT NULL DEFAULT 1,
    brood_id INT NOT NULL,
    wc_week_id INT NOT NULL,
    expected_value_id INT NOT NULL,

    FOREIGN KEY (brood_id)
    REFERENCES broods(brood_id),

    FOREIGN KEY (wc_week_id)
    REFERENCES wc_weeks(wc_week_id),

    FOREIGN KEY (expected_value_id)
    REFERENCES expected_values(expected_value_id)
);



ALTER USER 'root'@'localhost'
IDENTIFIED BY '123456789';

ALTER USER 'root'@'localhost'
IDENTIFIED BY 'root123!';

