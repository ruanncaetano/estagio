# Tasks — backlog granular

> Diferença pro `ai/plan.md`: o plan.md marca status por **estória inteira**.
> Aqui quebramos a estória atual (ou as próximas) em passos técnicos
> concretos, que sobrevivem entre sessões (diferente do TodoWrite, que é só
> da sessão corrente). Quando uma estória é concluída, apague as tasks dela
> daqui e marque o checkbox correspondente no `ai/plan.md`.

## Convenção de uso

Ao iniciar uma estória nova, quebrar em algo como:
```
## Estória XX — Nome
- [ ] Migration da tabela X
- [ ] Entidade/DTO em C#
- [ ] Endpoint(s) — método/rota
- [ ] Validações de RN: RN01, RN03, RN05...
- [ ] Tela de listagem
- [ ] Modal/form de cadastro-edição
- [ ] Teste manual do fluxo completo
```

Task só sai daqui quando estiver de fato pronta (não só "código escrito" —
também validada contra as RNs do módulo em `docs/requisitos/`).

## Fila atual

_(vazio — nenhuma estória iniciada ainda; ver `ai/plan.md` para a ordem
sugerida e escolher a primeira ao começar a implementação)_
