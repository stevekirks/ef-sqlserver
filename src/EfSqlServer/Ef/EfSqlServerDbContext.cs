using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace EfSqlServer.Ef
{
    public partial class EfSqlServerDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public EfSqlServerDbContext()
        {
        }

        public EfSqlServerDbContext(DbContextOptions<EfSqlServerDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Food> Foods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Food>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
