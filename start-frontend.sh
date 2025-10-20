#!/bin/bash

# 🚀 DreamLuso - Iniciar Frontend
# Este script inicia o frontend Angular

set -e

echo "🏠 DreamLuso - Iniciando Frontend"
echo "================================="

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

# Verificar se Node.js está instalado
if ! command -v node &> /dev/null; then
    error "Node.js não está instalado. Por favor, instale o Node.js primeiro."
    echo "Ubuntu/Debian: curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash - && sudo apt-get install -y nodejs"
    exit 1
fi

# Verificar se npm está instalado
if ! command -v npm &> /dev/null; then
    error "npm não está instalado. Por favor, instale o npm primeiro."
    exit 1
fi

# Navegar para o diretório do frontend
cd Presentation

# Verificar se node_modules existe
if [ ! -d "node_modules" ]; then
    log "Instalando dependências do frontend..."
    npm install
    if [ $? -eq 0 ]; then
        success "Dependências instaladas com sucesso!"
    else
        error "Falha ao instalar dependências"
        exit 1
    fi
else
    log "Dependências já instaladas."
fi

# Verificar se a API está rodando
log "Verificando se a API está rodando..."
if curl -s http://localhost:5149/health &>/dev/null; then
    success "API está rodando!"
else
    warning "API não está rodando. Certifique-se de que o backend está iniciado."
    echo "Execute: ./start-backend.sh"
    echo ""
fi

echo ""
echo "🌐 URLs do Frontend:"
echo "   Frontend: http://localhost:4200"
echo "   API: http://localhost:5149"
echo ""
echo "🔧 Credenciais de Teste:"
echo "   Admin: admin@gmail.com / Admin123!"
echo "   Agente: joao.silva@dreamluso.pt / Agent123!"
echo "   Cliente: ana.rodrigues@email.com / Client123!"
echo ""

# Iniciar o frontend
log "Iniciando frontend Angular..."
ng serve --open
