-- 001_create_cliente.sql
-- Estória 01 — Gerenciar Clientes (docs/requisitos/comercial.md)
--
-- Modelagem: herança table-per-type — uma tabela comum `cliente` +
-- especializações `cliente_pf` / `cliente_pj` (relação 1:0..1, PK = FK).
-- Fonte: docs/modelo-dados.md, reconciliado com o ERS (comercial.md):
--   + nome_fantasia (está no ERS, faltava no modelo)
--   + inscricao_estadual (está no modelo, opcional)
--
-- MySQL 5.5:
--   - ENGINE=InnoDB (necessário para FK).
--   - CHARSET=utf8 (não utf8mb4: no 5.5 o limite de 767 bytes por índice
--     inviabiliza VARCHAR longo + índice com 4 bytes/char).
--   - Sem CHECK constraint e sem coluna gerada (só 5.7+). O vínculo
--     tipo_cliente <-> tabela filha correta é invariante de ClienteService.
--   - RN05 ("nenhum par de clientes ATIVOS com o mesmo CPF/CNPJ") NÃO é
--     UNIQUE aqui: um UNIQUE simples barraria também dois inativos ou um
--     ativo + um inativo com o mesmo documento, o que o ERS permite.
--     A unicidade-entre-ativos é validada no ClienteService. Os índices
--     abaixo em cpf/cnpj são só para acelerar essa consulta.

CREATE TABLE cliente (
  id_cliente   INT UNSIGNED NOT NULL AUTO_INCREMENT,
  tipo_cliente VARCHAR(2)   NOT NULL COMMENT 'PF | PJ (validado no ClienteService — RN02)',
  nome         VARCHAR(255) NOT NULL COMMENT 'Nome (PF) / Razão Social (PJ)',
  telefone     VARCHAR(20)  NULL     COMMENT 'WhatsApp',
  email        VARCHAR(255) NULL,
  rua          VARCHAR(255) NULL,
  numero       VARCHAR(20)  NULL,
  bairro       VARCHAR(120) NULL,
  cidade       VARCHAR(120) NULL,
  cep          VARCHAR(9)   NULL,
  ativo        TINYINT(1)   NOT NULL DEFAULT 1 COMMENT 'RN06/RN07 — inativação, nunca DELETE físico',
  PRIMARY KEY (id_cliente),
  KEY idx_cliente_ativo (ativo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE cliente_pf (
  id_cliente INT UNSIGNED NOT NULL,
  cpf        VARCHAR(11)  NULL COMMENT 'Somente dígitos. Opcional (RN03). Unicidade-entre-ativos: RN05 no ClienteService',
  rg         VARCHAR(20)  NULL,
  PRIMARY KEY (id_cliente),
  KEY idx_cliente_pf_cpf (cpf),
  CONSTRAINT fk_cliente_pf_cliente FOREIGN KEY (id_cliente)
    REFERENCES cliente (id_cliente) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE cliente_pj (
  id_cliente         INT UNSIGNED NOT NULL,
  cnpj               VARCHAR(14)  NULL COMMENT 'Somente dígitos. Opcional (RN04). Unicidade-entre-ativos: RN05 no ClienteService',
  nome_fantasia      VARCHAR(255) NULL,
  nome_responsavel   VARCHAR(255) NULL,
  inscricao_estadual VARCHAR(20)  NULL,
  PRIMARY KEY (id_cliente),
  KEY idx_cliente_pj_cnpj (cnpj),
  CONSTRAINT fk_cliente_pj_cliente FOREIGN KEY (id_cliente)
    REFERENCES cliente (id_cliente) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

INSERT INTO schema_migration (version) VALUES ('001');
