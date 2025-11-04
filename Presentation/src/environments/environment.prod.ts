export const environment = {
  production: true,
  // API URL pode ser configurada via variável de ambiente no Vercel
  // Se não definida, usa ngrok ou outra URL pública
  apiUrl: (typeof process !== 'undefined' && process.env['NG_APP_API_URL']) 
    ? process.env['NG_APP_API_URL'] + '/api'
    : 'https://dreamluso-api.onrender.com/api'
};

