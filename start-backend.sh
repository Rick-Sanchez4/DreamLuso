#!/bin/bash

# 🚀 DreamLuso - Iniciar Backend com SQL Server
# Este script inicia o banco SQL Server e a API

set -e

echo "🏠 DreamLuso - Iniciando Backend"
echo "==============================="

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para log
log() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Verificar se Docker está rodando
if ! docker info &> /dev/null; then
    error "Docker não está rodando. Por favor, inicie o Docker primeiro."
    echo "sudo systemctl start docker"
    exit 1
fi

# Parar containers existentes
log "Parando containers existentes..."
docker compose -f docker-compose.sqlserver.yml down 2>/dev/null || true

# Iniciar SQL Server
log "Iniciando SQL Server..."
docker compose -f docker-compose.sqlserver.yml up -d dreamluso-sqlserver

# Aguardar SQL Server ficar pronto
log "Aguardando SQL Server ficar pronto..."
sleep 30

# Verificar se SQL Server está rodando
if docker ps | grep -q "dreamluso-sqlserver-local"; then
    success "SQL Server iniciado com sucesso!"
else
    error "Falha ao iniciar SQL Server"
    exit 1
fi

# Aguardar mais um pouco para garantir que está totalmente pronto
log "Aguardando SQL Server ficar totalmente pronto..."
sleep 20

# Testar conexão
log "Testando conexão com SQL Server..."
if docker exec dreamluso-sqlserver-local /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'DreamLuso2025!' -C -Q "SELECT 1" &>/dev/null; then
    success "Conexão com SQL Server estabelecida!"
else
    warning "Conexão ainda não está pronta. Aguardando mais um pouco..."
    sleep 30
fi

# Navegar para o diretório da API
cd DreamLuso.WebAPI

# Configurar ambiente
export ASPNETCORE_ENVIRONMENT=SqlServer
export ConnectionStrings__DreamLusoDB="Server=localhost,1433;Database=DreamLusoDB;User Id=sa;Password=DreamLuso2025!;TrustServerCertificate=True;MultipleActiveResultSets=True;Encrypt=false;"
export DatabaseProvider=SqlServer

log "Configuração:"
echo "  ASPNETCORE_ENVIRONMENT: $ASPNETCORE_ENVIRONMENT"
echo "  DatabaseProvider: $DatabaseProvider"

# Aplicar migrações
log "Aplicando migrações do banco de dados..."
dotnet ef database update --context ApplicationDbContext || {
    warning "Falha ao aplicar migrações via EF. Tentando via API..."
    # Se falhar, vamos tentar via API
}

# Verificar se o banco tem dados e executar seed se necessário
log "Verificando se o banco precisa ser populado..."
USER_COUNT=$(docker exec dreamluso-sqlserver-local /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'DreamLuso2025!' -C -Q "USE DreamLusoDB; SELECT COUNT(*) FROM [DreamLuso].[User];" -h -1 | tail -n 1 | tr -d ' ')

if [ "$USER_COUNT" = "0" ]; then
    log "Banco vazio detectado. Executando seed automático..."
    bash scripts/seed-database.sh
    success "Banco populado com dados de teste!"
else
    log "Banco já possui $USER_COUNT usuários. Seed não necessário."
fi

# Iniciar API
log "Iniciando API DreamLuso..."
echo ""
echo "🌐 URLs da Aplicação:"
echo "   API: http://localhost:5149"
echo "   Swagger: http://localhost:5149/swagger"
echo "   Health: http://localhost:5149/health"
echo ""
echo "📊 Banco de Dados:"
echo "   Host: localhost:1433"
echo "   Database: DreamLusoDB"
echo "   User: sa"
echo "   Password: DreamLuso2025!"
echo ""

# Executar a aplicação
dotnet run
