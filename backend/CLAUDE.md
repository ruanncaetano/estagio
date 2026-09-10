# Backend — Convenções

Carregado automaticamente quando o agente trabalha dentro de `/backend`.

## Stack
- C# / ASP.NET Web API (net10.0)
- MySQL 5.5 + **Dapper** (sem EF Core — ver `docs/decisions.md`). Schema em
  scripts SQL numerados: `backend/tcc/tcc/Data/Migrations/`.
- Swagger (Swashbuckle) para documentação, Serilog para log.

## Importante: modo de trabalho com C#
Ruan está usando este projeto também para praticar/aprender C#. **Prefira
guiar com perguntas e explicações antes de escrever código pronto**,
especialmente em decisões de modelagem ou padrões novos (ex: como estruturar
o snapshot do Contrato, como versionar o Adendo). Código direto é aceitável
para tarefas repetitivas ou já alinhadas (CRUDs básicos após o primeiro
exemplo do padrão).

## Estrutura de camadas

Web API em camadas (decisão de 2026-09-09 — ver `docs/decisions.md`). Projeto
único `backend/tcc/tcc/`, dividido por responsabilidade:

| Pasta | Papel | Regra |
|---|---|---|
| `Controllers/` | Entrada/saída HTTP (o "C") | Só recebe request, chama o Service, devolve response. Zero regra de negócio, zero SQL. |
| `Models/Domain/` | Entidades de domínio (o "M") | `PascalCase`, espelham `docs/diagrama-classes.md`. Sem atributos de API. |
| `Models/Dtos/` | Contratos de request/response da API | Nunca expor entidade de domínio direto. Regras de exibição do ERS moram aqui. |
| `Services/` | Regra de negócio | **Todas as validações de RN ficam aqui.** Orquestra Repositories. |
| `Repositories/` | Acesso a dados | Único lugar com query/persistência. Nunca `DELETE` físico. |
| `Data/` | Conexão MySQL + `Migrations/` (scripts SQL) | Dapper. Connection string fora do git (`appsettings.Local.json` / user-secrets). |
| `Middleware/` | Middleware de pipeline | Ex: `ExceptionHandlingMiddleware` (log de erros central). |

### Padrão por módulo
Para cada módulo (Cliente, Orçamento, ...), um par de interface + implementação
em cada camada de comportamento, registrados por DI no `Program.cs`:

```
IClienteRepository / ClienteRepository   → builder.Services.AddScoped<...>()
IClienteService    / ClienteService      → builder.Services.AddScoped<...>()
ClientesController  (usa IClienteService via construtor)
```

Fluxo de uma request: `Controller → Service (aplica RNs) → Repository → MySQL`.

Mapeamento DTO ↔ domínio é manual por enquanto; reavaliar AutoMapper na
Estória 01 se a repetição incomodar. Cada pasta tem um `README.md` com o
detalhe da camada.

## Convenções de nomenclatura
- Classes de domínio em `PascalCase`, espelhando os nomes do
  `docs/diagrama-classes.md` (ex: `Cliente`, `FichaTecnica`, `OrcamentoEquipe`).
- Tabelas em `snake_case`, singular, PK `id_<entidade>` — ver
  `docs/modelo-dados.md`.
- Toda entidade com regra de "não excluir fisicamente" precisa de uma coluna
  `ativo` (boolean) ou `situacao` (string), conforme o ERS especifica para
  cada uma — não usar always-boolean quando o ERS pede enum de situação
  (ex: Conta a Pagar tem Em aberto/Parcialmente paga/Paga/Vencida, não é só
  ativo/inativo).

## Documentação da API (Swagger) — obrigatória

Toda a API é documentada e testável pelo Swagger. Regras:

- **Todo endpoint** tem `/// <summary>` descrevendo o que faz, e
  `[ProducesResponseType(...)]` para cada status que pode retornar (200, 201,
  400, 404, 409...). DTOs de request/response também levam `/// <summary>`
  nas propriedades não óbvias. `HealthController` é o exemplo de referência.
- O `tcc.csproj` gera o XML de comentários (`GenerateDocumentationFile`), que
  o `AddSwaggerGen` consome — se faltar comentário, o Swagger fica pobre, não
  quebra o build (aviso 1591 silenciado).
- UI de teste: **`/doc`**. JSON OpenAPI: `/swagger/v1/swagger.json`. Ligado
  em todos os ambientes (config no `Program.cs`).

## Log (Serilog)

- Configurado no `Program.cs` (console + arquivo rotativo diário em `logs/`,
  14 dias). Nível em `appsettings.json` seção `Serilog`. `logs/` é gitignored.
- **Log de erros**: `Middleware/ExceptionHandlingMiddleware` captura qualquer
  exceção não tratada, registra com `LogError` (stack trace + método + rota +
  traceId) e responde `500` em `application/problem+json` sem vazar detalhe.
- Erro esperado de regra de negócio (validação de RN) **não** é exceção não
  tratada — o Service retorna resultado de falha e o Controller traduz para
  `400`/`409`. O middleware é só para o inesperado.

## Validações de regra de negócio (RN)
Cada estória do ERS tem uma lista de RNs numeradas. Ao implementar um
endpoint, mapeie explicitamente cada RN relevante para uma validação — e
comente no código qual RN aquela validação atende (ex:
`// RN05 — não permitir dois clientes ativos com mesmo CPF/CNPJ`). Isso
facilita auditar cobertura depois.
