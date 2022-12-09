using DataAccessLayer.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace FileStorage.Tests.ApiTests
{

    internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                RemoveLibraryDbContextRegistration(services);

                var serviceProvider = GetInMemoryServiceProvider();

                services.AddDbContextPool<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(Guid.Empty.ToString());
                    options.UseInternalServiceProvider(serviceProvider);
                });

                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    TestHelper.SeedData(context);
                }
            });
        }

        private static ServiceProvider GetInMemoryServiceProvider()
        {
            return new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
        }

        private static void RemoveLibraryDbContextRegistration(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
        }

    }
}