# Script para iniciar o frontend Angular
Write-Host "Verificando Node.js..." -ForegroundColor Cyan

# Verificar se Node.js está instalado
try {
    $nodeVersion = node --version
    Write-Host "Node.js encontrado: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "ERRO: Node.js não está instalado ou não está no PATH" -ForegroundColor Red
    Write-Host "Por favor, instale o Node.js versão 20 de https://nodejs.org/" -ForegroundColor Yellow
    exit 1
}

# Verificar se npm está disponível
try {
    $npmVersion = npm --version
    Write-Host "npm encontrado: $npmVersion" -ForegroundColor Green
} catch {
    Write-Host "ERRO: npm não está disponível" -ForegroundColor Red
    exit 1
}

# Verificar se node_modules existe
if (!(Test-Path "node_modules")) {
    Write-Host "node_modules não encontrado. Instalando dependências..." -ForegroundColor Yellow
    npm install
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERRO: Falha ao instalar dependências" -ForegroundColor Red
        exit 1
    }
    Write-Host "Dependências instaladas com sucesso!" -ForegroundColor Green
} else {
    Write-Host "node_modules encontrado" -ForegroundColor Green
}

# Verificar se Angular CLI está disponível
Write-Host "Verificando Angular CLI..." -ForegroundColor Cyan
try {
    $ngVersion = npx ng version
    Write-Host "Angular CLI disponível" -ForegroundColor Green
} catch {
    Write-Host "Aviso: Angular CLI pode não estar instalado localmente" -ForegroundColor Yellow
}

# Iniciar o servidor de desenvolvimento
Write-Host "`nIniciando servidor de desenvolvimento Angular..." -ForegroundColor Cyan
Write-Host "Acesse: http://localhost:4200" -ForegroundColor Green
Write-Host "`nPressione Ctrl+C para parar o servidor`n" -ForegroundColor Yellow

npm start
