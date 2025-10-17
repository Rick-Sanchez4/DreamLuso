# 🔧 Como Corrigir a Conexão do PostgreSQL no Render

## ❌ Problema
A API está retornando erro 500 porque a string de conexão do PostgreSQL não está sendo passada corretamente.

```
"Format of the initialization string does not conform to specification starting at index 0."
```

## ✅ Solução

### Passo 1: Obter a Connection String do PostgreSQL

1. Acesse: https://dashboard.render.com
2. Vá em **Databases** → `dreamluso-db`
3. Copie a **Internal Connection String**:
   ```
   postgresql://dreamluso_db_user:AkjQS3UI1PN9eJbb5bcxgmhTIlGzMZ5R@dpg-d3p1laili9vc73cof6cg-a/dreamluso_db
   ```

### Passo 2: Configurar Variável de Ambiente na API

1. Vá em **Services** → `dreamluso-api`
2. Clique em **Environment**
3. Encontre ou adicione a variável: `ConnectionStrings__DreamLusoDB`
4. Cole a connection string do Passo 1
5. Clique em **Save Changes**

### Passo 3: Adicionar a Chave JWT

1. No mesmo menu **Environment**
2. Verifique se `JwtSettings__Key` foi gerado automaticamente
3. Se não existir, adicione manualmente:
   ```
   Chave gerada: gere uma chave de 32+ caracteres aleatórios
   ```

### Passo 4: Verificar Outras Variáveis

Certifique-se que estas variáveis existem:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
DatabaseProvider=PostgreSQL
JwtSettings__Issuer=DreamLusoAPI
JwtSettings__Audience=DreamLusoClient
```

### Passo 5: Manual Deploy

1. Vá para a aba **Manual Deploy**
2. Clique em **Deploy latest commit**
3. Aguarde o deploy completar (5-10 minutos)

---

## 🧪 Testar a API

Após o deploy, teste:

```bash
# Health Check
curl https://dreamluso-api.onrender.com/health

# Registrar Admin
curl -X POST "https://dreamluso-api.onrender.com/api/accounts/register" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName":"Admin",
    "lastName":"DreamLuso",
    "email":"admin@dreamluso.com",
    "password":"Admin123!",
    "phone":"912345678",
    "role":3
  }'
```

---

## 📝 Popular o Sistema

Depois que o registro funcionar, execute:

```bash
cd /home/rick-sanchez/DreamLuso/scripts
python3 seed_render_api.py
```

Isso criará:
- ✅ Admin (admin@dreamluso.com / Admin123!)
- ✅ Agentes Imobiliários
- ✅ Clientes

---

## 🔍 Ver Logs do Render

Para ver o que está acontecendo:

1. Dashboard do Render → `dreamluso-api`
2. Clique em **Logs**
3. Veja os erros em tempo real

