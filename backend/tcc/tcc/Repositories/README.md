# Repositories — acesso a dados

Isola o resto do sistema do MySQL. Só aqui há query/persistência.

- Par por módulo: `IClienteRepository` (contrato) + `ClienteRepository`
  (implementação), registrados por DI no `Program.cs`.
- **Nunca `DELETE` físico** de cadastro — inativação (`Ativo = false`) ou
  mudança de `Situacao`, conforme `docs/conventions.md`. (Exceção pontual:
  linha órfã de value-object sem histórico próprio, ex. `endereco` que ficou
  sem dono ao ser esvaziado — ver `ClienteRepository.AtualizarAsync`.)
- Acesso a dados: **Dapper** (SQL explícito). Ver `Data/README.md`.
- `ClienteRepository` é o exemplo de referência: `SelectBase` com `LEFT JOIN`
  nas tabelas filhas, escrita multi-tabela dentro de uma transação, helpers
  de unicidade para a RN validada no Service.
