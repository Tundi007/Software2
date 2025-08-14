-- db/mysql-init/01-create-dbs.sql
CREATE DATABASE IF NOT EXISTS `IAMDb` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
CREATE DATABASE IF NOT EXISTS `StorageDb` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
-- (optional) create app users instead of using root:
-- CREATE USER IF NOT EXISTS 'iamuser'@'%' IDENTIFIED BY 'IamPass!123';
-- CREATE USER IF NOT EXISTS 'storageuser'@'%' IDENTIFIED BY 'StorPass!123';
-- GRANT ALL PRIVILEGES ON IAMDb.*     TO 'iamuser'@'%';
-- GRANT ALL PRIVILEGES ON StorageDb.* TO 'storageuser'@'%';
-- FLUSH PRIVILEGES;