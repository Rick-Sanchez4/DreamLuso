#!/bin/bash

# 🛑 DreamLuso - Parar Backend e ngrok

echo "🛑 Parando serviços..."

# Parar ngrok
if [ -f ngrok.pid ]; then
    NGROK_PID=$(cat ngrok.pid)
    if ps -p $NGROK_PID > /dev/null 2>&1; then
        kill $NGROK_PID 2>/dev/null || true
        echo "✅ ngrok parado"
    fi
    rm -f ngrok.pid
fi

# Parar backend
if [ -f backend.pid ]; then
    BACKEND_PID=$(cat backend.pid)
    if ps -p $BACKEND_PID > /dev/null 2>&1; then
        kill $BACKEND_PID 2>/dev/null || true
        echo "✅ Backend parado"
    fi
    rm -f backend.pid
fi

# Limpar arquivos temporários
rm -f .ngrok_url ngrok.log backend.log

echo "✅ Tudo parado!"

