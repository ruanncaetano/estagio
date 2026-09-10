# Services — regra de negócio

Onde vivem as **validações de regra de negócio (RN)** de cada estória. O
Controller chama o Service; o Service orquestra Repositories e aplica as RNs.

- Par por módulo: `IClienteService` (contrato) + `ClienteService` (implementação),
  registrados por DI no `Program.cs`.
- Toda RN implementada é rastreável no código com o número e a estória de
  origem — ex: `// E01 RN05 — não permitir dois clientes ativos com mesmo CPF/CNPJ`
  ou método `ValidarRN05_CpfCnpjUnicoEntreAtivos()`. Ver `docs/conventions.md`.
- Sem `HttpContext` aqui e sem SQL aqui — HTTP é do Controller, dados são do
  Repository.
