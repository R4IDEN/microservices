using microservices.catalog.api.Options;
using microservices.catalog.api.Repositories;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//option'lar için bir extension oluşturuldu.
builder.Services.AddOptionsExt();

//mongo db configuration
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var options = sp.GetRequiredService<MongoOption>();
    return new MongoClient(options.ConnectionString);
});

//mongo db'ye her bağlanılmak istendiğinde yeni bir appDb gönderilecek.
builder.Services.AddScoped<AppDbContext>(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var options = sp.GetRequiredService<MongoOption>();

    return AppDbContext.Create(mongoClient.GetDatabase(options.DatabaseName));
});







var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();

