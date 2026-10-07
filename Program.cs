using Microsoft.EntityFrameworkCore;
using HelpDeskApi.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("A string de conexão DefaultConnection não foi encontrada no appsettings.json.");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 30));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
