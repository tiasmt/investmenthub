using App.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.DataLayer
{
    public class InvestmentHubContext : DbContext
    {

        public InvestmentHubContext()
        {
            
        }
        public InvestmentHubContext(DbContextOptions<InvestmentHubContext> options) : base(options)
        {
            
        }

        public DbSet<Event> Events {get;set;}
        public DbSet<SnapshotEvent> Snapshots {get;set;}

    }
}