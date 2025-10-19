#!/usr/bin/env python3
"""
Script para aplicar migrations diretamente no PostgreSQL do Render
"""

import psycopg2
import sys

# Connection string do Render
CONNECTION_STRING = "postgresql://dreamluso_db_user:AkjQS3UI1PN9eJbb5bcxgmhTIlGzMZ5R@dpg-d3p1laili9vc73cof6cg-a/dreamluso_db"

def create_tables():
    """Cria as tabelas básicas do DreamLuso"""
    try:
        # Conectar ao banco
        conn = psycopg2.connect(CONNECTION_STRING)
        cursor = conn.cursor()
        
        print("✅ Conectado ao PostgreSQL do Render")
        
        # Criar tabela de migrations
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                "MigrationId" varchar(150) NOT NULL,
                "ProductVersion" varchar(32) NOT NULL,
                CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
            );
        """)
        
        # Criar tabela User
        cursor.execute("""
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
        """)
        
        # Criar índice único para email
        cursor.execute("""
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_User_Email" ON "User" ("Email");
        """)
        
        # Inserir migration record
        cursor.execute("""
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES ('20251019120000_ManualCreateTables', '8.0.0')
            ON CONFLICT ("MigrationId") DO NOTHING;
        """)
        
        # Commit das alterações
        conn.commit()
        
        print("✅ Tabelas criadas com sucesso!")
        print("✅ Tabela User criada")
        print("✅ Índice de email criado")
        print("✅ Migration history atualizada")
        
        cursor.close()
        conn.close()
        
        return True
        
    except Exception as e:
        print(f"❌ Erro ao criar tabelas: {e}")
        return False

def main():
    print("🚀 Aplicando migrations diretamente no PostgreSQL do Render")
    print("=" * 60)
    
    if create_tables():
        print("\n✅ Migrations aplicadas com sucesso!")
        print("🎉 Agora você pode testar a API!")
    else:
        print("\n❌ Falha ao aplicar migrations")
        sys.exit(1)

if __name__ == "__main__":
    main()
