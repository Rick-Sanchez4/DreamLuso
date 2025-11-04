# 🌐 Configuração do Backend Local com ngrok

Este guia explica como expor seu backend local para que o frontend no Vercel possa acessá-lo.

## 📋 Pré-requisitos

1. Backend rodando localmente em `http://localhost:5149`
2. ngrok instalado

## 🚀 Passo a Passo

### 1. Instalar ngrok

**Opção A - Via Snap (recomendado):**
```bash
sudo snap install ngrok
```

**Opção B - Download direto:**
```bash
# Baixe de: https://ngrok.com/download
# Ou via wget:
wget https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-linux-amd64.tgz
tar -xzf ngrok-v3-stable-linux-amd64.tgz
sudo mv ngrok /usr/local/bin/
```

### 2. Configurar ngrok (primeira vez)

1. Crie uma conta gratuita em: https://dashboard.ngrok.com/signup
2. Copie seu authtoken do dashboard
3. Configure:
```bash
ngrok config add-authtoken SEU_TOKEN_AQUI
```

### 3. Iniciar o Backend Local

```bash
cd /home/rick-sanchez/DreamLuso
./start-backend.sh
```

Aguarde o backend iniciar completamente (verificar em `http://localhost:5149/health`)

### 4. Expor com ngrok

**Opção A - Usar o script:**
```bash
./start-ngrok.sh
```

**Opção B - Manual:**
```bash
ngrok http 5149
```

Você verá algo como:
```
Forwarding  https://abc123.ngrok-free.app -> http://localhost:5149
```

### 5. Configurar no Vercel

1. Vá para o **Vercel Dashboard** → Seu projeto → **Settings** → **Environment Variables**
2. Adicione a variável:
   - **Key**: `NG_APP_API_URL`
   - **Value**: `https://abc123.ngrok-free.app` (a URL que o ngrok forneceu)
   - **Environment**: Production (e Preview se quiser)
3. Faça um **redeploy** do projeto no Vercel

### 6. Atualizar CORS no Backend

No arquivo `DreamLuso.WebAPI/Program.cs`, o CORS já está configurado para aceitar qualquer origem se necessário. Mas você pode adicionar a URL do ngrok:

```bash
# Adicione a variável de ambiente ao iniciar o backend:
export CORS_ALLOWED_ORIGINS="https://dream-luso.vercel.app,https://abc123.ngrok-free.app"
```

Ou edite `appsettings.json`:
```json
"CorsSettings": {
  "AllowedOrigins": [
    "http://localhost:4200",
    "https://dream-luso.vercel.app",
    "https://abc123.ngrok-free.app"
  ]
}
```

## ⚠️ Importante

- **URL do ngrok muda a cada reinício** (no plano gratuito)
- Você precisará atualizar a variável `NG_APP_API_URL` no Vercel sempre que reiniciar o ngrok
- Para URL fixa, considere o plano pago do ngrok ou use outra solução (Cloudflare Tunnel, etc.)

## 🔄 Alternativa: Cloudflare Tunnel (URL fixa gratuita)

```bash
# Instalar cloudflared
wget https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64
chmod +x cloudflared-linux-amd64
sudo mv cloudflared-linux-amd64 /usr/local/bin/cloudflared

# Criar túnel
cloudflared tunnel --url http://localhost:5149
```

## 📝 Notas

- O backend local precisa estar acessível pela internet
- Certifique-se de que o firewall permite conexões na porta 5149
- O ngrok pode ter limites de requisições no plano gratuito

