-- 002_create_endereco.sql
-- Extrai o endereço para uma tabela própria, reaproveitável por Cliente,
-- Fornecedor e Funcionário (decisão de 2026-09-09 — ver docs/decisions.md).
-- O ERS / docs/modelo-dados.md traziam o endereço como colunas soltas em cada
-- cadastro; aqui vira entidade `endereco` referenciada por FK opcional.
--
-- MySQL 5.5: InnoDB + utf8. Endereço é opcional (id_endereco NULL) e 1:1 com
-- o dono (não compartilhado entre dois clientes).

CREATE TABLE endereco (
  id_endereco INT UNSIGNED NOT NULL AUTO_INCREMENT,
  rua         VARCHAR(255) NULL,
  numero      VARCHAR(20)  NULL COMMENT 'Texto — aceita "s/n"',
  bairro      VARCHAR(120) NULL,
  cidade      VARCHAR(120) NULL,
  cep         VARCHAR(9)   NULL,
  PRIMARY KEY (id_endereco)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- cliente: troca as colunas de endereço por uma FK opcional para `endereco`.
-- (a tabela está vazia, então DROP COLUMN é seguro.)
ALTER TABLE cliente
  ADD COLUMN id_endereco INT UNSIGNED NULL AFTER email,
  ADD CONSTRAINT fk_cliente_endereco FOREIGN KEY (id_endereco)
    REFERENCES endereco (id_endereco) ON DELETE SET NULL ON UPDATE CASCADE,
  DROP COLUMN rua,
  DROP COLUMN numero,
  DROP COLUMN bairro,
  DROP COLUMN cidade,
  DROP COLUMN cep;

INSERT INTO schema_migration (version) VALUES ('002');
