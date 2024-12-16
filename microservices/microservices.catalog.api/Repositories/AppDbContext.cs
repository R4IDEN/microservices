using microservices.catalog.api.Features.Categories;
using microservices.catalog.api.Features.Courses;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using System.Reflection;

namespace microservices.catalog.api.Repositories
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Category> Categories { get; set; }

        public static AppDbContext Create(IMongoDatabase _database)
        {
            var opt = new DbContextOptionsBuilder<AppDbContext>()
                .UseMongoDB(_database.Client, _database.DatabaseNamespace.DatabaseName);

            return new AppDbContext(opt.Options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //collection(tablo)/Document(satır)/Field(sütun)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
