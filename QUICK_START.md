# 🚀 Quick Start - Backend Local + Frontend Vercel

## ⚡ Setup Rápido (Uma vez)

### 1. Instalar e configurar ngrok

```bash
# Instalar
sudo snap install ngrok

# Configurar (precisa criar conta em https://ngrok.com)
ngrok config add-authtoken SEU_TOKEN_AQUI
```

## 🎯 Uso Diário (Super Simples!)

### Para INICIAR tudo:

```bash
./start-backend-with-ngrok.sh
```

O script faz **automaticamente**:
1. ✅ Inicia SQL Server (se não estiver rodando)
2. ✅ Inicia Backend local (se não estiver rodando)
3. ✅ Inicia ngrok e obtém URL pública
4. ✅ Atualiza `environment.prod.ts` com URL do ngrok
5. ✅ Pergunta se quer fazer commit e push automático

**Resultado:** Frontend no Vercel consegue acessar seu backend local! 🎉

### Para PARAR tudo:

```bash
./stop-backend-ngrok.sh
```

Ou simplesmente pressione `Ctrl+C` no terminal onde o script está rodando.

## 📋 O que acontece:

```
Seu PC:
  Backend → localhost:5149
  ngrok   → https://abc123.ngrok-free.app → localhost:5149

Vercel (Frontend):
  https://dream-luso.vercel.app
  ↓
  Chama: https://abc123.ngrok-free.app/api
  ↓
  ngrok redireciona → localhost:5149 ✅
```

## ⚠️ Importante

- **Mantenha o terminal aberto** enquanto trabalha
- A URL do ngrok muda a cada reinício (plano gratuito)
- O script atualiza automaticamente o `environment.prod.ts`
- Se escolher commit automático, o Vercel faz redeploy sozinho

## 🔍 Ver logs

- **Backend:** `tail -f backend.log`
- **ngrok:** `tail -f ngrok.log`  
- **ngrok UI:** Abra http://localhost:4040 no browser

## 🎉 Pronto!

Agora é só rodar `./start-backend-with-ngrok.sh` e tudo funciona automaticamente!

