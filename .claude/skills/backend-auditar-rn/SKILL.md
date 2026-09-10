---
name: backend-auditar-rn
description: >-
  Audita a cobertura de regras de negócio (RN) de uma estória no código de
  backend do Fogo de Chão ERP: percorre cada RN numerada dos requisitos,
  procura a validação correspondente no código e produz um relatório de
  cobertura, registrando o que estiver faltando como pendência em
  ai/plan.md. Use ao fechar uma estória de backend ou ao revisar uma já
  entregue.
---

# Auditoria de cobertura de RN (backend)

Objetivo: garantir que toda regra de negócio da estória tem uma validação
rastreável no código — e que nada ficou de fora sem registro.

## 1. Listar as RNs da estória

- Abra `docs/requisitos/<modulo>.md`, localize a estória e copie a lista
  completa de RNs (RN01, RN02, ...), com o texto de cada uma.
- Marque quais são de **backend** (validação/persistência/fluxo), quais são
  de **exibição** (frontend) e quais são informativas (sem código). Só as de
  backend entram nesta auditoria — as de exibição vão para o agente de
  frontend.

## 2. Procurar cada RN no código

Para cada RN de backend, busque no `/backend` por:

- comentário `// RNxx` ou `RNxx (Estória …)`
- nome de método no padrão `ValidarRNxx_...`
- na falta dos dois, a lógica equivalente (mesmo sem etiqueta) — se existir
  a lógica mas faltar a etiqueta, é achado de "coberta sem rastreabilidade".

Use Grep no diretório `backend/`.

## 3. Classificar cada RN

| Status | Significado |
|---|---|
| ✅ Coberta e rastreável | validação existe e está etiquetada com o número da RN |
| ⚠️ Coberta sem rastreabilidade | lógica existe mas sem comentário/nome com a RN → adicionar a etiqueta |
| ❌ Não coberta | nenhuma validação encontrada |
| ➖ N/A no backend | RN é de exibição ou informativa |

## 4. Produzir o relatório

Formato de saída (texto, para o Ruan):

```
Estória XX — <nome> — cobertura de RN (backend)
RN01  ✅  <como está coberta / onde>
RN02  ❌  não encontrada — <o que faltaria fazer>
RN03  ⚠️  coberta em ServiceX.Metodo, sem etiqueta — adicionar // RN03
...
Resumo: N cobertas / M pendentes / K sem rastreabilidade
```

## 5. Registrar as pendências

Toda RN ❌ ou ⚠️ vira uma linha na seção de pendências da estória em
`ai/plan.md` — com o número da RN, o texto curto e o que falta. Não deixe
pendência viver só no relatório da conversa.

Se você mesmo puder corrigir os ⚠️ (só adicionar a etiqueta) na hora, faça e
marque como resolvido no relatório.
