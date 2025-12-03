# Planejamento de Implementação - DreamLuso Front-End

## 📋 Visão Geral

Este documento detalha o planejamento de implementação das funcionalidades em falta e melhorias identificadas no front-end do DreamLuso.

**Estimativa Total:** 6-8 semanas  
**Metodologia:** Implementação incremental por fases, testando cada funcionalidade antes de avançar.

---

## 🎯 FASE 1: Funcionalidades Críticas (1-2 semanas)

### 1.1 Sistema de Favoritos
**Prioridade:** 🔴 Crítica  
**Estimativa:** 2-3 dias

**Tarefas:**
- [ ] Criar endpoint no backend para favoritos (se não existir)
  - `POST /favorites` - Adicionar favorito
  - `DELETE /favorites/{id}` - Remover favorito
  - `GET /favorites/client/{clientId}` - Listar favoritos do cliente
- [ ] Criar `FavoriteService` no front-end
- [ ] Adicionar botão "Adicionar aos Favoritos" na página de detalhes da propriedade
- [ ] Implementar página completa de favoritos (`favorites.component.ts`)
- [ ] Adicionar indicador visual de favorito nas propriedades
- [ ] Testar fluxo completo

**Ficheiros a criar/modificar:**
- `DreamLuso.WebAPI/Endpoints/FavoriteEndpoints.cs` (novo)
- `Presentation/src/app/core/services/favorite.service.ts` (novo)
- `Presentation/src/app/features/client/pages/favorites/favorites.component.ts` (completar)
- `Presentation/src/app/features/public/pages/property-detail/property-detail.component.ts` (adicionar botão)
- `Presentation/src/app/shared/components/property-card/property-card.component.ts` (adicionar ícone)

---

### 1.2 Aprovação/Rejeição de Agentes
**Prioridade:** 🔴 Crítica  
**Estimativa:** 1 dia

**Tarefas:**
- [ ] Verificar se endpoint de rejeição existe no backend
- [ ] Integrar `approveAgent()` com API real
- [ ] Integrar `rejectAgent()` com API real
- [ ] Adicionar tratamento de erros adequado
- [ ] Testar fluxo completo

**Ficheiros a modificar:**
- `Presentation/src/app/features/admin/pages/agents/agents.component.ts`

---

### 1.3 Paginação nas Listagens
**Prioridade:** 🔴 Crítica  
**Estimativa:** 2-3 dias

**Tarefas:**
- [ ] Criar componente reutilizável de paginação
- [ ] Implementar paginação na listagem de propriedades (pública)
- [ ] Implementar paginação na listagem de propriedades (agente)
- [ ] Implementar paginação na listagem de propostas (cliente)
- [ ] Implementar paginação na listagem de propostas (agente)
- [ ] Implementar paginação na listagem de visitas
- [ ] Testar performance com grandes volumes de dados

**Ficheiros a criar/modificar:**
- `Presentation/src/app/shared/components/pagination/pagination.component.ts` (novo)
- `Presentation/src/app/features/public/pages/properties/properties.component.ts`
- `Presentation/src/app/features/agent/pages/properties/properties.component.ts`
- `Presentation/src/app/features/client/pages/proposals/proposals.component.ts`
- `Presentation/src/app/features/agent/pages/proposals/proposals.component.ts`
- `Presentation/src/app/features/client/pages/visits/visits.component.ts`
- `Presentation/src/app/features/agent/pages/visits/visits.component.ts`

---

### 1.4 Configurações do Administrador
**Prioridade:** 🔴 Crítica  
**Estimativa:** 1-2 dias

**Tarefas:**
- [ ] Criar endpoint no backend para configurações (se não existir)
- [ ] Criar `SettingsService` no front-end
- [ ] Implementar `saveSettings()` com chamada à API
- [ ] Implementar `cancelChanges()` para resetar valores
- [ ] Adicionar carregamento de configurações existentes
- [ ] Adicionar validação de formulário
- [ ] Testar guardar/cancelar

**Ficheiros a criar/modificar:**
- `DreamLuso.WebAPI/Endpoints/SettingsEndpoints.cs` (novo, se necessário)
- `Presentation/src/app/core/services/settings.service.ts` (novo)
- `Presentation/src/app/features/admin/pages/settings/settings.component.ts` (completar)

---

## 🎯 FASE 2: Funcionalidades Importantes (2-3 semanas)

### 2.1 Negociação de Propostas
**Prioridade:** 🟡 Média  
**Estimativa:** 2-3 dias

**Tarefas:**
- [ ] Criar componente de chat/negociação
- [ ] Integrar com `ProposalService.addNegotiation()`
- [ ] Mostrar histórico de negociações
- [ ] Adicionar notificações quando há nova mensagem
- [ ] Testar fluxo de negociação

**Ficheiros a criar/modificar:**
- `Presentation/src/app/shared/components/negotiation-chat/negotiation-chat.component.ts` (novo)
- `Presentation/src/app/features/client/pages/proposals/proposal-detail.component.ts`
- `Presentation/src/app/features/agent/pages/proposals/proposals.component.ts`

---

### 2.2 Gestão de Contratos
**Prioridade:** 🟡 Média  
**Estimativa:** 2 dias

**Tarefas:**
- [ ] Verificar se página de contratos do agente está completa
- [ ] Criar página de contratos para clientes
- [ ] Integrar com endpoints existentes
- [ ] Adicionar visualização de detalhes do contrato
- [ ] Testar fluxo completo

**Ficheiros a criar/modificar:**
- `Presentation/src/app/features/client/pages/contracts/contracts.component.ts` (novo, se não existir)
- `Presentation/src/app/features/agent/pages/contracts/contracts.component.ts` (verificar/completar)
- `Presentation/src/app/core/services/contract.service.ts` (verificar se existe)

---

### 2.3 Tratamento de Erros Melhorado
**Prioridade:** 🟡 Média  
**Estimativa:** 2 dias

**Tarefas:**
- [ ] Criar páginas de erro personalizadas (404, 500)
- [ ] Melhorar mensagens de erro no `ErrorInterceptor`
- [ ] Adicionar retry automático para falhas de rede
- [ ] Implementar logging de erros (opcional: Sentry)
- [ ] Testar diferentes cenários de erro

**Ficheiros a criar/modificar:**
- `Presentation/src/app/features/public/pages/not-found/not-found.component.ts` (novo)
- `Presentation/src/app/features/public/pages/server-error/server-error.component.ts` (novo)
- `Presentation/src/app/core/interceptors/error.interceptor.ts` (melhorar)
- `Presentation/src/app/app.routes.ts` (adicionar rotas de erro)

---

### 2.4 Validação de Formulários
**Prioridade:** 🟡 Média  
**Estimativa:** 2 dias

**Tarefas:**
- [ ] Adicionar validação de telefone português
- [ ] Adicionar validação de NIF português
- [ ] Adicionar validação de código postal português
- [ ] Melhorar mensagens de erro de validação
- [ ] Adicionar validação em tempo real
- [ ] Testar todos os formulários

**Ficheiros a criar/modificar:**
- `Presentation/src/app/shared/validators/portuguese-validators.ts` (novo)
- Formulários existentes (adicionar validações)

---

## 🎯 FASE 3: Melhorias e Optimizações (3-4 semanas)

### 3.1 Performance e Optimização
**Prioridade:** 🟡 Média  
**Estimativa:** 3-4 dias

**Tarefas:**
- [ ] Implementar virtual scrolling para listas grandes
- [ ] Adicionar cache para dados estáticos
- [ ] Optimizar imagens (lazy loading)
- [ ] Implementar service workers (PWA)
- [ ] Adicionar compressão de assets
- [ ] Testar performance (Lighthouse)

**Ficheiros a criar/modificar:**
- `Presentation/src/app/shared/components/virtual-scroll/virtual-scroll.component.ts` (novo)
- `Presentation/angular.json` (optimizações)
- `Presentation/src/app/core/services/cache.service.ts` (novo)

---

### 3.2 Notificações em Tempo Real
**Prioridade:** 🟡 Média  
**Estimativa:** 3-4 dias

**Tarefas:**
- [ ] Verificar se backend suporta SignalR/WebSockets
- [ ] Implementar cliente SignalR no front-end
- [ ] Adicionar notificações push no browser
- [ ] Adicionar badge de notificações não lidas
- [ ] Testar notificações em tempo real

**Ficheiros a criar/modificar:**
- `Presentation/src/app/core/services/signalr.service.ts` (novo)
- `Presentation/src/app/core/services/notification.service.ts` (melhorar)
- Componentes de sidebar (adicionar badge)

---

### 3.3 Filtros Avançados
**Prioridade:** 🟡 Média  
**Estimativa:** 2-3 dias

**Tarefas:**
- [ ] Adicionar filtro por distrito (dropdown completo)
- [ ] Adicionar filtro por características (checkboxes)
- [ ] Adicionar filtro por certificado energético
- [ ] Guardar filtros na URL (query params)
- [ ] Implementar filtros guardados
- [ ] Testar todos os filtros

**Ficheiros a criar/modificar:**
- `Presentation/src/app/features/public/pages/properties/properties.component.ts`
- `Presentation/src/app/shared/components/advanced-filters/advanced-filters.component.ts` (novo)

---

### 3.4 Testes
**Prioridade:** 🟡 Média  
**Estimativa:** 4-5 dias

**Tarefas:**
- [ ] Configurar ambiente de testes
- [ ] Criar testes unitários para serviços principais
- [ ] Criar testes de componentes críticos
- [ ] Criar testes E2E para fluxos principais
- [ ] Integrar testes no CI/CD

**Ficheiros a criar:**
- Testes unitários para cada serviço
- Testes de componentes
- Testes E2E (Cypress ou Playwright)

---

## 🎯 FASE 4: Nice to Have (Futuro)

### 4.1 Analytics Completo
**Prioridade:** 🟢 Baixa  
**Estimativa:** 2 dias

### 4.2 Internacionalização
**Prioridade:** 🟢 Baixa  
**Estimativa:** 3-4 dias

### 4.3 Acessibilidade (A11y)
**Prioridade:** 🟢 Baixa  
**Estimativa:** 3-4 dias

### 4.4 Upload de Imagens Melhorado
**Prioridade:** 🟢 Baixa  
**Estimativa:** 2 dias

---

## 📊 Métricas de Sucesso

### Fase 1
- ✅ Sistema de favoritos funcional
- ✅ Agentes podem ser aprovados/rejeitados
- ✅ Todas as listagens têm paginação
- ✅ Configurações do admin podem ser guardadas

### Fase 2
- ✅ Negociação de propostas funcional
- ✅ Contratos completamente implementados
- ✅ Erros tratados adequadamente
- ✅ Formulários validados correctamente

### Fase 3
- ✅ Performance melhorada (Lighthouse score > 80)
- ✅ Notificações em tempo real funcionais
- ✅ Filtros avançados implementados
- ✅ Cobertura de testes > 60%

---

## 🧪 Estratégia de Testes

Para cada funcionalidade implementada:
1. **Teste Manual:** Verificar fluxo completo no browser
2. **Teste de Integração:** Verificar comunicação com backend
3. **Teste de Regressão:** Verificar que não quebrou funcionalidades existentes
4. **Teste de Performance:** Verificar que não degradou performance

---

## 📝 Notas de Implementação

- Sempre fazer commit após cada funcionalidade completa
- Testar em diferentes browsers (Chrome, Firefox, Safari)
- Testar em diferentes tamanhos de ecrã (mobile, tablet, desktop)
- Documentar alterações importantes
- Atualizar este documento conforme progresso

---

**Última atualização:** $(date)

