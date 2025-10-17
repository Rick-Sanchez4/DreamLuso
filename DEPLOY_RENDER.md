# 🚀 Deploy DreamLuso no Render (GRATUITO)

Este guia explica como fazer deploy completo do **DreamLuso** (Backend .NET 8 + PostgreSQL) no **Render** de forma **100% gratuita**.

---

## 📋 PRÉ-REQUISITOS

✅ Conta no [Render](https://render.com) (gratuita)  
✅ Repositório no GitHub  
✅ Código do DreamLuso atualizado

---

## 🎯 ARQUITETURA DO DEPLOY

```
┌─────────────────────────────────────┐
│         RENDER (Gratuito)           │
├─────────────────────────────────────┤
│                                     │
│  📦 PostgreSQL Database             │
│     ├─ 1 GB Storage                 │
│     ├─ Frankfurt Region             │
│     └─ Free Tier                    │
│                                     │
│  🌐 Backend API (.NET 8)            │
│     ├─ Docker Container             │
│     ├─ Auto Deploy (GitHub)         │
│     ├─ Health Check: /health        │
│     ├─ Sleep após 15min inatividade │
│     └─ Free Tier (750h/mês)         │
│                                     │
└─────────────────────────────────────┘
```

---

## 🔧 PASSO 1: PREPARAR O PROJETO

### ✅ Alterações Já Aplicadas

O projeto já foi configurado com:

1. **Package PostgreSQL adicionado** (`Npgsql.EntityFrameworkCore.PostgreSQL`)
2. **DependencyInjection.cs atualizado** - Detecta automaticamente o provider
3. **ApplicationDbContext.cs ajustado** - Schema condicional (SQL Server vs PostgreSQL)
4. **appsettings.Production.json criado** - Configuração para produção
5. **Dockerfile otimizado** - Porta 8080, health checks
6. **render.yaml criado** - Deploy automático
7. **.dockerignore criado** - Build mais rápido

---

## 🚀 PASSO 2: FAZER COMMIT E PUSH

```bash
# Adicionar todas as alterações
git add -A

# Commit
git commit -m "feat: configurar deploy para Render com PostgreSQL"

# Push para GitHub
git push origin main
```

---

## 🎯 PASSO 3: CRIAR CONTA NO RENDER

1. Ir para: https://render.com
2. Clicar em **"Get Started for Free"**
3. Conectar com GitHub
4. Autorizar acesso ao repositório **DreamLuso**

---

## 📦 PASSO 4: CRIAR POSTGRESQL DATABASE

### Via Dashboard (Recomendado):

1. No Render Dashboard, clicar em **"New +"**
2. Selecionar **"PostgreSQL"**
3. Configurar:
   ```
   Name: dreamluso-db
   Database: dreamluso_db
   User: dreamluso_user
   Region: Frankfurt (Europa)
   Plan: Free
   ```
4. Clicar em **"Create Database"**
5. **Aguardar 2-3 minutos** até ficar "Available"
6. **Copiar a "Internal Connection String"** (começar com `postgresql://...`)

---

## 🌐 PASSO 5: CRIAR WEB SERVICE (BACKEND)

### Via Dashboard:

1. No Render Dashboard, clicar em **"New +"**
2. Selecionar **"Web Service"**
3. Conectar ao repositório GitHub: **DreamLuso**
4. Configurar:

```
Name: dreamluso-api
Region: Frankfurt
Branch: main
Runtime: Docker
Docker Command: (deixar vazio, usa o Dockerfile)
Plan: Free
```

### Variáveis de Ambiente:

Adicionar as seguintes Environment Variables:

```bash
# Obrigatórias
ASPNETCORE_ENVIRONMENT = Production
ASPNETCORE_URLS = http://+:8080
DatabaseProvider = PostgreSQL

# Connection String (colar a Internal Connection String da base de dados)
ConnectionStrings__DreamLusoDB = postgresql://dreamluso_user:senha@dpg-xxxxx-a.frankfurt-postgres.render.com/dreamluso_db

# JWT Settings (gerar uma chave secreta longa)
JwtSettings__Key = SUA_CHAVE_SECRETA_MINIMO_32_CARACTERES_AQUI_PARA_HS512_SECURITY
JwtSettings__Issuer = DreamLusoAPI
JwtSettings__Audience = DreamLusoClient

# Base URL (será atualizada após deploy)
AppSettings__BaseUrl = https://dreamluso-api.onrender.com
```

5. **Health Check Path:** `/health`
6. Clicar em **"Create Web Service"**

---

## ⏳ PASSO 6: AGUARDAR O DEPLOY

1. Render vai:
   - ✅ Clonar o repositório
   - ✅ Build do Docker image
   - ✅ Deploy do container
   - ✅ Executar health check

2. **Tempo estimado:** 5-10 minutos

3. Quando ficar **verde "Live"**, o backend está online! 🎉

---

## 🗄️ PASSO 7: APLICAR MIGRATIONS

### Opção A: Via Terminal Local (Recomendado)

```bash
# Exportar a connection string do Render
export ConnectionStrings__DreamLusoDB="postgresql://dreamluso_user:senha@dpg-xxxxx-a.frankfurt-postgres.render.com/dreamluso_db"

# Aplicar migrations
cd DreamLuso.WebAPI
dotnet ef database update --project ../DreamLuso.Data

# Executar seed (opcional)
dotnet run --seed
```

### Opção B: Via Render Shell

1. No Render Dashboard → dreamluso-api
2. Ir em **"Shell"**
3. Executar:
```bash
dotnet ef database update --project DreamLuso.Data
```

---

## ✅ PASSO 8: TESTAR O BACKEND

### 1. Testar Health Check:
```bash
curl https://dreamluso-api.onrender.com/health
# Deve retornar: Healthy
```

### 2. Testar Swagger:
```
https://dreamluso-api.onrender.com/swagger
```

### 3. Testar Registro de Usuário:
```bash
curl -X POST https://dreamluso-api.onrender.com/api/accounts/register \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Teste",
    "lastName": "User",
    "email": "teste@example.com",
    "password": "Test123!",
    "role": 1,
    "phone": "912345678"
  }'
```

---

## 🔄 DEPLOY AUTOMÁTICO

Agora, toda vez que você fizer **push para o GitHub**, o Render vai:
1. Detectar as mudanças
2. Fazer build automático
3. Deploy da nova versão
4. **Zero downtime!** 🚀

---

## ⚠️ LIMITAÇÕES DO PLANO GRATUITO

| Recurso | Limite |
|---------|--------|
| **PostgreSQL Storage** | 1 GB |
| **Backend Uptime** | 750 horas/mês |
| **Sleep após inatividade** | 15 minutos |
| **Wake-up time** | ~30 segundos |
| **Bandwidth** | 100 GB/mês |
| **Build time** | 500 min/mês |

### 💡 Dica: Evitar Sleep

Se quiser manter o backend ativo 24/7:
- Usar serviços como [UptimeRobot](https://uptimerobot.com) (gratuito)
- Fazer ping no `/health` a cada 10 minutos

---

## 🛠️ TROUBLESHOOTING

### ❌ Erro: "Cannot open server"

**Causa:** Connection string incorreta

**Solução:**
1. Copiar a **Internal Connection String** do Render
2. Atualizar variável `ConnectionStrings__DreamLusoDB`

### ❌ Erro: "No database provider configured"

**Causa:** Variável `DatabaseProvider` não configurada

**Solução:**
- Adicionar: `DatabaseProvider = PostgreSQL`

### ❌ Build falha

**Causa:** Problema no Dockerfile ou dependências

**Solução:**
```bash
# Testar build local
docker build -t dreamluso-test .
docker run -p 8080:8080 dreamluso-test
```

### ❌ Health check falha

**Causa:** Aplicação não inicia corretamente

**Solução:**
1. Ver logs no Render Dashboard
2. Verificar variáveis de ambiente
3. Verificar connection string

---

## 📊 MONITORAMENTO

### Via Render Dashboard:
- **Logs:** Real-time logs
- **Metrics:** CPU, Memory, Requests
- **Events:** Deploy history

### Logs em tempo real:
```bash
# Via Render CLI (opcional)
render logs -s dreamluso-api --tail
```

---

## 🌐 ATUALIZAR FRONTEND

No seu frontend Angular, atualizar a API URL:

```typescript
// src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://dreamluso-api.onrender.com/api'
};
```

---

## 🎉 PRONTO!

Seu backend está rodando **100% GRATUITO** no Render! 🚀

**URLs:**
- 🌐 **API:** https://dreamluso-api.onrender.com
- 📚 **Swagger:** https://dreamluso-api.onrender.com/swagger
- ❤️ **Health:** https://dreamluso-api.onrender.com/health
- 🗄️ **Database:** Gerenciado pelo Render

---

## 🆘 SUPORTE

- [Documentação Render](https://render.com/docs)
- [Render Community](https://community.render.com)
- [PostgreSQL no Render](https://render.com/docs/databases)

---

**Última atualização:** 17/10/2025  
**Versão:** 1.0.0  
**Status:** ✅ Configurado e testado

