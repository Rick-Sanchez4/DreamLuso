#!/bin/bash

# Script para iniciar ngrok e expor o backend local
# Pré-requisito: ngrok instalado (https://ngrok.com/download)

echo "🚀 Iniciando ngrok para expor backend local..."
echo "================================"

# Verificar se ngrok está instalado
if ! command -v ngrok &> /dev/null; then
    echo "❌ ngrok não está instalado!"
    echo ""
    echo "📥 Instale o ngrok:"
    echo "  1. Baixe em: https://ngrok.com/download"
    echo "  2. Ou instale via snap: snap install ngrok"
    echo "  3. Ou via wget: wget https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-linux-amd64.tgz"
    echo ""
    echo "🔑 Depois configure seu token:"
    echo "  ngrok config add-authtoken SEU_TOKEN_AQUI"
    echo ""
    exit 1
fi

# Verificar se o backend está rodando
if ! curl -s http://localhost:5149/health > /dev/null 2>&1; then
    echo "⚠️  Backend não está rodando em localhost:5149"
    echo "   Inicie o backend primeiro com: ./start-backend.sh"
    echo ""
    read -p "Deseja continuar mesmo assim? (s/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Ss]$ ]]; then
        exit 1
    fi
fi

echo "✅ Backend detectado em localhost:5149"
echo ""
echo "🌐 Iniciando ngrok..."
echo "   URL pública será exibida abaixo"
echo "   Pressione Ctrl+C para parar"
echo ""

# Iniciar ngrok
ngrok http 5149

