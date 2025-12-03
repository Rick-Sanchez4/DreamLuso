# 🏠 DreamLuso - Sistema Imobiliário

Sistema completo de gestão imobiliária com backend .NET e frontend Angular.

## 🚀 Início Rápido

### 1. Iniciar Backend (API + Banco SQL Server)
```bash
./start-backend.sh
```
> **Nota:** Na primeira execução, o script automaticamente:
> - Cria o banco de dados
> - Aplica as migrações
> - Popula com dados de teste (usuários, etc.)

### 2. Iniciar Frontend (Angular)
```bash
./start-frontend.sh
```

## 🔑 Credenciais de Teste

### 👑 Administrador
- **Email:** `admin@gmail.com`
- **Senha:** `Admin123!`

### 👔 Agente Imobiliário
- **Email:** `joao.silva@dreamluso.pt`
- **Senha:** `Agent123!`

### 👥 Cliente
- **Email:** `ana.rodrigues@email.com`
- **Senha:** `Client123!`

## 🌐 URLs

- **Frontend:** http://localhost:4200
- **API:** http://localhost:5149
- **Swagger:** http://localhost:5149/swagger
- **Health Check:** http://localhost:5149/health

## 📊 Banco de Dados

- **Tipo:** SQL Server 2022
- **Host:** localhost:1433
- **Database:** DreamLusoDB
- **Usuário:** sa
- **Senha:** DreamLuso2025!

### 📋 Dados Iniciais

Na **primeira execução**, o sistema automaticamente cria:

- ✅ **1 Administrador** (admin@gmail.com)
- ✅ **3 Agentes Imobiliários** (joao.silva, maria.santos, pedro.costa)
- ✅ **5 Clientes** (ana.rodrigues, carlos.ferreira, sofia.almeida, miguel.oliveira, beatriz.lopes)

> **💡 Dica:** Se quiser resetar os dados, execute:
> ```bash
> docker compose -f docker-compose.sqlserver.yml down -v
> ./start-backend.sh
> ```

## 🛠️ Comandos Úteis

### Gerenciar Containers
```bash
# Ver status
docker compose -f docker-compose.sqlserver.yml ps

# Parar tudo
docker compose -f docker-compose.sqlserver.yml down

# Ver logs
docker compose -f docker-compose.sqlserver.yml logs -f
```

### Conectar ao Banco
```bash
docker exec -it dreamluso-sqlserver-local /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'DreamLuso2025!' -C
```

### Resetar Sistema
```bash
# Parar tudo
docker compose -f docker-compose.sqlserver.yml down

# Remover volumes (CUIDADO: apaga todos os dados!)
docker compose -f docker-compose.sqlserver.yml down -v

# Iniciar novamente
./start-backend.sh
```

## 📝 Funcionalidades

- ✅ Gestão de usuários (Admin, Agentes, Clientes)
- ✅ Gestão de propriedades
- ✅ Agendamento de visitas
- ✅ Sistema de propostas
- ✅ Contratos
- ✅ Notificações
- ✅ Upload de imagens
- ✅ Dashboard administrativo
- ✅ **Sistema de Favoritos** (NOVO)

## 🔧 Requisitos

- Docker e Docker Compose
- .NET 8.0 SDK
- Node.js 18+ e npm
- Angular CLI

## 📞 Suporte

Em caso de problemas:
1. Verificar se Docker está rodando
2. Verificar logs: `docker compose -f docker-compose.sqlserver.yml logs`
3. Reiniciar containers: `docker compose -f docker-compose.sqlserver.yml restart`

---

**Última atualização:** 2025-01-27  
**Versão:** 1.0.0  
**Status:** ✅ Pronto para uso
