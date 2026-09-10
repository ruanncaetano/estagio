using Scalar.AspNetCore;
using Serilog;
using tcc.Data;
using tcc.Middleware;
using tcc.Repositories;
using tcc.Services;

// Logger de bootstrap: registra falhas que acontecem já na subida da aplicação,
// antes do container de DI existir. É substituído pelo logger definitivo abaixo.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/fogo-erp-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configuração local com segredos (connection string do MySQL). Fora do git
    // — ver appsettings.Local.example.json. Opcional para não quebrar CI.
    builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

    // --- Log (Serilog) ---
    // Console + arquivo rotativo diário em logs/. Nível vem de appsettings.json
    // (seção "Serilog"). Erros não tratados são registrados pelo
    // ExceptionHandlingMiddleware.
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/fogo-erp-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14));

    // --- Controllers (camada de entrada HTTP) ---
    builder.Services.AddControllers();

    // --- OpenAPI ---
    // Documentação da API é obrigatória (ver backend/CLAUDE.md). O documento
    // OpenAPI é gerado por Microsoft.AspNetCore.OpenApi (os comentários /// dos
    // controllers/DTOs entram via source generator, com GenerateDocumentationFile).
    // A UI de teste (Scalar) fica em /doc.
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info.Title = "Fogo de Chão ERP — API";
            document.Info.Version = "v1";
            document.Info.Description = "API do ERP do Fogo de Chão Buffet Experience.";
            return Task.CompletedTask;
        });
    });

    // --- Registro de DI por módulo (Services / Repositories) ---
    // Um bloco por módulo conforme as estórias forem implementadas.
    // Ver backend/CLAUDE.md ("Estrutura de camadas").

    // Infra de dados (compartilhada por todos os módulos).
    builder.Services.AddScoped<IDbConnectionFactory, MySqlConnectionFactory>();

    // Estória 01 — Clientes.
    builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
    builder.Services.AddScoped<IClienteService, ClienteService>();

    var app = builder.Build();

    // Log de erros central: primeira coisa no pipeline para capturar tudo.
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Loga uma linha resumida por request (método, rota, status, duração).
    app.UseSerilogRequestLogging();

    // Documentação sempre ligada:
    //   JSON OpenAPI  -> /openapi/v1.json
    //   UI de teste   -> /doc  (Scalar, com "try it out")
    app.MapOpenApi();
    app.MapScalarApiReference("/doc", options =>
    {
        options.WithTitle("Fogo de Chão ERP — API");
    });

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao subir");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
