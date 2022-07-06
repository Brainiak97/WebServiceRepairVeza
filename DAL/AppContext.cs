using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Comment>? Comments { get; set; }
        public DbSet<RepairLog>? RepairLogs { get; set; }
        public DbSet<RepairGroup>? RepairGroups { get; set; }
        public DbSet<Notification>? Notifications { get; set; }

        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
            //Database.EnsureDeleted(); //need
            if (!Database.CanConnect() && !Database.EnsureCreated())
            {
                throw new InvalidOperationException("Database was not created properly.");
            }
        }
    }
}
