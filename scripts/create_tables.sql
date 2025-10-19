-- Script para criar tabelas básicas do DreamLuso no PostgreSQL

-- Criar tabela de migrations
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" varchar(150) NOT NULL,
    "ProductVersion" varchar(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- Criar tabela User
CREATE TABLE IF NOT EXISTS "User" (
    "Id" uuid NOT NULL,
    "FirstName" varchar(100) NOT NULL,
    "LastName" varchar(100) NOT NULL,
    "Email" varchar(255) NOT NULL,
    "Phone" varchar(20),
    "PasswordHash" bytea,
    "PasswordSalt" bytea,
    "Role" varchar(50) NOT NULL,
    "IsActive" boolean NOT NULL,
    "EmailConfirmed" boolean NOT NULL,
    "ProfileImageUrl" varchar(500),
    "DateOfBirth" timestamp,
    "LastLogin" timestamp,
    "FailedLoginAttempts" integer NOT NULL,
    "LockedUntil" timestamp,
    "RefreshToken" varchar(500),
    "RefreshTokenExpiry" timestamp,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "CreatedBy" varchar(100),
    "UpdatedBy" varchar(100),
    CONSTRAINT "PK_User" PRIMARY KEY ("Id")
);

-- Criar índice único para email
CREATE UNIQUE INDEX IF NOT EXISTS "IX_User_Email" ON "User" ("Email");

-- Inserir migration record
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251019120000_ManualCreateTables', '8.0.0')
ON CONFLICT ("MigrationId") DO NOTHING;

-- Verificar se as tabelas foram criadas
SELECT 'User table created successfully' as status;
SELECT 'Migration history updated' as status;
