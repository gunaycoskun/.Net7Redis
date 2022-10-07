using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NET7Redis.Cache;
using NET7RedisAPI.Model;
using NET7RedisAPI.Repository;
using NET7RedisAPI.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseInMemoryDatabase("myDatabase");
});
//Decorated Pattern Öncesi
//builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductRepository>(sp =>
{
    var appDbcontext= sp.GetRequiredService<AppDbContext>();
    var productRepository=new ProductRepository(appDbcontext);

    var redisService = sp.GetRequiredService<RedisService>();
    return new ProductRepositoryWithCache(productRepository, redisService, Convert.ToInt16(builder.Configuration["CacheOption:DbIndex"]));
});

builder.Services.AddSingleton<RedisService>(opt =>
{
    return new RedisService(builder.Configuration["CacheOption:ConnectionString"], builder.Configuration["CacheOption:Password"]);
});
builder.Services.AddScoped<StackExchange.Redis.IDatabase>(opt =>
{
    var redisService=opt.GetRequiredService<RedisService>();
    return redisService.GetDatabase(Convert.ToInt16(builder.Configuration["CacheOption:DbIndex"]));
});
builder.Services.AddScoped<IProductService, ProductService>();
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
