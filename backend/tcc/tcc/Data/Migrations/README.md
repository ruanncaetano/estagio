# Migrations (SQL manual — MySQL 5.5)

Sem EF Core (o MySQL 5.5 não é suportado pelos providers atuais). O schema é
versionado como scripts `.sql` numerados, **forward-only** (sem `down`).

## Convenção
- `NNN_descricao.sql`, `NNN` sequencial de 3 dígitos (`000`, `001`, ...).
- Cada script termina com `INSERT INTO schema_migration (version) VALUES ('NNN');`.
- Nunca editar um script já aplicado — criar o próximo.
- `000_schema_migration.sql` cria a tabela de controle e roda primeiro.

## Aplicar (dev)
Banco: `fogo_erp` (ajuste se usar outro nome).

```sh
mysql -uroot -p -e "CREATE DATABASE IF NOT EXISTS fogo_erp CHARACTER SET utf8 COLLATE utf8_unicode_ci;"
mysql -uroot -p fogo_erp < 000_schema_migration.sql
mysql -uroot -p fogo_erp < 001_create_cliente.sql
```

Conferir o que já foi aplicado: `SELECT * FROM schema_migration ORDER BY version;`

## Pendente
Um runner que aplica automaticamente os scripts faltantes (comparando a pasta
com `schema_migration`) pode entrar depois; por ora a aplicação é manual.
