# 🗑️ INSTRUÇÕES PARA DELETAR RECURSOS DO AZURE

**Data:** 15/10/2025  
**Objetivo:** Remover TODOS os recursos do Azure para evitar cobranças

---

## ✅ RECURSOS A DELETAR

### **Resource Group:** `mechasoft-rg`
**Localização:** West Europe

**Contém:**
1. ✗ `DreamLusoDB` - SQL Database
2. ✗ `DV_RO_MechaSoft` - SQL Database
3. ✗ `mechasoft-server-2025` - SQL Server

**DELETANDO O RESOURCE GROUP, TUDO É DELETADO AUTOMATICAMENTE!**

---

## 🔧 MÉTODO 1: VIA PORTAL AZURE (RECOMENDADO)

### Passo 1: Aceder ao Resource Group

1. Ir para: https://portal.azure.com
2. No menu lateral, clicar em **"Resource groups"** (Grupos de recursos)
3. Procurar e clicar em: **`mechasoft-rg`**

### Passo 2: Deletar o Resource Group

4. No topo da página, clicar no botão **"Delete resource group"** (Eliminar grupo de recursos)
5. Na janela de confirmação:
   - Digitar: **`mechasoft-rg`** (nome exato)
   - Marcar checkbox: "Apply force delete for selected Virtual machines and Virtual machine scale sets"
   - Clicar em: **"Delete"** (Eliminar)

### Passo 3: Aguardar Conclusão

6. Azure vai mostrar uma notificação de progresso
7. Aguardar 2-5 minutos até completar
8. Verificar em "Notifications" (sino no topo) se deletou com sucesso

---

## 💻 MÉTODO 2: VIA AZURE CLI (MAIS RÁPIDO)

### Pré-requisitos

```bash
# Instalar Azure CLI (se não tiver)
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

### Comandos

```bash
# 1. Fazer login
az login

# 2. Deletar o resource group (sem confirmação)
az group delete --name mechasoft-rg --yes --no-wait

# 3. Verificar se foi deletado
az group list --output table
```

**NOTA:** O `--no-wait` faz o comando retornar imediatamente. A deleção continua em background.

---

## 📋 VERIFICAR SE FOI DELETADO

### Via Portal:

1. Ir para: https://portal.azure.com/#browse/resourcegroups
2. Verificar se `mechasoft-rg` **NÃO** aparece mais na lista

### Via CLI:

```bash
az group exists --name mechasoft-rg
# Deve retornar: false
```

---

## ⚠️ IMPORTANTE

### **O QUE SERÁ DELETADO:**

- ✗ Todas as bases de dados (DreamLusoDB e DV_RO_MechaSoft)
- ✗ O servidor SQL (mechasoft-server-2025)
- ✗ Todas as configurações de firewall
- ✗ Todos os backups automáticos
- ✗ O resource group

### **O QUE NÃO SERÁ DELETADO:**

- ✅ Sua conta Azure
- ✅ Sua subscrição
- ✅ Outros resource groups (se tiver)

### **CUSTO:**

- **ANTES:** Possível cobrança pelo SQL Database (mesmo free tier tem limites)
- **DEPOIS:** **€0,00** - NENHUMA COBRANÇA

---

## 🔍 OUTROS RESOURCES GROUPS ENCONTRADOS

Encontrei outros resource groups na sua conta que podem estar vazios:

1. `AmigoDoLarWebMVC20240313215707ResourceGroup` - East US
2. `appsvc_linux_centralus` - Central US
3. `azureapp-auto-alerts-5ba51d-rafaeloliveirarafa04_gmail_com` - West Europe
4. `DefaultResourceGroup-EUS` - East US
5. `game2` - East US 2
6. `game3` - East US 2
7. `gameBusca` - East US 2
8. `GameTest` - East US 2

**RECOMENDAÇÃO:** Verificar se estes têm recursos dentro. Se estiverem vazios, podem ser deletados também para manter a conta limpa.

### Como verificar:

```bash
# Via CLI - listar recursos em cada grupo
az resource list --resource-group NOME_DO_GRUPO --output table

# Se retornar vazio, deletar:
az group delete --name NOME_DO_GRUPO --yes --no-wait
```

---

## ✅ CHECKLIST FINAL

Após deletar, verificar:

- [ ] Resource group `mechasoft-rg` não aparece mais na lista
- [ ] Nenhum SQL Server ativo na subscrição
- [ ] Nenhuma SQL Database ativa
- [ ] Receber notificação de "Deletion succeeded"

---

## 📞 EM CASO DE PROBLEMAS

### Erro: "Cannot delete resource group"

**Possíveis causas:**
- Recursos com locks (bloqueios)
- Recursos sendo usados por outros serviços

**Solução:**
```bash
# Remover locks primeiro
az lock list --resource-group mechasoft-rg
az lock delete --name NOME_DO_LOCK --resource-group mechasoft-rg

# Depois deletar o grupo
az group delete --name mechasoft-rg --yes
```

### Erro: "Authorization failed"

**Solução:** Verificar se tem permissões de Owner ou Contributor na subscrição.

---

## 💰 CONFIRMAÇÃO DE ZERO COBRANÇAS

Após deletar tudo:

1. Ir para: https://portal.azure.com/#blade/Microsoft_Azure_Billing/SubscriptionsBlade
2. Clicar na sua subscrição
3. Ir em "Cost Analysis" (Análise de custos)
4. Verificar que não há custos futuros projetados

---

**Última atualização:** 15/10/2025 21:22  
**Status:** ⏳ Aguardando deleção manual
