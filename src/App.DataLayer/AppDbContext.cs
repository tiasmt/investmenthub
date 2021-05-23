using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace App.DataLayer
{
    public class AppDbContext : DbContext, IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Entity1> Entity1 { get; set; }
        public DbSet<Entity2> Entity2 { get; set; }
        public DbSet<Entity3> Entity3 { get; set; }

        // For migrations and design time
        public AppDbContext()
        {
        }

        public AppDbContext CreateDbContext(string[] args = null)
        {
            return new AppDbContext(this.GetOptionBuilder(args).Options);
        }
        // endregion
    }


    public static class DbContextExt
    {
        public static DbContextOptionsBuilder<AppDbContext> GetOptionBuilder(this DbContext context, string[] args = null)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var connStr = "User ID=postgres;Host=127.0.0.1;Port=44410;Database=App.Design";
            if (args != null && args.Length > 0)
                connStr = args[0];
            builder.UseNpgsql(
                connStr, options => options.SetPostgresVersion(new Version(9, 6)));
            return builder;
        }
    }
}
