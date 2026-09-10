-- 000_schema_migration.sql
-- Tabela de controle: registra quais scripts de migration já foram aplicados.
-- MySQL 5.5 — InnoDB, utf8.

CREATE TABLE schema_migration (
  version    VARCHAR(20) NOT NULL,
  applied_at TIMESTAMP   NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (version)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

INSERT INTO schema_migration (version) VALUES ('000');
