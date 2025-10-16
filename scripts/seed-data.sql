-- =====================================================
-- DreamLuso - Script de SEED com Dados de Exemplo
-- =====================================================
-- Criado em: 15/10/2025
-- Propósito: Popular banco de dados Azure SQL com dados realistas
-- =====================================================

USE DreamLusoDB;
GO

-- Limpar dados existentes (CUIDADO: apenas para ambiente de DEV/TEST!)
PRINT '🧹 Limpando dados existentes...';
DELETE FROM [DreamLuso].[Notifications];
DELETE FROM [DreamLuso].[ProposalNegotiations];
DELETE FROM [DreamLuso].[PropertyProposals];
DELETE FROM [DreamLuso].[Comments];
DELETE FROM [DreamLuso].[Contracts];
DELETE FROM [DreamLuso].[PropertyVisits];
DELETE FROM [DreamLuso].[PropertyImages];
DELETE FROM [DreamLuso].[Properties];
DELETE FROM [DreamLuso].[RealEstateAgents];
DELETE FROM [DreamLuso].[Clients];
DELETE FROM [DreamLuso].[Users];
GO

PRINT '✅ Dados antigos limpos!';
GO

-- =====================================================
-- 1️⃣ USUÁRIOS
-- =====================================================
PRINT '👤 Criando usuários...';

-- Nota: Password hash para "Admin123!" e senhas genéricas
-- Em produção, use hashing real (HMACSHA512)

DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
DECLARE @Agent1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Agent2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Agent3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Client1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Client2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Client3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Client4Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Client5Id UNIQUEIDENTIFIER = NEWID();

-- Admin
INSERT INTO [DreamLuso].[Users] (Id, Email, PasswordHash, PasswordSalt, Role, IsActive, EmailConfirmed, CreatedAt, UpdatedAt, IsDeleted, FirstName, LastName)
VALUES (
    @AdminId,
    'admin@gmail.com',
    0x4164000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, -- Hash fake
    0x53616C74, -- Salt fake
    'Admin',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Admin',
    'DreamLuso'
);

-- Agente 1: João Silva
INSERT INTO [DreamLuso].[Users] (Id, Email, PasswordHash, PasswordSalt, Role, IsActive, EmailConfirmed, CreatedAt, UpdatedAt, IsDeleted, FirstName, LastName)
VALUES (
    @Agent1Id,
    'joao.silva@dreamluso.pt',
    0x4164000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000,
    0x53616C74,
    'RealEstateAgent',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'João',
    'Silva'
);

-- Agente 2: Maria Santos
INSERT INTO [DreamLuso].[Users] (Id, Email, PasswordHash, PasswordSalt, Role, IsActive, EmailConfirmed, CreatedAt, UpdatedAt, IsDeleted, FirstName, LastName)
VALUES (
    @Agent2Id,
    'maria.santos@dreamluso.pt',
    0x4164000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000,
    0x53616C74,
    'RealEstateAgent',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Maria',
    'Santos'
);

-- Agente 3: Pedro Costa
INSERT INTO [DreamLuso].[Users] (Id, Email, PasswordHash, PasswordSalt, Role, IsActive, EmailConfirmed, CreatedAt, UpdatedAt, IsDeleted, FirstName, LastName)
VALUES (
    @Agent3Id,
    'pedro.costa@dreamluso.pt',
    0x4164000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000,
    0x53616C74,
    'RealEstateAgent',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Pedro',
    'Costa'
);

-- Cliente 1: Ana Rodrigues
INSERT INTO [DreamLuso].[Users] (Id, Email, PasswordHash, PasswordSalt, Role, IsActive, EmailConfirmed, CreatedAt, UpdatedAt, IsDeleted, FirstName, LastName)
VALUES (
    @Client1Id,
    'ana.rodrigues@email.com',
    0x4164000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000,
    0x53616C74,
    'Client',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Ana',
    'Rodrigues'
);

-- Cliente 2: Carlos Ferreira
INSERT INTO [DreamLuso].[Users] (Id, Email, PasswordHash, PasswordSalt, Role, IsActive, EmailConfirmed, CreatedAt, UpdatedAt, IsDeleted, FirstName, LastName)
VALUES (
    @Client2Id,
    'carlos.ferreira@email.com',
    0x4164000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000,
    0x53616C74,
    'Client',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Carlos',
    'Ferreira'
);

-- Cliente 3: Sofia Almeida
INSERT INTO [DreamLuso].[Users] (Id, Email, PasswordHash, PasswordSalt, Role, IsActive, EmailConfirmed, CreatedAt, UpdatedAt, IsDeleted, FirstName, LastName)
VALUES (
    @Client3Id,
    'sofia.almeida@email.com',
    0x4164000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000,
    0x53616C74,
    'Client',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Sofia',
    'Almeida'
);

-- Cliente 4: Miguel Oliveira
INSERT INTO [DreamLuso].[Users] (Id, Email, PasswordHash, PasswordSalt, Role, IsActive, EmailConfirmed, CreatedAt, UpdatedAt, IsDeleted, FirstName, LastName)
VALUES (
    @Client4Id,
    'miguel.oliveira@email.com',
    0x4164000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000,
    0x53616C74,
    'Client',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Miguel',
    'Oliveira'
);

-- Cliente 5: Beatriz Lopes
INSERT INTO [DreamLuso].[Users] (Id, Email, PasswordHash, PasswordSalt, Role, IsActive, EmailConfirmed, CreatedAt, UpdatedAt, IsDeleted, FirstName, LastName)
VALUES (
    @Client5Id,
    'beatriz.lopes@email.com',
    0x4164000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000,
    0x53616C74,
    'Client',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Beatriz',
    'Lopes'
);

PRINT '✅ 9 Usuários criados (1 Admin, 3 Agentes, 5 Clientes)';
GO

-- =====================================================
-- 2️⃣ AGENTES IMOBILIÁRIOS
-- =====================================================
PRINT '🏢 Criando agentes imobiliários...';

DECLARE @Agent1RecId UNIQUEIDENTIFIER = NEWID();
DECLARE @Agent2RecId UNIQUEIDENTIFIER = NEWID();
DECLARE @Agent3RecId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [DreamLuso].[RealEstateAgents] (Id, UserId, LicenseNumber, Specialization, YearsOfExperience, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (@Agent1RecId, (SELECT Id FROM [DreamLuso].[Users] WHERE Email = 'joao.silva@dreamluso.pt'), 'AMI-12345', 'Residencial', 8, GETUTCDATE(), GETUTCDATE(), 0),
    (@Agent2RecId, (SELECT Id FROM [DreamLuso].[Users] WHERE Email = 'maria.santos@dreamluso.pt'), 'AMI-23456', 'Luxury', 12, GETUTCDATE(), GETUTCDATE(), 0),
    (@Agent3RecId, (SELECT Id FROM [DreamLuso].[Users] WHERE Email = 'pedro.costa@dreamluso.pt'), 'AMI-34567', 'Comercial', 5, GETUTCDATE(), GETUTCDATE(), 0);

PRINT '✅ 3 Agentes imobiliários criados';
GO

-- =====================================================
-- 3️⃣ CLIENTES
-- =====================================================
PRINT '👥 Criando clientes...';

INSERT INTO [DreamLuso].[Clients] (Id, UserId, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (NEWID(), (SELECT Id FROM [DreamLuso].[Users] WHERE Email = 'ana.rodrigues@email.com'), GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), (SELECT Id FROM [DreamLuso].[Users] WHERE Email = 'carlos.ferreira@email.com'), GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), (SELECT Id FROM [DreamLuso].[Users] WHERE Email = 'sofia.almeida@email.com'), GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), (SELECT Id FROM [DreamLuso].[Users] WHERE Email = 'miguel.oliveira@email.com'), GETUTCDATE(), GETUTCDATE(), 0),
    (NEWID(), (SELECT Id FROM [DreamLuso].[Users] WHERE Email = 'beatriz.lopes@email.com'), GETUTCDATE(), GETUTCDATE(), 0);

PRINT '✅ 5 Clientes criados';
GO

-- =====================================================
-- 4️⃣ PROPRIEDADES
-- =====================================================
PRINT '🏠 Criando propriedades...';

DECLARE @Prop1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Prop2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Prop3 UNIQUEIDENTIFIER = NEWID();
DECLARE @Prop4 UNIQUEIDENTIFIER = NEWID();
DECLARE @Prop5 UNIQUEIDENTIFIER = NEWID();

-- Propriedade 1: Apartamento T3 em Lisboa
INSERT INTO [DreamLuso].[Properties] (
    Id, Title, Description, Type, TransactionType, Price, Area, Bedrooms, Bathrooms, 
    HasGarage, Municipality, Parish, YearBuilt, EnergyRating, Status, IsFeatured,
    RealEstateAgentId, CreatedAt, UpdatedAt, IsDeleted, Street, PostalCode, Floor, Condition
)
VALUES (
    @Prop1,
    'Apartamento T3 Moderno em Alvalade',
    'Excelente apartamento T3 totalmente renovado, com acabamentos de luxo, cozinha equipada e varanda. Localização premium em Alvalade, perto de transportes e comércio.',
    'Apartment',
    'Sale',
    385000.00,
    120.5,
    3,
    2,
    1,
    'Lisboa',
    'Alvalade',
    2015,
    'A',
    'Available',
    1,
    (SELECT Id FROM [DreamLuso].[RealEstateAgents] WHERE LicenseNumber = 'AMI-12345'),
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Rua Professor Reinaldo dos Santos, 15',
    '1700-400',
    3,
    'Renovated'
);

-- Propriedade 2: Moradia V4 no Porto
INSERT INTO [DreamLuso].[Properties] (
    Id, Title, Description, Type, TransactionType, Price, Area, Bedrooms, Bathrooms, 
    HasGarage, Municipality, Parish, YearBuilt, EnergyRating, Status, IsFeatured,
    RealEstateAgentId, CreatedAt, UpdatedAt, IsDeleted, Street, PostalCode, Condition
)
VALUES (
    @Prop2,
    'Moradia V4 com Jardim e Piscina - Porto',
    'Magnífica moradia V4 isolada, com jardim privativo de 500m², piscina aquecida e garagem para 3 viaturas. Excelente exposição solar e vistas desafogadas.',
    'House',
    'Sale',
    650000.00,
    280.0,
    4,
    3,
    1,
    'Porto',
    'Foz do Douro',
    2018,
    'A+',
    'Available',
    1,
    (SELECT Id FROM [DreamLuso].[RealEstateAgents] WHERE LicenseNumber = 'AMI-23456'),
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Rua do Molhe, 234',
    '4150-498',
    'New'
);

-- Propriedade 3: Apartamento T2 para arrendar em Cascais
INSERT INTO [DreamLuso].[Properties] (
    Id, Title, Description, Type, TransactionType, Price, Area, Bedrooms, Bathrooms, 
    HasGarage, Municipality, Parish, YearBuilt, EnergyRating, Status, IsFeatured,
    RealEstateAgentId, CreatedAt, UpdatedAt, IsDeleted, Street, PostalCode, Floor, Condition
)
VALUES (
    @Prop3,
    'T2 Junto à Praia de Cascais',
    'Apartamento T2 mobilado e equipado, a 200m da praia. Perfeito para quem procura qualidade de vida junto ao mar. Condomínio com piscina.',
    'Apartment',
    'Rent',
    1200.00,
    85.0,
    2,
    2,
    1,
    'Cascais',
    'Cascais e Estoril',
    2020,
    'A',
    'Available',
    1,
    (SELECT Id FROM [DreamLuso].[RealEstateAgents] WHERE LicenseNumber = 'AMI-34567'),
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Avenida Marginal, 1456',
    '2750-353',
    2,
    'New'
);

-- Propriedade 4: Loja comercial em Braga
INSERT INTO [DreamLuso].[Properties] (
    Id, Title, Description, Type, TransactionType, Price, Area, Bedrooms, Bathrooms, 
    HasGarage, Municipality, Parish, YearBuilt, EnergyRating, Status, IsFeatured,
    RealEstateAgentId, CreatedAt, UpdatedAt, IsDeleted, Street, PostalCode, Floor, Condition
)
VALUES (
    @Prop4,
    'Loja Comercial em Zona Premium de Braga',
    'Espaço comercial com 150m², duas frentes, casa de banho e arrumos. Localização excelente em zona de grande movimento pedonal.',
    'Commercial',
    'Rent',
    1800.00,
    150.0,
    0,
    1,
    0,
    'Braga',
    'Braga (São José de São Lázaro e São João do Souto)',
    2010,
    'B',
    'Available',
    0,
    (SELECT Id FROM [DreamLuso].[RealEstateAgents] WHERE LicenseNumber = 'AMI-12345'),
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Avenida da Liberdade, 89',
    '4710-251',
    0,
    'Used'
);

-- Propriedade 5: Apartamento T1 em Coimbra
INSERT INTO [DreamLuso].[Properties] (
    Id, Title, Description, Type, TransactionType, Price, Area, Bedrooms, Bathrooms, 
    HasGarage, Municipality, Parish, YearBuilt, EnergyRating, Status, IsFeatured,
    RealEstateAgentId, CreatedAt, UpdatedAt, IsDeleted, Street, PostalCode, Floor, Condition
)
VALUES (
    @Prop5,
    'T1 Moderno no Centro de Coimbra',
    'Apartamento T1 totalmente remodelado, com cozinha em open space e varanda. Ideal para estudantes ou jovem casal.',
    'Apartment',
    'Rent',
    550.00,
    55.0,
    1,
    1,
    0,
    'Coimbra',
    'Coimbra (Sé Nova, Santa Cruz, Almedina e São Bartolomeu)',
    2005,
    'C',
    'Available',
    1,
    (SELECT Id FROM [DreamLuso].[RealEstateAgents] WHERE LicenseNumber = 'AMI-23456'),
    GETUTCDATE(),
    GETUTCDATE(),
    0,
    'Rua da Sofia, 67',
    '3000-389',
    1,
    'Renovated'
);

PRINT '✅ 5 Propriedades criadas';
GO

-- =====================================================
-- 5️⃣ IMAGENS DE PROPRIEDADES
-- =====================================================
PRINT '📸 Criando imagens de propriedades...';

-- Imagens para Propriedade 1
INSERT INTO [DreamLuso].[PropertyImages] (Id, PropertyId, ImageUrl, IsMain, CreatedAt, UpdatedAt, IsDeleted)
SELECT NEWID(), Id, 'uploads/properties/prop1_main.jpg', 1, GETUTCDATE(), GETUTCDATE(), 0
FROM [DreamLuso].[Properties] WHERE Title LIKE '%Apartamento T3 Moderno%';

INSERT INTO [DreamLuso].[PropertyImages] (Id, PropertyId, ImageUrl, IsMain, CreatedAt, UpdatedAt, IsDeleted)
SELECT NEWID(), Id, 'uploads/properties/prop1_kitchen.jpg', 0, GETUTCDATE(), GETUTCDATE(), 0
FROM [DreamLuso].[Properties] WHERE Title LIKE '%Apartamento T3 Moderno%';

-- Imagens para Propriedade 2
INSERT INTO [DreamLuso].[PropertyImages] (Id, PropertyId, ImageUrl, IsMain, CreatedAt, UpdatedAt, IsDeleted)
SELECT NEWID(), Id, 'uploads/properties/prop2_main.jpg', 1, GETUTCDATE(), GETUTCDATE(), 0
FROM [DreamLuso].[Properties] WHERE Title LIKE '%Moradia V4 com Jardim%';

-- Imagens para Propriedade 3
INSERT INTO [DreamLuso].[PropertyImages] (Id, PropertyId, ImageUrl, IsMain, CreatedAt, UpdatedAt, IsDeleted)
SELECT NEWID(), Id, 'uploads/properties/prop3_main.jpg', 1, GETUTCDATE(), GETUTCDATE(), 0
FROM [DreamLuso].[Properties] WHERE Title LIKE '%T2 Junto à Praia%';

PRINT '✅ Imagens de propriedades criadas';
GO

-- =====================================================
-- 6️⃣ VISITAS
-- =====================================================
PRINT '📅 Criando visitas agendadas...';

INSERT INTO [DreamLuso].[PropertyVisits] (Id, PropertyId, ClientId, ScheduledDate, Status, Notes, CreatedAt, UpdatedAt, IsDeleted)
SELECT 
    NEWID(),
    p.Id,
    c.Id,
    DATEADD(DAY, 3, GETUTCDATE()),
    'Scheduled',
    'Cliente interessado em conhecer o imóvel',
    GETUTCDATE(),
    GETUTCDATE(),
    0
FROM [DreamLuso].[Properties] p
CROSS JOIN [DreamLuso].[Clients] c
WHERE p.Title LIKE '%Apartamento T3 Moderno%' AND c.UserId IN (SELECT Id FROM [DreamLuso].[Users] WHERE Email = 'ana.rodrigues@email.com');

INSERT INTO [DreamLuso].[PropertyVisits] (Id, PropertyId, ClientId, ScheduledDate, Status, Notes, CreatedAt, UpdatedAt, IsDeleted)
SELECT 
    NEWID(),
    p.Id,
    c.Id,
    DATEADD(DAY, -2, GETUTCDATE()),
    'Completed',
    'Visita realizada com sucesso. Cliente muito interessado.',
    GETUTCDATE(),
    GETUTCDATE(),
    0
FROM [DreamLuso].[Properties] p
CROSS JOIN [DreamLuso].[Clients] c
WHERE p.Title LIKE '%Moradia V4%' AND c.UserId IN (SELECT Id FROM [DreamLuso].[Users] WHERE Email = 'carlos.ferreira@email.com');

PRINT '✅ Visitas criadas';
GO

-- =====================================================
-- 7️⃣ COMENTÁRIOS/AVALIAÇÕES
-- =====================================================
PRINT '💬 Criando comentários...';

INSERT INTO [DreamLuso].[Comments] (Id, PropertyId, UserId, Message, Rating, HelpfulCount, IsFlagged, ParentCommentId, CreatedAt, UpdatedAt, IsDeleted)
SELECT 
    NEWID(),
    p.Id,
    u.Id,
    'Excelente localização! Apartamento muito bem conservado e com ótima luminosidade.',
    5,
    3,
    0,
    NULL,
    GETUTCDATE(),
    GETUTCDATE(),
    0
FROM [DreamLuso].[Properties] p
CROSS JOIN [DreamLuso].[Users] u
WHERE p.Title LIKE '%Apartamento T3 Moderno%' AND u.Email = 'ana.rodrigues@email.com';

INSERT INTO [DreamLuso].[Comments] (Id, PropertyId, UserId, Message, Rating, HelpfulCount, IsFlagged, ParentCommentId, CreatedAt, UpdatedAt, IsDeleted)
SELECT 
    NEWID(),
    p.Id,
    u.Id,
    'Moradia fantástica! A piscina e o jardim são incríveis. Recomendo!',
    5,
    5,
    0,
    NULL,
    GETUTCDATE(),
    GETUTCDATE(),
    0
FROM [DreamLuso].[Properties] p
CROSS JOIN [DreamLuso].[Users] u
WHERE p.Title LIKE '%Moradia V4%' AND u.Email = 'sofia.almeida@email.com';

PRINT '✅ Comentários criados';
GO

-- =====================================================
-- 8️⃣ PROPOSTAS
-- =====================================================
PRINT '📝 Criando propostas...';

DECLARE @Proposal1 UNIQUEIDENTIFIER = NEWID();

INSERT INTO [DreamLuso].[PropertyProposals] (
    Id, ProposalNumber, PropertyId, ClientId, ProposedValue, Type, Status, 
    PaymentMethod, IntendedMoveDate, AdditionalNotes, CreatedAt, UpdatedAt, IsDeleted
)
SELECT 
    @Proposal1,
    'PROP-2025-' + CAST(CAST(RAND() * 900 + 100 AS INT) AS VARCHAR),
    p.Id,
    c.Id,
    370000.00,
    'Purchase',
    'InNegotiation',
    'Financiamento Bancário',
    DATEADD(MONTH, 2, GETUTCDATE()),
    'Interessado em fechar negócio rapidamente. Tenho pré-aprovação bancária.',
    GETUTCDATE(),
    GETUTCDATE(),
    0
FROM [DreamLuso].[Properties] p
CROSS JOIN [DreamLuso].[Clients] c
WHERE p.Title LIKE '%Apartamento T3 Moderno%' AND c.UserId IN (SELECT Id FROM [DreamLuso].[Users] WHERE Email = 'carlos.ferreira@email.com');

PRINT '✅ Propostas criadas';
GO

-- =====================================================
-- 9️⃣ NOTIFICAÇÕES
-- =====================================================
PRINT '🔔 Criando notificações...';

INSERT INTO [DreamLuso].[Notifications] (
    Id, SenderId, RecipientId, Message, Status, Type, Priority, 
    IsTransient, ExpirationDate, CreatedAt, UpdatedAt, IsDeleted
)
SELECT 
    NEWID(),
    u_sender.Id,
    u_recipient.Id,
    'Nova visita agendada para ' + CONVERT(VARCHAR, DATEADD(DAY, 3, GETUTCDATE()), 103),
    'Unread',
    'Visit',
    'High',
    0,
    DATEADD(DAY, 7, GETUTCDATE()),
    GETUTCDATE(),
    GETUTCDATE(),
    0
FROM [DreamLuso].[Users] u_sender
CROSS JOIN [DreamLuso].[Users] u_recipient
WHERE u_sender.Email = 'ana.rodrigues@email.com' 
  AND u_recipient.Email = 'joao.silva@dreamluso.pt';

INSERT INTO [DreamLuso].[Notifications] (
    Id, SenderId, RecipientId, Message, Status, Type, Priority, 
    IsTransient, ExpirationDate, CreatedAt, UpdatedAt, IsDeleted
)
SELECT 
    NEWID(),
    u_sender.Id,
    u_recipient.Id,
    'Você recebeu uma nova proposta de €370.000',
    'Unread',
    'Proposal',
    'High',
    0,
    DATEADD(DAY, 14, GETUTCDATE()),
    GETUTCDATE(),
    GETUTCDATE(),
    0
FROM [DreamLuso].[Users] u_sender
CROSS JOIN [DreamLuso].[Users] u_recipient
WHERE u_sender.Email = 'carlos.ferreira@email.com' 
  AND u_recipient.Email = 'joao.silva@dreamluso.pt';

PRINT '✅ Notificações criadas';
GO

-- =====================================================
-- 🎯 RESUMO FINAL
-- =====================================================
PRINT '';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT '✅ SEED COMPLETO - DADOS INSERIDOS COM SUCESSO!';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT '';
PRINT '📊 ESTATÍSTICAS:';
PRINT '   ├─ Usuários: ' + CAST((SELECT COUNT(*) FROM [DreamLuso].[Users]) AS VARCHAR) + ' (1 Admin, 3 Agentes, 5 Clientes)';
PRINT '   ├─ Propriedades: ' + CAST((SELECT COUNT(*) FROM [DreamLuso].[Properties]) AS VARCHAR);
PRINT '   ├─ Imagens: ' + CAST((SELECT COUNT(*) FROM [DreamLuso].[PropertyImages]) AS VARCHAR);
PRINT '   ├─ Visitas: ' + CAST((SELECT COUNT(*) FROM [DreamLuso].[PropertyVisits]) AS VARCHAR);
PRINT '   ├─ Comentários: ' + CAST((SELECT COUNT(*) FROM [DreamLuso].[Comments]) AS VARCHAR);
PRINT '   ├─ Propostas: ' + CAST((SELECT COUNT(*) FROM [DreamLuso].[PropertyProposals]) AS VARCHAR);
PRINT '   └─ Notificações: ' + CAST((SELECT COUNT(*) FROM [DreamLuso].[Notifications]) AS VARCHAR);
PRINT '';
PRINT '🔑 CREDENCIAIS DE ACESSO:';
PRINT '';
PRINT '   👑 ADMIN:';
PRINT '      Email: admin@gmail.com';
PRINT '      Senha: Admin123!';
PRINT '';
PRINT '   👔 AGENTES:';
PRINT '      Email: joao.silva@dreamluso.pt | Senha: Agent123!';
PRINT '      Email: maria.santos@dreamluso.pt | Senha: Agent123!';
PRINT '      Email: pedro.costa@dreamluso.pt | Senha: Agent123!';
PRINT '';
PRINT '   👤 CLIENTES:';
PRINT '      Email: ana.rodrigues@email.com | Senha: Client123!';
PRINT '      Email: carlos.ferreira@email.com | Senha: Client123!';
PRINT '      Email: sofia.almeida@email.com | Senha: Client123!';
PRINT '';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
GO
