// ⚠️ IMPORTANTE: Atualize esta URL com a URL real do Render após o deploy!
// 
// Passos:
// 1. Faça o deploy no Render
// 2. Copie a URL do serviço (ex: https://dreamluso-api.onrender.com)
// 3. Atualize apiUrl abaixo com a URL do Render
// 4. Faça commit e push para o Vercel fazer redeploy
//
// Exemplo: apiUrl: 'https://dreamluso-api.onrender.com/api'
//
// TEMPORÁRIO: Usando URL placeholder - ATUALIZE COM A URL REAL DO RENDER!

import { Environment } from './environment.interface';

export const environment: Environment = {
  production: true,
  apiUrl: 'https://dreamluso-api.onrender.com/api', // ← ATUALIZE COM A URL REAL DO RENDER
  baseUrl: 'https://dreamluso-api.onrender.com' // Base URL without /api for static files
};

