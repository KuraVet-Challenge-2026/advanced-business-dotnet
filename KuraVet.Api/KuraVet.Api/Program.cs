using KuraVet.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços (Controllers, Swagger, etc)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "KuraVet API",
        Version = "v1",
        Description = "API RESTful para o ecossistema de saúde veterinária contínua KuraVet."
    });

    // Pega o caminho do arquivo XML gerado no Passo 1 e injeta no Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// ====================================================================
// A CONFIGURAÇÃO DO BANCO TEM QUE ENTRAR AQUI, ANTES DO BUILD!
// ====================================================================
builder.Services.AddDbContext<KuraVetDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// ====================================================================
// O BUILD (A LINHA QUE CONSTRÓI O PROJETO)
// ====================================================================
var app = builder.Build();

// Daqui para baixo fica o fluxo de pipeline (app.Use...)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();