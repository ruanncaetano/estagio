# Repositories — acesso a dados

Isola o resto do sistema do MySQL. Só aqui há query/persistência.

- Par por módulo: `IClienteRepository` (contrato) + `ClienteRepository`
  (implementação), registrados por DI no `Program.cs`.
- **Nunca `DELETE` físico** — inativação (`Ativo = false`) ou mudança de
  `Situacao`, conforme `docs/conventions.md`.
- A tecnologia de acesso (EF Core vs Dapper) ainda **não foi decidida** —
  ver `Data/README.md` e a pendência em `ai/plan.md`.
