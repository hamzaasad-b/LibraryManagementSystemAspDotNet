using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Data.Entities.Map;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class LmsDbContext : IdentityDbContext<User, IdentityRole<uint>, uint>
    {
        private DbSet<Book>? Books { get; set; }

        public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new UserMap().Configure(modelBuilder.Entity<User>());
            new BookMap().Configure(modelBuilder.Entity<Book>());
        }
    }
}