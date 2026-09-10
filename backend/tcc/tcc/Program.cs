var builder = WebApplication.CreateBuilder(args);

// --- Controllers (camada de entrada HTTP) ---
builder.Services.AddControllers();

// --- Registro de DI por módulo (Services / Repositories) ---
// Um bloco por módulo conforme as estórias forem implementadas, ex:
//   builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
//   builder.Services.AddScoped<IClienteService, ClienteService>();
// Ver backend/CLAUDE.md ("Estrutura de camadas").

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
