#!/bin/bash

# 🚀 DreamLuso - Iniciar Backend + ngrok + Atualizar Frontend
# Este script automatiza todo o processo de desenvolvimento

set -e

# Cores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log() { echo -e "${BLUE}[INFO]${NC} $1"; }
success() { echo -e "${GREEN}[SUCCESS]${NC} $1"; }
warning() { echo -e "${YELLOW}[WARNING]${NC} $1"; }
error() { echo -e "${RED}[ERROR]${NC} $1"; }

echo "🚀 DreamLuso - Auto Setup (Backend + ngrok + Frontend)"
echo "======================================================"
echo ""

# Verificar se ngrok está instalado
if ! command -v ngrok &> /dev/null; then
    error "ngrok não está instalado!"
    echo ""
    echo "📥 Instale o ngrok:"
    echo "  sudo snap install ngrok"
    echo "  ou baixe de: https://ngrok.com/download"
    echo ""
    echo "🔑 Depois configure:"
    echo "  ngrok config add-authtoken SEU_TOKEN"
    echo ""
    exit 1
fi

# Verificar se ngrok está configurado
if [ ! -f ~/.config/ngrok/ngrok.yml ] && [ ! -f ~/.ngrok2/ngrok.yml ]; then
    warning "ngrok não está configurado!"
    echo ""
    echo "🔑 Configure com:"
    echo "  ngrok config add-authtoken SEU_TOKEN"
    echo "  (Token disponível em: https://dashboard.ngrok.com/get-started/your-authtoken)"
    echo ""
    exit 1
fi

# Verificar se Docker está rodando
if ! docker info &> /dev/null; then
    error "Docker não está rodando. Inicie o Docker primeiro."
    exit 1
fi

# Iniciar SQL Server se não estiver rodando
log "Verificando SQL Server..."
if ! docker ps | grep -q "dreamluso-sqlserver-local"; then
    log "Iniciando SQL Server..."
    docker compose -f docker-compose.sqlserver.yml up -d dreamluso-sqlserver
    log "Aguardando SQL Server ficar pronto..."
    sleep 10
else
    success "SQL Server já está rodando"
fi

# Verificar se backend está rodando
log "Verificando se backend está rodando..."
if ! curl -s http://localhost:5149/health > /dev/null 2>&1; then
    warning "Backend não está rodando. Iniciando..."
    
    # Iniciar backend em background
    log "Iniciando backend em background..."
    cd DreamLuso.WebAPI
    export ASPNETCORE_ENVIRONMENT=SqlServer
    export ConnectionStrings__DreamLusoDB="Server=localhost,1433;Database=DreamLusoDB;User Id=sa;Password=DreamLuso2025!;TrustServerCertificate=True;MultipleActiveResultSets=True;Encrypt=false;"
    export DatabaseProvider=SqlServer
    
    # Iniciar backend em background
    dotnet run > ../backend.log 2>&1 &
    BACKEND_PID=$!
    echo $BACKEND_PID > ../backend.pid
    
    cd ..
    
    log "Aguardando backend iniciar..."
    sleep 15
    
    # Verificar se iniciou
    if ! curl -s http://localhost:5149/health > /dev/null 2>&1; then
        error "Backend falhou ao iniciar. Verifique backend.log"
        exit 1
    fi
    success "Backend iniciado (PID: $BACKEND_PID)"
else
    success "Backend já está rodando"
fi

# Verificar se ngrok já está rodando
log "Verificando ngrok..."
NGROK_PID=$(pgrep -f "ngrok http 5149" || echo "")
if [ ! -z "$NGROK_PID" ]; then
    warning "ngrok já está rodando (PID: $NGROK_PID)"
    read -p "Deseja parar e reiniciar? (s/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Ss]$ ]]; then
        kill $NGROK_PID 2>/dev/null || true
        sleep 2
    else
        log "Usando ngrok existente..."
        # Tentar obter URL do ngrok API
        sleep 2
        NGROK_URL=$(curl -s http://localhost:4040/api/tunnels | grep -o '"public_url":"https://[^"]*"' | head -1 | cut -d'"' -f4 || echo "")
        if [ ! -z "$NGROK_URL" ]; then
            success "URL do ngrok: $NGROK_URL"
            echo "$NGROK_URL" > .ngrok_url
        fi
        exit 0
    fi
fi

# Iniciar ngrok em background
log "Iniciando ngrok..."
ngrok http --url=lifelong-jamal-scarless.ngrok-free.dev 5149 > ngrok.log 2>&1 &
NGROK_PID=$!
echo $NGROK_PID > ngrok.pid

log "Aguardando ngrok iniciar..."
sleep 5

# Usar domínio fixo do ngrok
NGROK_URL="https://lifelong-jamal-scarless.ngrok-free.dev"
log "Usando domínio ngrok: $NGROK_URL"
sleep 3

success "✅ ngrok rodando: $NGROK_URL"
echo "$NGROK_URL" > .ngrok_url

# Atualizar environment.prod.ts
log "Atualizando environment.prod.ts..."
ENV_FILE="Presentation/src/environments/environment.prod.ts"
API_URL="${NGROK_URL}/api"

# Criar backup
cp "$ENV_FILE" "${ENV_FILE}.backup"

# Atualizar arquivo
cat > "$ENV_FILE" << EOF
// ⚠️ AUTO-GENERADO - Este arquivo é atualizado automaticamente pelo script start-backend-with-ngrok.sh
// URL gerada em: $(date)
// ngrok URL: $NGROK_URL

export const environment = {
  production: true,
  apiUrl: '${API_URL}'
};
EOF

success "✅ environment.prod.ts atualizado com: $API_URL"

# Perguntar se deseja fazer commit e push
echo ""
read -p "Deseja fazer commit e push automático? (s/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Ss]$ ]]; then
    log "Fazendo commit e push..."
    git add "$ENV_FILE"
    git commit -m "chore: Update API URL to ngrok ($NGROK_URL)" || true
    git push origin main || warning "Falha ao fazer push (pode fazer manualmente depois)"
    success "✅ Commit e push concluídos!"
    log "Vercel fará redeploy automaticamente em alguns minutos"
else
    warning "Pulando commit. Você pode fazer manualmente depois:"
    echo "  git add $ENV_FILE"
    echo "  git commit -m 'Update API URL'"
    echo "  git push origin main"
fi

# Mostrar informações
echo ""
echo "======================================================"
success "✅ Setup completo!"
echo ""
echo "📊 Status:"
echo "  Backend: http://localhost:5149"
echo "  ngrok:   $NGROK_URL"
echo "  Frontend: https://dream-luso.vercel.app"
echo ""
echo "📝 Logs:"
echo "  Backend: tail -f backend.log"
echo "  ngrok:   tail -f ngrok.log"
echo "  ngrok UI: http://localhost:4040"
echo ""
echo "🛑 Para parar tudo:"
echo "  ./stop-backend-ngrok.sh"
echo ""
warning "⚠️  Mantenha este terminal aberto enquanto trabalha!"
echo "   (Pressione Ctrl+C para parar tudo)"
echo ""

# Manter script rodando e capturar Ctrl+C
trap 'echo ""; log "Parando serviços..."; ./stop-backend-ngrok.sh; exit 0' INT TERM

# Aguardar indefinidamente
while true; do
    sleep 60
    # Verificar se backend ainda está rodando
    if ! curl -s http://localhost:5149/health > /dev/null 2>&1; then
        warning "Backend parou! Verifique backend.log"
    fi
done

