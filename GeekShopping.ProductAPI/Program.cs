using AutoMapper;
using GeekShopping.ProductAPI.Config;
using GeekShopping.ProductAPI.Model.Context;
using GeekShopping.ProductAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

#region old
//// Add services to the container.
//var host = Host.CreateDefaultBuilder(args)
//            .ConfigureAppConfiguration((hostContext, config) =>
//            {
//                // Carregar configurações do appsettings.json
//                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

//                // Adicionar outras configurações conforme necessário
//                // Exemplo: config.AddEnvironmentVariables();
//            })
//            .Build();

//var configuration = host.Services.GetRequiredService<IConfiguration>();

//// Agora você pode acessar as configurações diretamente
//var connection = configuration.GetConnectionString("Connection");
#endregion

var connection = builder.Configuration["ConnectionStrings:Connection"];
builder.Services.AddDbContext<SqlContext>(options => options.UseSqlServer(connection));

IMapper mapper = MappingConfig.RegisterMap().CreateMapper();

builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GeekShopping", Description = "Tudo sobre seus produtos" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseAuthorization();

app.MapControllers();

app.Run();
