# Models/Dtos — contratos da API

Objetos de entrada/saída dos endpoints — o que o Controller recebe e devolve.
Nunca expor a entidade de domínio direto na API.

- Convenção de nome: `CriarClienteRequest`, `ClienteResponse`,
  `AtualizarClienteRequest`.
- Regras de exibição do ERS moram aqui também (ex: DTO do PDF do orçamento
  não carrega custo interno/margem — RN03).
- Mapeamento DTO ↔ domínio: manual por enquanto (decidir AutoMapper na
  Estória 01 se a repetição incomodar).
