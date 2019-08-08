using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using AutoMapper;
using BlogPlatform.Models.EF_Model_classes;
using System.Reflection;

namespace BlogPlatformTest.Helpers
{
    // taken from
    // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests
    // ?view=aspnetcore-2.2#basic-tests-with-the-default-webapplicationfactory
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (ApplicationDbContext) using an in-memory 
                // database for testing.
                services.AddDbContext<BlogsDataBaseContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                services.AddAutoMapper(TestMapper.Configure, new Assembly[0]);

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<BlogsDataBaseContext>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();
                    DbCreator.AddItems(db);
                }
            });
        }
    }

}
