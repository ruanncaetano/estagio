using System.Reflection;
using Serilog;
using tcc.Middleware;

// Logger de bootstrap: registra falhas que acontecem já na subida da aplicação,
// antes do container de DI existir. É substituído pelo logger definitivo abaixo.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/fogo-erp-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

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

    // --- Swagger / OpenAPI ---
    // Documentação da API é obrigatória (ver backend/CLAUDE.md). A UI de teste
    // fica em /doc.
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new()
        {
            Title = "Fogo de Chão ERP — API",
            Version = "v1",
            Description = "API do ERP do Fogo de Chão Buffet Experience.",
        });

        // Puxa os comentários /// dos controllers/DTOs para a documentação.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        }
    });

    // --- Registro de DI por módulo (Services / Repositories) ---
    // Um bloco por módulo conforme as estórias forem implementadas, ex:
    //   builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
    //   builder.Services.AddScoped<IClienteService, ClienteService>();
    // Ver backend/CLAUDE.md ("Estrutura de camadas").

    var app = builder.Build();

    // Log de erros central: primeira coisa no pipeline para capturar tudo.
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Loga uma linha resumida por request (método, rota, status, duração).
    app.UseSerilogRequestLogging();

    // Swagger sempre ligado — UI de teste em /doc, JSON em /swagger/v1/swagger.json.
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fogo de Chão ERP — API v1");
        options.RoutePrefix = "doc";
        options.DocumentTitle = "Fogo de Chão ERP — API";
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
