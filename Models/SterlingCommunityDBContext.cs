using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SterlingCommunityAPI.Models
{
    public partial class SterlingCommunityDBContext : DbContext
    {
        public SterlingCommunityDBContext()
        {
        }

        public SterlingCommunityDBContext(DbContextOptions<SterlingCommunityDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Session> Session { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=SterlingCommunityDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>(entity =>
            {
                entity.Property(e => e.Bvn)
                    .HasColumnName("BVN")
                    .HasMaxLength(50);

                entity.Property(e => e.DateInserted).HasColumnType("datetime");
                entity.Property(e => e.DateSessionKeyInsertedByUser).HasColumnType("datetime");

                entity.Property(e => e.DateIsActiveChanged).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(50);
                entity.Property(e => e.AccountNumber).HasMaxLength(50);

                entity.Property(e => e.MobileNumber).HasMaxLength(50);

                entity.Property(e => e.SessionKey).IsRequired();
                entity.Property(e => e.SessionKeyInsertedByUser);


                entity.Property(e => e.UserName)
                    
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
