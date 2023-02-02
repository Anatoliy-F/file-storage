using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccessLayer.Data
{
    /// <summary>
    /// Factory to interact with cli ef tools
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        /// <summary>
        /// Create Db context to interact witn dotnet cli ef tools
        /// </summary>
        /// <param name="args"></param>
        /// <returns>AppDbContext</returns>
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = @"Server=(localdb)\mssqllocaldb;Database=filestorage;Trusted_Connection=True;";
            optionsBuilder.UseSqlServer(connectionString);
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
