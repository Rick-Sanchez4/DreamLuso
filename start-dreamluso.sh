#!/bin/bash

echo "╔═══════════════════════════════════════════════════════════════════╗"
echo "║                                                                    ║"
echo "║              🚀 DREAMLUSO - STARTUP SCRIPT 🚀                     ║"
echo "║                                                                    ║"
echo "╚═══════════════════════════════════════════════════════════════════╝"
echo ""

# Check if Docker is running
if ! docker ps > /dev/null 2>&1; then
    echo "❌ Docker não está rodando. Por favor, inicie o Docker primeiro."
    exit 1
fi

echo "✅ Docker está rodando"
echo ""

# Start SQL Server container if not running
if ! docker ps | grep -q dreamluso-sql; then
    echo "🔄 Iniciando SQL Server..."
    
    # Check if container exists but is stopped
    if docker ps -a | grep -q dreamluso-sql; then
        docker start dreamluso-sql
    else
        docker run -e "ACCEPT_EULA=Y" \
                   -e "SA_PASSWORD=YourStrong@Passw0rd" \
                   -p 1433:1433 \
                   --name dreamluso-sql \
                   -d mcr.microsoft.com/mssql/server:2022-latest
    fi
    
    echo "⏳ Aguardando SQL Server inicializar..."
    sleep 15
    echo "✅ SQL Server iniciado!"
else
    echo "✅ SQL Server já está rodando"
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Check if database exists and apply migrations if needed
echo "🔄 Verificando migrations..."
cd DreamLuso.Data
dotnet ef database update --startup-project ../DreamLuso.WebAPI > /dev/null 2>&1

if [ $? -eq 0 ]; then
    echo "✅ Migrations aplicadas com sucesso!"
else
    echo "⚠️  Erro ao aplicar migrations. Execute manualmente se necessário."
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Start the API
echo "🚀 Iniciando DreamLuso API..."
cd ../DreamLuso.WebAPI

echo ""
echo "✅ API disponível em:"
echo "   🌐 HTTP:  http://localhost:5149"
echo "   🔒 HTTPS: https://localhost:7001"
echo "   📚 Swagger: https://localhost:7001/swagger"
echo "   ❤️  Health: https://localhost:7001/health"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

dotnet run

