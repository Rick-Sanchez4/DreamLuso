#!/bin/bash

echo "╔═══════════════════════════════════════════════════════════════════╗"
echo "║                                                                    ║"
echo "║              🛑 DREAMLUSO - STOP SCRIPT 🛑                        ║"
echo "║                                                                    ║"
echo "╚═══════════════════════════════════════════════════════════════════╝"
echo ""

# Stop API
echo "🔄 Parando DreamLuso API..."
pkill -f "dotnet.*DreamLuso.WebAPI" 2>/dev/null
if [ $? -eq 0 ]; then
    echo "✅ API parada com sucesso!"
else
    echo "ℹ️  API não estava rodando"
fi

echo ""

# Stop SQL Server container
echo "🔄 Parando SQL Server container..."
docker stop dreamluso-sql 2>/dev/null
if [ $? -eq 0 ]; then
    echo "✅ SQL Server container parado!"
else
    echo "ℹ️  SQL Server container não estava rodando"
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "✅ DreamLuso parado com sucesso!"
echo ""
echo "Para iniciar novamente, execute: ./start-dreamluso.sh"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

