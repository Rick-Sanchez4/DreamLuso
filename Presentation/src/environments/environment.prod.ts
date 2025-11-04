// ⚠️ IMPORTANTE: Atualize esta URL quando iniciar o ngrok!
// 
// Passos:
// 1. Inicie o backend local: ./start-backend.sh
// 2. Inicie o ngrok: ./start-ngrok.sh
// 3. Copie a URL do ngrok (ex: https://abc123.ngrok-free.app)
// 4. Atualize apiUrl abaixo com a URL do ngrok
// 5. Faça commit e push para o Vercel fazer redeploy
//
// Exemplo: apiUrl: 'https://abc123.ngrok-free.app/api'
//
// TEMPORÁRIO: Usando URL placeholder para permitir build
// Atualize com a URL real do ngrok antes de usar

export const environment = {
  production: true,
  apiUrl: 'https://dreamluso-api.onrender.com/api' // ← Atualize com URL do ngrok quando disponível
};

