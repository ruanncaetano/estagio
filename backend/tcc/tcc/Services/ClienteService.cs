using tcc.Common;
using tcc.Models.Domain;
using tcc.Models.Dtos;
using tcc.Repositories;

namespace tcc.Services;

/// <summary>
/// Regras de negócio da Estória 01 — Gerenciar Clientes. Cada validação de RN
/// está marcada com um comentário <c>// E01 RNxx</c> para auditoria de
/// cobertura (ver <c>backend/CLAUDE.md</c> e <c>docs/conventions.md</c>).
/// </summary>
public sealed class ClienteService : IClienteService
{
    private readonly IClienteRepository _repositorio;

    public ClienteService(IClienteRepository repositorio)
        => _repositorio = repositorio;

    // E01 RN01 — TODO: restringir a Administrador / Vendedor-Comercial quando
    // houver autenticação no projeto. Hoje não há camada de auth; a restrição
    // de perfil ainda não é aplicável. Pendência registrada em ai/plan.md.

    public async Task<IReadOnlyList<ClienteResponse>> ListarAsync(bool? ativo, string? busca, CancellationToken cancellationToken = default)
    {
        var clientes = await _repositorio.ListarAsync(ativo, busca, cancellationToken);
        return clientes.Select(ClienteResponse.DoDominio).ToList();
    }

    public async Task<Result<ClienteResponse>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _repositorio.ObterPorIdAsync(id, cancellationToken);
        return cliente is null
            ? Result<ClienteResponse>.NaoEncontrado($"Cliente {id} não encontrado.")
            : Result<ClienteResponse>.Ok(ClienteResponse.DoDominio(cliente));
    }

    public async Task<Result<ClienteResponse>> CriarAsync(CriarClienteRequest request, CancellationToken cancellationToken = default)
    {
        // E01 RN02 — o cadastro aceita Pessoa Física e Pessoa Jurídica; o tipo
        // precisa ser reconhecido ("PF" ou "PJ").
        if (!TipoClienteExtensions.TentarConverter(request.Tipo, out var tipo))
        {
            return Result<ClienteResponse>.Validacao("Tipo de cliente deve ser 'PF' ou 'PJ'.", "E01_RN02");
        }

        var nome = request.Nome?.Trim();
        if (string.IsNullOrWhiteSpace(nome))
        {
            // Campo obrigatório (seção "Campos" da Estória 01).
            return Result<ClienteResponse>.Validacao("Nome (ou Razão Social) é obrigatório.", "E01_NOME");
        }

        // E01 RN03 (PF) / E01 RN04 (PJ) — documento é opcional; quando informado,
        // guardamos só os dígitos e exigimos o tamanho correto (11 CPF / 14 CNPJ).
        var documento = SomenteDigitos(tipo == TipoCliente.Pf ? request.Cpf : request.Cnpj);
        var erroDocumento = ValidarDocumento(tipo, documento);
        if (erroDocumento is not null)
        {
            return Result<ClienteResponse>.Validacao(erroDocumento, tipo == TipoCliente.Pf ? "E01_RN03" : "E01_RN04");
        }

        // E01 RN05 — não permitir dois clientes ATIVOS com o mesmo CPF/CNPJ.
        if (documento is not null && await ExisteOutroAtivoComDocumentoAsync(tipo, documento, idExcluir: null, cancellationToken))
        {
            return Result<ClienteResponse>.Conflito(
                $"Já existe um cliente ativo com este {(tipo == TipoCliente.Pf ? "CPF" : "CNPJ")}.", "E01_RN05");
        }

        var cliente = new Cliente
        {
            Tipo = tipo,
            Nome = nome,
            Telefone = Normalizar(request.Telefone),
            Email = Normalizar(request.Email),
            Ativo = true, // E01 RN07 — nasce ativo.
            Endereco = ConstruirEndereco(request.Endereco),
        };

        if (tipo == TipoCliente.Pf)
        {
            cliente.Pf = new ClientePf { Cpf = documento, Rg = Normalizar(request.Rg) };
        }
        else
        {
            cliente.Pj = new ClientePj
            {
                Cnpj = documento,
                NomeFantasia = Normalizar(request.NomeFantasia),
                NomeResponsavel = Normalizar(request.NomeResponsavel),
                InscricaoEstadual = Normalizar(request.InscricaoEstadual),
            };
        }

        var novoId = await _repositorio.InserirAsync(cliente, cancellationToken);
        var criado = await _repositorio.ObterPorIdAsync(novoId, cancellationToken);
        return Result<ClienteResponse>.Ok(ClienteResponse.DoDominio(criado!));
    }

    public async Task<Result<ClienteResponse>> AtualizarAsync(int id, AtualizarClienteRequest request, CancellationToken cancellationToken = default)
    {
        var existente = await _repositorio.ObterPorIdAsync(id, cancellationToken);
        if (existente is null)
        {
            return Result<ClienteResponse>.NaoEncontrado($"Cliente {id} não encontrado.");
        }

        var nome = request.Nome?.Trim();
        if (string.IsNullOrWhiteSpace(nome))
        {
            return Result<ClienteResponse>.Validacao("Nome (ou Razão Social) é obrigatório.", "E01_NOME");
        }

        // O tipo PF/PJ não muda numa edição (ver AtualizarClienteRequest).
        var tipo = existente.Tipo;

        // E01 RN03 / E01 RN04 — mesma regra de documento da criação.
        var documento = SomenteDigitos(tipo == TipoCliente.Pf ? request.Cpf : request.Cnpj);
        var erroDocumento = ValidarDocumento(tipo, documento);
        if (erroDocumento is not null)
        {
            return Result<ClienteResponse>.Validacao(erroDocumento, tipo == TipoCliente.Pf ? "E01_RN03" : "E01_RN04");
        }

        // E01 RN05 — não pode haver dois clientes ATIVOS com o mesmo documento.
        // Só é conflito se este cliente está ativo (dois inativos, ou ativo+inativo,
        // são permitidos pelo ERS). A reativação tem a sua própria checagem.
        if (documento is not null
            && existente.Ativo
            && await ExisteOutroAtivoComDocumentoAsync(tipo, documento, idExcluir: id, cancellationToken))
        {
            return Result<ClienteResponse>.Conflito(
                $"Já existe outro cliente ativo com este {(tipo == TipoCliente.Pf ? "CPF" : "CNPJ")}.", "E01_RN05");
        }

        existente.Nome = nome;
        existente.Telefone = Normalizar(request.Telefone);
        existente.Email = Normalizar(request.Email);
        existente.Endereco = ConstruirEndereco(request.Endereco);

        if (tipo == TipoCliente.Pf)
        {
            existente.Pf!.Cpf = documento;
            existente.Pf!.Rg = Normalizar(request.Rg);
        }
        else
        {
            existente.Pj!.Cnpj = documento;
            existente.Pj!.NomeFantasia = Normalizar(request.NomeFantasia);
            existente.Pj!.NomeResponsavel = Normalizar(request.NomeResponsavel);
            existente.Pj!.InscricaoEstadual = Normalizar(request.InscricaoEstadual);
        }

        await _repositorio.AtualizarAsync(existente, cancellationToken);
        var atualizado = await _repositorio.ObterPorIdAsync(id, cancellationToken);
        return Result<ClienteResponse>.Ok(ClienteResponse.DoDominio(atualizado!));
    }

    public async Task<Result> InativarAsync(int id, CancellationToken cancellationToken = default)
    {
        var existente = await _repositorio.ObterPorIdAsync(id, cancellationToken);
        if (existente is null)
        {
            return Result.NaoEncontrado($"Cliente {id} não encontrado.");
        }

        // Idempotente: já inativo → nada a fazer.
        if (!existente.Ativo)
        {
            return Result.Ok();
        }

        // E01 RN06 — nunca exclusão física. E01 RN07 — muda a situação para inativo.
        // E01 RN08 — só a flag muda; orçamentos/contratos/eventos e o endereço
        // continuam intactos (o Repository não apaga nada aqui).
        await _repositorio.DefinirAtivoAsync(id, ativo: false, cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> ReativarAsync(int id, CancellationToken cancellationToken = default)
    {
        var existente = await _repositorio.ObterPorIdAsync(id, cancellationToken);
        if (existente is null)
        {
            return Result.NaoEncontrado($"Cliente {id} não encontrado.");
        }

        if (existente.Ativo)
        {
            return Result.Ok();
        }

        // E01 RN05 — reativar não pode criar um segundo cliente ativo com o
        // mesmo CPF/CNPJ. Revalida antes de ligar a flag.
        var documento = existente.Documento;
        if (documento is not null
            && await ExisteOutroAtivoComDocumentoAsync(existente.Tipo, documento, idExcluir: id, cancellationToken))
        {
            return Result.Conflito(
                $"Não é possível reativar: já existe um cliente ativo com este {(existente.Tipo == TipoCliente.Pf ? "CPF" : "CNPJ")}.",
                "E01_RN05");
        }

        await _repositorio.DefinirAtivoAsync(id, ativo: true, cancellationToken);
        return Result.Ok();
    }

    // --- Helpers ---

    private Task<bool> ExisteOutroAtivoComDocumentoAsync(TipoCliente tipo, string documento, int? idExcluir, CancellationToken cancellationToken)
        => tipo == TipoCliente.Pf
            ? _repositorio.ExisteAtivoComCpfAsync(documento, idExcluir, cancellationToken)
            : _repositorio.ExisteAtivoComCnpjAsync(documento, idExcluir, cancellationToken);

    /// <summary>
    /// E01 RN03/RN04 — valida o documento quando informado. CPF = 11 dígitos,
    /// CNPJ = 14. Decisão: <b>não</b> conferimos o dígito verificador — o ERS
    /// não exige, e uma checagem de DV incompleta daria falsa segurança. Se um
    /// dia for exigido, o ponto de extensão é aqui.
    /// </summary>
    private static string? ValidarDocumento(TipoCliente tipo, string? documentoSoDigitos)
    {
        if (documentoSoDigitos is null)
        {
            return null; // opcional — ausência é válida
        }

        return tipo switch
        {
            TipoCliente.Pf when documentoSoDigitos.Length != 11 => "CPF deve ter 11 dígitos.",
            TipoCliente.Pj when documentoSoDigitos.Length != 14 => "CNPJ deve ter 14 dígitos.",
            _ => null,
        };
    }

    /// <summary>Mantém só os dígitos; devolve <c>null</c> se não sobrar nenhum.</summary>
    private static string? SomenteDigitos(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        var digitos = new string(valor.Where(char.IsDigit).ToArray());
        return digitos.Length == 0 ? null : digitos;
    }

    /// <summary>Trim; devolve <c>null</c> para string vazia (não gravar "" no banco).</summary>
    private static string? Normalizar(string? valor)
        => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    /// <summary>
    /// Converte o bloco de endereço do request em entidade, ou <c>null</c>
    /// quando nenhum campo foi preenchido (cliente sem endereço).
    /// </summary>
    private static Endereco? ConstruirEndereco(EnderecoRequest? request)
    {
        if (request is null)
        {
            return null;
        }

        var endereco = new Endereco
        {
            Rua = Normalizar(request.Rua),
            Numero = Normalizar(request.Numero),
            Bairro = Normalizar(request.Bairro),
            Cidade = Normalizar(request.Cidade),
            Cep = Normalizar(request.Cep),
        };

        return endereco.Vazio ? null : endereco;
    }
}
