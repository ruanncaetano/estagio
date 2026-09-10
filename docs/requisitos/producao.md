# Módulo Produção

Estórias: 03 (Insumo), 04 (Ficha Técnica), 05 (Fornecedores).

> Nota: a Estória 14 (Ficha de Produção do Evento) também é "produção" no
> sentido de negócio, mas foi colocada em `operacao-eventos.md` porque
> depende diretamente do Evento — ver esse arquivo.

---

## Estória 03 — Gerenciar Insumo

**Estória**: Como administrador, quero cadastrar, consultar, editar e
inativar insumos, vinculando-os a fornecedores e seus respectivos preços,
para manter uma base de insumos organizada, com custo de referência
confiável para uso nas fichas técnicas.

### Campos
- Código, Nome, Categoria, Unidade de medida (lista fixa: **UN, KG**).
- Vínculo opcional com um ou mais Fornecedores, cada um com seu preço
  cadastrado manualmente (independente de compra/cotação).

### Comportamento
- Preço médio = média dos **3 preços mais recentes** cadastrados (de
  qualquer fornecedor).
- Exibir os 3 últimos preços cadastrados com respectivo fornecedor.
- Consulta por nome, código ou categoria.
- Não há controle de estoque nesta funcionalidade.

### Regras de negócio
- RN01 — Apenas Administrador cadastra/gerencia insumos.
- RN02 — Permitir insumos com nomes duplicados (sem unicidade de nome).
- RN03 — Insumo pode ter zero, um ou vários fornecedores.
- RN04 — Mesmo fornecedor pode ter preço próprio para o insumo.
- RN05 — Preço médio = base nos 3 preços mais recentes, independente do fornecedor.
- RN06 — Não excluir fisicamente.
- RN07 — Deve ter situação ativo/inativo.
- RN08 — Histórico de preços preservado após inativação.
- RN09 — Sem controle de estoque de insumos nesta funcionalidade.

---

## Estória 04 — Gerenciar Ficha Técnica

**Estória**: Como administrador, quero cadastrar, consultar, editar e
inativar fichas técnicas de receitas, compostas por insumos e suas
quantidades, para padronizar o preparo dos pratos e calcular
automaticamente o custo de produção de cada item do cardápio.

### Campos
- Nome da receita, Descrição, Rendimento total + unidade de medida,
  Pessoas atendidas (por receita), Tempo de preparo (min), Instruções de
  preparo (passo a passo).
- Composição: um ou mais insumos, cada um com sua quantidade.

### Cálculos automáticos
```
Antecedência (min) = Tempo de Preparo + 10%
Subtotal do insumo = quantidade × preço unitário do insumo
Custo total da ficha = soma dos subtotais de todos os insumos
```

### Regras de negócio
- RN01 — Apenas Administrador cadastra/gerencia fichas técnicas.
- RN02 — Permitir nomes de receita duplicados.
- RN03 — Ficha técnica deve conter um ou mais insumos, cada um com quantidade.
- RN04 — Não permitir vincular insumos **inativos** a uma ficha técnica.
- RN05 — Custo de cada insumo = quantidade × preço unitário.
- RN06 — Custo total da ficha = soma dos subtotais.
- RN07 — Antecedência = Tempo de Preparo + 10%.
- RN08 — Não excluir fisicamente.
- RN09 — Deve ter situação ativa/inativa.
- RN10 — Histórico e vínculos (uso em orçamentos anteriores) preservados
  após inativação.

---

## Estória 05 — Gerenciar Fornecedores

**Estória**: Como administrador ou vendedor, quero cadastrar, consultar,
editar e inativar fornecedores, para manter uma base organizada de
fornecedores, utilizada no vínculo de preços de insumos.

### Campos
- Nome, CNPJ/CPF (opcional), E-mail, Telefone, Endereço.

### Regras de negócio
- RN01 — Apenas Administrador ou Vendedor cadastra/gerencia fornecedores.
- RN02 — CNPJ/CPF não é obrigatório.
- RN03 — Quando informado, CNPJ/CPF não pode se repetir entre fornecedores,
  mesmo que um esteja inativo.
- RN04 — Não excluir fisicamente.
- RN05 — Deve ter situação ativo/inativo.
- RN06 — Vínculo do fornecedor com insumos e histórico de preços
  preservado após inativação.
