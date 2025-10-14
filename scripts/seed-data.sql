-- ============================================
-- DREAMLUSO - SEED DATA SCRIPT
-- Popula base de dados com dados de teste
-- ============================================

USE DreamLusoDB;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT 'Iniciando seed de dados do DreamLuso...';
GO

-- ============================================
-- 1. USUÁRIOS (ADMIN E AGENTES)
-- ============================================

PRINT 'Inserindo usuários...';

-- Admin User
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
INSERT INTO Users 
(Id, Email, PasswordHash, PasswordSalt, Account, Access, FirstName, LastName, 
 PhoneNumber, DateOfBirth, ImageUrl, IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES 
(@AdminId, 'admin@dreamluso.pt', 
 -- Password: Admin@123
 0x5A9B7C3D1E4F6A2B8C9D0E1F2A3B4C5D6E7F8A9B0C1D2E3F4A5B6C7D8E9F0A1B, 
 0x1A2B3C4D5E6F7A8B9C0D1E2F3A4B5C6D,
 'Admin', 'Full', 'Administrador', 'Sistema', 
 '912345678', '1990-01-01', NULL, 1, 0, GETUTCDATE(), 'system');

-- Real Estate Agent 1
DECLARE @Agent1Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO Users 
(Id, Email, PasswordHash, PasswordSalt, Account, Access, FirstName, LastName, 
 PhoneNumber, DateOfBirth, ImageUrl, IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES 
(@Agent1Id, 'ana.costa@dreamluso.pt', 
 0x5A9B7C3D1E4F6A2B8C9D0E1F2A3B4C5D6E7F8A9B0C1D2E3F4A5B6C7D8E9F0A1B, 
 0x1A2B3C4D5E6F7A8B9C0D1E2F3A4B5C6D,
 'Agent', 'Full', 'Ana', 'Costa', 
 '963258741', '1988-05-15', NULL, 1, 0, GETUTCDATE(), 'system');

-- Real Estate Agent 2
DECLARE @Agent2Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO Users 
(Id, Email, PasswordHash, PasswordSalt, Account, Access, FirstName, LastName, 
 PhoneNumber, DateOfBirth, ImageUrl, IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES 
(@Agent2Id, 'ricardo.silva@dreamluso.pt', 
 0x5A9B7C3D1E4F6A2B8C9D0E1F2A3B4C5D6E7F8A9B0C1D2E3F4A5B6C7D8E9F0A1B, 
 0x1A2B3C4D5E6F7A8B9C0D1E2F3A4B5C6D,
 'Agent', 'Full', 'Ricardo', 'Silva', 
 '925874136', '1985-11-22', NULL, 1, 0, GETUTCDATE(), 'system');

PRINT 'Usuários inseridos com sucesso!';
GO

-- ============================================
-- 2. CLIENTES
-- ============================================

PRINT 'Inserindo clientes...';

-- Cliente 1
DECLARE @Client1Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO Users 
(Id, Email, PasswordHash, PasswordSalt, Account, Access, FirstName, LastName, 
 PhoneNumber, DateOfBirth, ImageUrl, IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES 
(@Client1Id, 'joao.martins@email.pt', 
 0x5A9B7C3D1E4F6A2B8C9D0E1F2A3B4C5D6E7F8A9B0C1D2E3F4A5B6C7D8E9F0A1B, 
 0x1A2B3C4D5E6F7A8B9C0D1E2F3A4B5C6D,
 'Client', 'Limited', 'João', 'Martins', 
 '912456789', '1992-03-10', NULL, 1, 0, GETUTCDATE(), 'system');

INSERT INTO Clients (Id, UserId, IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES (NEWID(), @Client1Id, 1, 0, GETUTCDATE(), 'system');

-- Cliente 2
DECLARE @Client2Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO Users 
(Id, Email, PasswordHash, PasswordSalt, Account, Access, FirstName, LastName, 
 PhoneNumber, DateOfBirth, ImageUrl, IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES 
(@Client2Id, 'maria.santos@email.pt', 
 0x5A9B7C3D1E4F6A2B8C9D0E1F2A3B4C5D6E7F8A9B0C1D2E3F4A5B6C7D8E9F0A1B, 
 0x1A2B3C4D5E6F7A8B9C0D1E2F3A4B5C6D,
 'Client', 'Limited', 'Maria', 'Santos', 
 '934567890', '1987-07-25', NULL, 1, 0, GETUTCDATE(), 'system');

INSERT INTO Clients (Id, UserId, IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES (NEWID(), @Client2Id, 1, 0, GETUTCDATE(), 'system');

PRINT 'Clientes inseridos com sucesso!';
GO

-- ============================================
-- 3. AGENTES IMOBILIÁRIOS
-- ============================================

PRINT 'Inserindo agentes imobiliários...';

-- Recuperar IDs dos agentes
DECLARE @Agent1UserId UNIQUEIDENTIFIER = (SELECT Id FROM Users WHERE Email = 'ana.costa@dreamluso.pt');
DECLARE @Agent2UserId UNIQUEIDENTIFIER = (SELECT Id FROM Users WHERE Email = 'ricardo.silva@dreamluso.pt');

INSERT INTO RealEstateAgents 
(Id, UserId, OfficeEmail, TotalSales, TotalListings, Certifications, LanguagesSpoken, 
 IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES 
(NEWID(), @Agent1UserId, 'ana.costa@dreamluso.pt', 15, 45, 
 'Certificação AMI, Especialização em Luxo', 'Português, Inglês, Francês',
 1, 0, GETUTCDATE(), 'system');

INSERT INTO RealEstateAgents 
(Id, UserId, OfficeEmail, TotalSales, TotalListings, Certifications, LanguagesSpoken, 
 IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES 
(NEWID(), @Agent2UserId, 'ricardo.silva@dreamluso.pt', 22, 68, 
 'Certificação AMI, Especialização Comercial', 'Português, Inglês, Espanhol',
 1, 0, GETUTCDATE(), 'system');

PRINT 'Agentes imobiliários inseridos com sucesso!';
GO

-- ============================================
-- 4. PROPRIEDADES
-- ============================================

PRINT 'Inserindo propriedades...';

-- Propriedade 1 - Apartamento T2 Porto
DECLARE @Property1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Agent1RealEstateId UNIQUEIDENTIFIER = (SELECT Id FROM RealEstateAgents WHERE OfficeEmail = 'ana.costa@dreamluso.pt');

INSERT INTO Properties
(Id, Title, Description, RealEstateAgentId, Type, Size, Bedrooms, Bathrooms, Price, 
 Status, DateListed, YearBuilt, IsActive, IsDeleted, CreatedAt, CreatedBy,
 Street, Number, Parish, Municipality, District, PostalCode, Complement)
VALUES 
(@Property1Id, 'Apartamento T2 Moderno no Centro do Porto', 
 'Apartamento completamente renovado, com vista para o rio Douro. Cozinha equipada, sala ampla com varanda, 2 quartos com roupeiros embutidos. Localização premium.',
 @Agent1RealEstateId, 'Apartment', 85.0, 2, 1, 295000.00,
 'Available', GETUTCDATE(), 2018, 1, 0, GETUTCDATE(), 'system',
 'Rua de Santa Catarina', '150', 'Santo Ildefonso', 'Porto', 'Porto', '4000-442', '3º Dto');

-- Propriedade 2 - Moradia T4 Cascais
DECLARE @Property2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Agent2RealEstateId UNIQUEIDENTIFIER = (SELECT Id FROM RealEstateAgents WHERE OfficeEmail = 'ricardo.silva@dreamluso.pt');

INSERT INTO Properties
(Id, Title, Description, RealEstateAgentId, Type, Size, Bedrooms, Bathrooms, Price, 
 Status, DateListed, YearBuilt, IsActive, IsDeleted, CreatedAt, CreatedBy,
 Street, Number, Parish, Municipality, District, PostalCode, Complement)
VALUES 
(@Property2Id, 'Moradia T4 com Piscina em Cascais', 
 'Magnífica moradia com 4 quartos, piscina aquecida, jardim privativo e garagem para 3 carros. Acabamentos de luxo, domotica completa. A 5min da praia.',
 @Agent2RealEstateId, 'House', 280.0, 4, 3, 875000.00,
 'Available', GETUTCDATE(), 2020, 1, 0, GETUTCDATE(), 'system',
 'Avenida Marginal', '2500', 'Cascais', 'Cascais', 'Lisboa', '2750-310', NULL);

-- Propriedade 3 - Escritório Lisboa
DECLARE @Property3Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO Properties
(Id, Title, Description, RealEstateAgentId, Type, Size, Bedrooms, Bathrooms, Price, 
 Status, DateListed, YearBuilt, IsActive, IsDeleted, CreatedAt, CreatedBy,
 Street, Number, Parish, Municipality, District, PostalCode, Complement)
VALUES 
(@Property3Id, 'Escritório Moderno na Avenida da Liberdade', 
 'Espaço comercial de 150m² em edifício premium. Open space configurável, 2 WCs, kitchenette. Ar condicionado central. Estacionamento disponível.',
 @Agent1RealEstateId, 'Commercial', 150.0, 0, 2, 3500.00,
 'Available', GETUTCDATE(), 2021, 1, 0, GETUTCDATE(), 'system',
 'Avenida da Liberdade', '195', 'Santo António', 'Lisboa', 'Lisboa', '1250-142', '5º Andar');

PRINT 'Propriedades inseridas com sucesso!';
GO

PRINT '============================================';
PRINT 'SEED DATA CONCLUÍDO COM SUCESSO!';
PRINT 'Total: 3 Users (1 Admin, 2 Agents), 2 Clients, 2 Agents, 3 Properties';
PRINT '============================================';
GO

