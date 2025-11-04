#!/bin/bash

# Script para configurar ngrok rapidamente

echo "🔑 Configuração do ngrok"
echo "========================"
echo ""
echo "1. Acesse: https://dashboard.ngrok.com/signup"
echo "2. Crie uma conta (gratuita)"
echo "3. Vá em: https://dashboard.ngrok.com/get-started/your-authtoken"
echo "4. Copie seu authtoken"
echo ""
read -p "Cole seu authtoken aqui: " AUTHTOKEN

if [ -z "$AUTHTOKEN" ]; then
    echo "❌ Authtoken não fornecido"
    exit 1
fi

ngrok config add-authtoken "$AUTHTOKEN"

if [ $? -eq 0 ]; then
    echo "✅ ngrok configurado com sucesso!"
    echo ""
    echo "Agora você pode executar:"
    echo "  ./start-backend-with-ngrok.sh"
else
    echo "❌ Erro ao configurar ngrok"
    exit 1
fi

