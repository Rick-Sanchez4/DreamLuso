-- Script de inicialização do banco SQL Server local
-- Este script é executado automaticamente quando o container é criado

-- Criar banco de dados se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DreamLusoDB')
BEGIN
    CREATE DATABASE DreamLusoDB;
    PRINT 'Database DreamLusoDB created successfully!';
END
ELSE
BEGIN
    PRINT 'Database DreamLusoDB already exists.';
END

-- Usar o banco DreamLusoDB
USE DreamLusoDB;

-- Criar usuário específico para a aplicação (opcional)
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'dreamluso_user')
BEGIN
    CREATE LOGIN dreamluso_user WITH PASSWORD = 'DreamLuso2025!';
    PRINT 'Login dreamluso_user created successfully!';
END
ELSE
BEGIN
    PRINT 'Login dreamluso_user already exists.';
END

-- Dar permissões ao usuário no banco
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'dreamluso_user')
BEGIN
    CREATE USER dreamluso_user FOR LOGIN dreamluso_user;
    ALTER ROLE db_owner ADD MEMBER dreamluso_user;
    PRINT 'User dreamluso_user created and granted permissions!';
END
ELSE
BEGIN
    PRINT 'User dreamluso_user already exists in database.';
END

-- Configurar collation se necessário
-- ALTER DATABASE DreamLusoDB COLLATE SQL_Latin1_General_CP1_CI_AS;

PRINT 'DreamLuso SQL Server Database initialized successfully!';
PRINT 'Database: DreamLusoDB';
PRINT 'Server: localhost:1433';
PRINT 'SA Password: DreamLuso2025!';
PRINT 'Application User: dreamluso_user / DreamLuso2025!';
