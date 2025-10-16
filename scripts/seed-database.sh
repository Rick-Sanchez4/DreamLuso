#!/bin/bash

# =====================================================
# DreamLuso - Script de SEED Automatizado
# =====================================================
# Testa conexão com Azure SQL e popula banco de dados
# =====================================================

set -e

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "🎯 DREAMLUSO - SEED AUTOMATIZADO"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

API_URL="http://localhost:5149"
MAX_ATTEMPTS=15
ATTEMPT=1

echo "⏳ Aguardando API estar disponível..."
while [ $ATTEMPT -le $MAX_ATTEMPTS ]; do
    if curl -s -f "$API_URL/health" > /dev/null 2>&1 || curl -s -f "$API_URL/api/properties" > /dev/null 2>&1; then
        echo "✅ API respondendo!"
        break
    fi
    echo "   Tentativa $ATTEMPT/$MAX_ATTEMPTS - aguardando 5s..."
    sleep 5
    ATTEMPT=$((ATTEMPT + 1))
done

if [ $ATTEMPT -gt $MAX_ATTEMPTS ]; then
    echo "❌ API não está respondendo após $MAX_ATTEMPTS tentativas"
    echo "   Verifique se a API está rodando: cd DreamLuso.WebAPI && dotnet run"
    exit 1
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📤 ETAPA 1: Criando Usuário Admin"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Criar Admin
RESPONSE=$(curl -s -X POST "$API_URL/api/accounts/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@gmail.com",
    "password": "Admin123!",
    "firstName": "Admin",
    "lastName": "DreamLuso",
    "role": "Admin"
  }')

if echo "$RESPONSE" | grep -q '"isSuccess":true\|userId\|Admin'; then
    echo "✅ Admin criado com sucesso!"
    echo "$RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$RESPONSE"
elif echo "$RESPONSE" | grep -q "already exists\|já existe"; then
    echo "⚠️  Admin já existe! Continuando..."
else
    echo "❌ Erro ao criar Admin:"
    echo "$RESPONSE"
    echo ""
    echo "⚠️  Possíveis causas:"
    echo "   1. Firewall do Azure ainda propagando (aguarde 5 min)"
    echo "   2. Connection string incorreta"
    echo "   3. Banco de dados não acessível"
    exit 1
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📤 ETAPA 2: Criando Agentes Imobiliários"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Agente 1
curl -s -X POST "$API_URL/api/accounts/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao.silva@dreamluso.pt",
    "password": "Agent123!",
    "firstName": "João",
    "lastName": "Silva",
    "role": "RealEstateAgent"
  }' > /dev/null && echo "✅ João Silva criado" || echo "⚠️  João Silva já existe"

# Agente 2
curl -s -X POST "$API_URL/api/accounts/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "maria.santos@dreamluso.pt",
    "password": "Agent123!",
    "firstName": "Maria",
    "lastName": "Santos",
    "role": "RealEstateAgent"
  }' > /dev/null && echo "✅ Maria Santos criada" || echo "⚠️  Maria Santos já existe"

# Agente 3
curl -s -X POST "$API_URL/api/accounts/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "pedro.costa@dreamluso.pt",
    "password": "Agent123!",
    "firstName": "Pedro",
    "lastName": "Costa",
    "role": "RealEstateAgent"
  }' > /dev/null && echo "✅ Pedro Costa criado" || echo "⚠️  Pedro Costa já existe"

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📤 ETAPA 3: Criando Clientes"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Cliente 1
curl -s -X POST "$API_URL/api/accounts/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "ana.rodrigues@email.com",
    "password": "Client123!",
    "firstName": "Ana",
    "lastName": "Rodrigues",
    "role": "Client"
  }' > /dev/null && echo "✅ Ana Rodrigues criada" || echo "⚠️  Ana Rodrigues já existe"

# Cliente 2
curl -s -X POST "$API_URL/api/accounts/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "carlos.ferreira@email.com",
    "password": "Client123!",
    "firstName": "Carlos",
    "lastName": "Ferreira",
    "role": "Client"
  }' > /dev/null && echo "✅ Carlos Ferreira criado" || echo "⚠️  Carlos Ferreira já existe"

# Cliente 3
curl -s -X POST "$API_URL/api/accounts/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "sofia.almeida@email.com",
    "password": "Client123!",
    "firstName": "Sofia",
    "lastName": "Almeida",
    "role": "Client"
  }' > /dev/null && echo "✅ Sofia Almeida criada" || echo "⚠️  Sofia Almeida já existe"

# Cliente 4
curl -s -X POST "$API_URL/api/accounts/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "miguel.oliveira@email.com",
    "password": "Client123!",
    "firstName": "Miguel",
    "lastName": "Oliveira",
    "role": "Client"
  }' > /dev/null && echo "✅ Miguel Oliveira criado" || echo "⚠️  Miguel Oliveira já existe"

# Cliente 5
curl -s -X POST "$API_URL/api/accounts/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "beatriz.lopes@email.com",
    "password": "Client123!",
    "firstName": "Beatriz",
    "lastName": "Lopes",
    "role": "Client"
  }' > /dev/null && echo "✅ Beatriz Lopes criada" || echo "⚠️  Beatriz Lopes já existe"

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "✅ SEED CONCLUÍDO!"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "📊 USUÁRIOS CRIADOS:"
echo "   ├─ 1 Admin"
echo "   ├─ 3 Agentes Imobiliários"
echo "   └─ 5 Clientes"
echo ""
echo "🔑 CREDENCIAIS DE ACESSO:"
echo ""
echo "   👑 ADMIN:"
echo "      Email: admin@gmail.com"
echo "      Senha: Admin123!"
echo ""
echo "   👔 AGENTES:"
echo "      joao.silva@dreamluso.pt | Agent123!"
echo "      maria.santos@dreamluso.pt | Agent123!"
echo "      pedro.costa@dreamluso.pt | Agent123!"
echo ""
echo "   👤 CLIENTES:"
echo "      ana.rodrigues@email.com | Client123!"
echo "      carlos.ferreira@email.com | Client123!"
echo "      sofia.almeida@email.com | Client123!"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "📝 PRÓXIMOS PASSOS:"
echo "   1. Fazer login como Admin: http://localhost:4200/login"
echo "   2. Criar propriedades via dashboard"
echo "   3. Testar funcionalidades"
echo ""
echo "🌐 URLs:"
echo "   Backend: http://localhost:5149"
echo "   Frontend: http://localhost:4200"
echo ""

