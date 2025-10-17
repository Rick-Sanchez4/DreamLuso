# 🔧 CONFIGURAÇÃO MANUAL DO RENDER - URGENTE

## ⚠️ PROBLEMA
A connection string do PostgreSQL não está a ser passada corretamente via `render.yaml`.  
**SOLUÇÃO: Configurar manualmente no dashboard.**

---

## 📋 PASSOS OBRIGATÓRIOS

### 1️⃣ Aceder ao Dashboard do Render
🔗 https://dashboard.render.com/web/srv-d3p1f4ili9vc73cof5v0

### 2️⃣ Configurar Variáveis de Ambiente

Clique em **Environment** no menu lateral.

#### ✅ ADICIONAR/ATUALIZAR ESTAS VARIÁVEIS:

| Variável | Valor |
|----------|-------|
| `ConnectionStrings__DreamLusoDB` | `postgresql://dreamluso_db_user:AkjQS3UI1PN9eJbb5bcxgmhTIlGzMZ5R@dpg-d3p1laili9vc73cof6cg-a/dreamluso_db` |
| `DatabaseProvider` | `PostgreSQL` |
| `JwtSettings__Key` | `YHJpcvOI6k/SzVLo4bYfIhG1mlvDLcM8voe4bkVrrks=` |
| `JwtSettings__Issuer` | `DreamLusoAPI` |
| `JwtSettings__Audience` | `DreamLusoClient` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ASPNETCORE_URLS` | `http://+:8080` |

### 3️⃣ Salvar e Aguardar Deploy

1. Clique em **"Save Changes"** no topo da página
2. O Render vai automaticamente fazer redeploy
3. Aguarde 5-10 minutos

---

## 🧪 TESTAR APÓS CONFIGURAÇÃO

Quando o deploy completar, execute este comando:

```bash
curl -X POST "https://dreamluso-api.onrender.com/api/accounts/register" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName":"Admin",
    "lastName":"DreamLuso",
    "email":"admin@dreamluso.com",
    "password":"Admin123!",
    "phone":"912345678",
    "role":3
  }' | python3 -m json.tool
```

**Se funcionar, você verá:**
```json
{
    "userId": "xxx-xxx-xxx",
    "email": "admin@dreamluso.com",
    "firstName": "Admin",
    "lastName": "DreamLuso"
}
```

---

## 🚨 IMPORTANTE

**POR QUE MANUAL?**
- O Render Blueprint (`render.yaml`) nem sempre aplica as variáveis de ambiente corretamente
- A configuração manual garante que os valores sejam salvos

**DEPOIS QUE FUNCIONAR:**
Execute o script para popular o sistema completo:
```bash
cd /home/rick-sanchez/DreamLuso/scripts
python3 seed_render_api.py
```

---

## 📸 ONDE CLICAR NO DASHBOARD

1. Dashboard → **Web Services** → `dreamluso-api`
2. Menu lateral → **Environment**
3. Botão **"Add Environment Variable"** para cada variável
4. Preencher **Key** e **Value**
5. Clicar em **"Save Changes"**

---

✅ **APÓS CONFIGURAR, ME AVISE QUE VOU TESTAR!**

