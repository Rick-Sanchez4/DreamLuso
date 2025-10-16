# 🔑 DreamLuso - Credenciais e Dados de SEED

> **Data:** 15/10/2025  
> **Status:** Aguardando firewall Azure propagar  
> **Ambiente:** Desenvolvimento/Testes

---

## 🎯 Como Popular o Banco de Dados

### **Opção 1: Script Automatizado (Recomendado)**

```bash
# Executar o script de seed
cd /home/rick-sanchez/DreamLuso
bash scripts/seed-database.sh
```

### **Opção 2: Via SQL Direto**

```bash
# Conectar ao Azure SQL e executar
sqlcmd -S mechasoft-server-2025.database.windows.net \
  -d DreamLusoDB \
  -U mechasoft_admin \
  -P 'Azure2025@Secure!' \
  -i scripts/seed-data.sql
```

### **Opção 3: Via Azure Data Studio**

1. Abrir Azure Data Studio
2. Conectar ao servidor: `mechasoft-server-2025.database.windows.net`
3. Banco: `DreamLusoDB`
4. Executar: `scripts/seed-data.sql`

---

## 👤 CREDENCIAIS DE USUÁRIOS

### 👑 **ADMINISTRADOR**

| Campo | Valor |
|-------|-------|
| **Email** | `admin@gmail.com` |
| **Senha** | `Admin123!` |
| **Role** | `Admin` |
| **Nome** | Admin DreamLuso |

**Acesso:**
- Dashboard: http://localhost:4200/admin/dashboard
- Permissões: Acesso total ao sistema

---

### 👔 **AGENTES IMOBILIÁRIOS**

#### **Agente 1: João Silva**
| Campo | Valor |
|-------|-------|
| **Email** | `joao.silva@dreamluso.pt` |
| **Senha** | `Agent123!` |
| **Role** | `RealEstateAgent` |
| **Licença AMI** | AMI-12345 |
| **Especialização** | Residencial |
| **Experiência** | 8 anos |

#### **Agente 2: Maria Santos**
| Campo | Valor |
|-------|-------|
| **Email** | `maria.santos@dreamluso.pt` |
| **Senha** | `Agent123!` |
| **Role** | `RealEstateAgent` |
| **Licença AMI** | AMI-23456 |
| **Especialização** | Luxury |
| **Experiência** | 12 anos |

#### **Agente 3: Pedro Costa**
| Campo | Valor |
|-------|-------|
| **Email** | `pedro.costa@dreamluso.pt` |
| **Senha** | `Agent123!` |
| **Role** | `RealEstateAgent` |
| **Licença AMI** | AMI-34567 |
| **Especialização** | Comercial |
| **Experiência** | 5 anos |

**Acesso:**
- Dashboard: http://localhost:4200/agent/dashboard
- Permissões: Gerenciar propriedades, visitas, contratos, propostas

---

### 👥 **CLIENTES**

#### **Cliente 1: Ana Rodrigues**
| Campo | Valor |
|-------|-------|
| **Email** | `ana.rodrigues@email.com` |
| **Senha** | `Client123!` |
| **Role** | `Client` |

#### **Cliente 2: Carlos Ferreira**
| Campo | Valor |
|-------|-------|
| **Email** | `carlos.ferreira@email.com` |
| **Senha** | `Client123!` |
| **Role** | `Client` |

#### **Cliente 3: Sofia Almeida**
| Campo | Valor |
|-------|-------|
| **Email** | `sofia.almeida@email.com` |
| **Senha** | `Client123!` |
| **Role** | `Client` |

#### **Cliente 4: Miguel Oliveira**
| Campo | Valor |
|-------|-------|
| **Email** | `miguel.oliveira@email.com` |
| **Senha** | `Client123!` |
| **Role** | `Client` |

#### **Cliente 5: Beatriz Lopes**
| Campo | Valor |
|-------|-------|
| **Email** | `beatriz.lopes@email.com` |
| **Senha** | `Client123!` |
| **Role** | `Client` |

**Acesso:**
- Dashboard: http://localhost:4200/client/dashboard
- Permissões: Ver propriedades, agendar visitas, fazer propostas

---

## 🏠 PROPRIEDADES DE EXEMPLO

### **Propriedade 1: Apartamento T3 em Lisboa**
- **Tipo:** Venda
- **Preço:** €385.000
- **Localização:** Alvalade, Lisboa
- **Área:** 120,5 m²
- **Quartos:** 3
- **Status:** Disponível
- **Agente:** João Silva

### **Propriedade 2: Moradia V4 no Porto**
- **Tipo:** Venda
- **Preço:** €650.000
- **Localização:** Foz do Douro, Porto
- **Área:** 280 m²
- **Quartos:** 4
- **Status:** Disponível
- **Agente:** Maria Santos

### **Propriedade 3: T2 em Cascais**
- **Tipo:** Arrendamento
- **Preço:** €1.200/mês
- **Localização:** Cascais
- **Área:** 85 m²
- **Quartos:** 2
- **Status:** Disponível
- **Agente:** Pedro Costa

### **Propriedade 4: Loja Comercial em Braga**
- **Tipo:** Arrendamento
- **Preço:** €1.800/mês
- **Localização:** Centro de Braga
- **Área:** 150 m²
- **Status:** Disponível
- **Agente:** João Silva

### **Propriedade 5: T1 em Coimbra**
- **Tipo:** Arrendamento
- **Preço:** €550/mês
- **Localização:** Centro Histórico, Coimbra
- **Área:** 55 m²
- **Quartos:** 1
- **Status:** Disponível
- **Agente:** Maria Santos

---

## 📊 DADOS ADICIONAIS

### **Visitas Agendadas**
- Ana Rodrigues → Apartamento T3 em Lisboa (daqui a 3 dias)
- Carlos Ferreira → Moradia V4 no Porto (visita concluída)

### **Comentários/Avaliações**
- Ana Rodrigues avaliou o T3 em Lisboa: ⭐⭐⭐⭐⭐ (5 estrelas)
- Sofia Almeida avaliou a Moradia V4: ⭐⭐⭐⭐⭐ (5 estrelas)

### **Propostas**
- Carlos Ferreira fez proposta de €370.000 para o T3 em Lisboa
- Status: Em Negociação

### **Notificações**
- João Silva recebeu notificação de nova visita
- João Silva recebeu notificação de nova proposta

---

## 🌐 URLs DO PROJETO

### **Backend (API)**
- URL: http://localhost:5149
- Swagger: http://localhost:5149/swagger
- Health: http://localhost:5149/health

### **Frontend (Angular)**
- URL: http://localhost:4200
- Login: http://localhost:4200/login
- Propriedades: http://localhost:4200/properties

### **Banco de Dados**
- Servidor: `mechasoft-server-2025.database.windows.net`
- Database: `DreamLusoDB`
- Admin: `mechasoft_admin`
- Password: `Azure2025@Secure!`

---

## 🔧 RESOLUÇÃO DE PROBLEMAS

### **Erro: Firewall Azure bloqueando conexão**

**Solução:**
```bash
# Verificar IP atual
curl -s https://api.ipify.org && echo

# Adicionar IP no Azure Portal
# Servidor → Firewalls and virtual networks → Add client IP
```

### **Erro: Admin já existe**

**Solução:** Usar as credenciais existentes:
- Email: `admin@gmail.com`
- Senha: `Admin123!`

### **Erro: API não responde**

**Solução:**
```bash
# Verificar se API está rodando
cd /home/rick-sanchez/DreamLuso/DreamLuso.WebAPI
dotnet run
```

### **Erro: Frontend não carrega**

**Solução:**
```bash
# Verificar se frontend está rodando
cd /home/rick-sanchez/DreamLuso/Presentation
npm start
```

---

## 📝 PRÓXIMOS PASSOS

1. ✅ **Aguardar firewall Azure propagar** (5-10 minutos)
2. ✅ **Executar script de seed:** `bash scripts/seed-database.sh`
3. ✅ **Fazer login como Admin:** http://localhost:4200/login
4. ✅ **Explorar dashboard e funcionalidades**
5. ✅ **Testar criação de propriedades**
6. ✅ **Testar fluxo completo:** Visita → Proposta → Contrato

---

## 🎯 TESTES SUGERIDOS

### **Como Admin**
1. Login com `admin@gmail.com`
2. Acessar dashboard admin
3. Ver lista de usuários
4. Ver lista de propriedades
5. Ver estatísticas gerais

### **Como Agente**
1. Login com `joao.silva@dreamluso.pt`
2. Criar nova propriedade
3. Ver visitas agendadas
4. Responder a propostas
5. Criar contrato

### **Como Cliente**
1. Login com `ana.rodrigues@email.com`
2. Pesquisar propriedades
3. Ver detalhes de propriedade
4. Agendar visita
5. Fazer proposta

---

## 📞 SUPORTE

Em caso de dúvidas ou problemas:
1. Verificar logs da API: `/home/rick-sanchez/DreamLuso/DreamLuso.WebAPI/bin/Debug/net9.0/logs`
2. Verificar console do navegador (F12)
3. Consultar documentação: `REVISAO_COMPLETA.md`

---

**Última atualização:** 15/10/2025  
**Versão:** 1.0.0  
**Status:** ⏳ Aguardando firewall Azure

