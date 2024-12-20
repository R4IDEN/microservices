using microservices.catalog.api;
using microservices.catalog.api.Features.Categories;
using microservices.catalog.api.Features.Courses;
using microservices.catalog.api.Options;
using microservices.catalog.api.Repositories;
using microservices.shared.Extensions;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

//add services to the container.
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

//common service extension
builder.Services.AddCommonServiceExt(typeof(CatalogAssembly));





//app Create
var app = builder.Build();

//category endpoint
app.AddCategoryGroupEndpointExt();
//course endpoint
app.AddCourseGroupEndpointExt();


//configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();

