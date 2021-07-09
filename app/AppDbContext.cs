using System;
using Microsoft.EntityFrameworkCore;

namespace app
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<A> A { get; set; }

        public DbSet<B> B { get; set; }
    }
}
