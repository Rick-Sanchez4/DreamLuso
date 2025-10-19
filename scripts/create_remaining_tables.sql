-- Script para criar as tabelas restantes do DreamLuso no PostgreSQL

-- Criar tabela Client
CREATE TABLE IF NOT EXISTS "Client" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "MaxBudget" decimal(18,2),
    "MinBudget" decimal(18,2),
    "PreferredLocation" varchar(255),
    "PropertyType" varchar(50),
    "Bedrooms" integer,
    "Bathrooms" integer,
    "HasParking" boolean,
    "HasGarden" boolean,
    "HasPool" boolean,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "CreatedBy" varchar(100),
    "UpdatedBy" varchar(100),
    CONSTRAINT "PK_Client" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Client_User_UserId" FOREIGN KEY ("UserId") REFERENCES "User" ("Id") ON DELETE CASCADE
);

-- Criar tabela RealEstateAgent
CREATE TABLE IF NOT EXISTS "RealEstateAgent" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "LicenseNumber" varchar(100),
    "AgencyName" varchar(255),
    "Experience" integer,
    "Specializations" text,
    "CommissionRate" decimal(5,2),
    "TotalRevenue" decimal(18,2),
    "IsApproved" boolean NOT NULL,
    "ApprovalDate" timestamp,
    "ApprovalStatus" integer,
    "RejectionReason" text,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "CreatedBy" varchar(100),
    "UpdatedBy" varchar(100),
    CONSTRAINT "PK_RealEstateAgent" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_RealEstateAgent_User_UserId" FOREIGN KEY ("UserId") REFERENCES "User" ("Id") ON DELETE CASCADE
);

-- Criar tabela Property
CREATE TABLE IF NOT EXISTS "Property" (
    "Id" uuid NOT NULL,
    "Title" varchar(255) NOT NULL,
    "Description" text,
    "Price" decimal(18,2) NOT NULL,
    "PropertyType" varchar(50) NOT NULL,
    "Bedrooms" integer,
    "Bathrooms" integer,
    "Area" decimal(10,2),
    "YearBuilt" integer,
    "Location" varchar(255),
    "Address" text,
    "Features" text,
    "Status" varchar(50) NOT NULL,
    "AgentId" uuid,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "CreatedBy" varchar(100),
    "UpdatedBy" varchar(100),
    CONSTRAINT "PK_Property" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Property_RealEstateAgent_AgentId" FOREIGN KEY ("AgentId") REFERENCES "RealEstateAgent" ("Id") ON DELETE SET NULL
);

-- Criar tabela PropertyImage
CREATE TABLE IF NOT EXISTS "PropertyImage" (
    "Id" uuid NOT NULL,
    "PropertyId" uuid NOT NULL,
    "ImageUrl" varchar(500) NOT NULL,
    "IsMain" boolean NOT NULL,
    "Order" integer,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "CreatedBy" varchar(100),
    "UpdatedBy" varchar(100),
    CONSTRAINT "PK_PropertyImage" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PropertyImage_Property_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Property" ("Id") ON DELETE CASCADE
);

-- Criar tabela PropertyVisit
CREATE TABLE IF NOT EXISTS "PropertyVisit" (
    "Id" uuid NOT NULL,
    "PropertyId" uuid NOT NULL,
    "ClientId" uuid NOT NULL,
    "AgentId" uuid,
    "VisitDate" timestamp NOT NULL,
    "Status" varchar(50) NOT NULL,
    "Notes" text,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "CreatedBy" varchar(100),
    "UpdatedBy" varchar(100),
    CONSTRAINT "PK_PropertyVisit" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PropertyVisit_Client_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Client" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PropertyVisit_Property_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Property" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PropertyVisit_RealEstateAgent_AgentId" FOREIGN KEY ("AgentId") REFERENCES "RealEstateAgent" ("Id") ON DELETE SET NULL
);

-- Criar tabela Contract
CREATE TABLE IF NOT EXISTS "Contract" (
    "Id" uuid NOT NULL,
    "PropertyId" uuid NOT NULL,
    "ClientId" uuid NOT NULL,
    "AgentId" uuid,
    "ContractType" varchar(50) NOT NULL,
    "Price" decimal(18,2) NOT NULL,
    "StartDate" timestamp NOT NULL,
    "EndDate" timestamp,
    "Status" varchar(50) NOT NULL,
    "Terms" text,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "CreatedBy" varchar(100),
    "UpdatedBy" varchar(100),
    CONSTRAINT "PK_Contract" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Contract_Client_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Client" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Contract_Property_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Property" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Contract_RealEstateAgent_AgentId" FOREIGN KEY ("AgentId") REFERENCES "RealEstateAgent" ("Id") ON DELETE SET NULL
);

-- Criar tabela Notification
CREATE TABLE IF NOT EXISTS "Notification" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Title" varchar(255) NOT NULL,
    "Message" text NOT NULL,
    "Type" varchar(50) NOT NULL,
    "IsRead" boolean NOT NULL,
    "SenderId" uuid,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "CreatedBy" varchar(100),
    "UpdatedBy" varchar(100),
    CONSTRAINT "PK_Notification" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Notification_User_UserId" FOREIGN KEY ("UserId") REFERENCES "User" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Notification_User_SenderId" FOREIGN KEY ("SenderId") REFERENCES "User" ("Id") ON DELETE SET NULL
);

-- Criar tabela PropertyProposal
CREATE TABLE IF NOT EXISTS "PropertyProposal" (
    "Id" uuid NOT NULL,
    "PropertyId" uuid NOT NULL,
    "ClientId" uuid NOT NULL,
    "ProposedPrice" decimal(18,2) NOT NULL,
    "Message" text,
    "Status" varchar(50) NOT NULL,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "CreatedBy" varchar(100),
    "UpdatedBy" varchar(100),
    CONSTRAINT "PK_PropertyProposal" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PropertyProposal_Client_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Client" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PropertyProposal_Property_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Property" ("Id") ON DELETE CASCADE
);

-- Criar tabela ProposalNegotiation
CREATE TABLE IF NOT EXISTS "ProposalNegotiation" (
    "Id" uuid NOT NULL,
    "ProposalId" uuid NOT NULL,
    "Message" text NOT NULL,
    "Price" decimal(18,2),
    "SenderType" varchar(50) NOT NULL,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "CreatedBy" varchar(100),
    "UpdatedBy" varchar(100),
    CONSTRAINT "PK_ProposalNegotiation" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProposalNegotiation_PropertyProposal_ProposalId" FOREIGN KEY ("ProposalId") REFERENCES "PropertyProposal" ("Id") ON DELETE CASCADE
);

-- Criar tabela Comment
CREATE TABLE IF NOT EXISTS "Comment" (
    "Id" uuid NOT NULL,
    "PropertyId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Content" text NOT NULL,
    "Rating" integer,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "CreatedBy" varchar(100),
    "UpdatedBy" varchar(100),
    CONSTRAINT "PK_Comment" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Comment_Property_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Property" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Comment_User_UserId" FOREIGN KEY ("UserId") REFERENCES "User" ("Id") ON DELETE CASCADE
);

-- Verificar se as tabelas foram criadas
SELECT 'All tables created successfully' as status;
