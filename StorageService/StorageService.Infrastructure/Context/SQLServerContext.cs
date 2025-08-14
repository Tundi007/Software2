using Microsoft.EntityFrameworkCore;
using StorageService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Infrastructure.Context
{
    public class SQLServerContext : DbContext
    {
        public SQLServerContext(DbContextOptions<SQLServerContext> options) : base(options)
        {

        }


        public DbSet<UserStorage> UserStorages { get; set; }
        public DbSet<StorageType> StorageTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=localhost,1433;Database=StorageDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;");
        //}
    }
}
