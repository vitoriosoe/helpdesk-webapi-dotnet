using Microsoft.EntityFrameworkCore;
using HelpDeskApi.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("A string de conex�o DefaultConnection n�o foi encontrada no appsettings.json.");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 30));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion,
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    ));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ativar o Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Ativar o CORS
app.UseCors("PermitirTudo");

// Bloco único para validação da base de dados
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
        Console.WriteLine("--> Conexão com o TiDB Cloud estabelecida e tabelas validadas!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Erro ao conectar no TiDB Cloud: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"--> Detalhes do Erro: {ex.InnerException.Message}");
        }
    }
}

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
app.Run();
