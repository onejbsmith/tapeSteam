using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace tapeStream.Server.Models
{
    public partial class tapeStreamDb_devContext : DbContext
    {
        public tapeStreamDb_devContext()
        {
        }

        public tapeStreamDb_devContext(DbContextOptions<tapeStreamDb_devContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Asks> Asks { get; set; }
        public virtual DbSet<Bids> Bids { get; set; }
        public virtual DbSet<Buys> Buys { get; set; }
        public virtual DbSet<Marks> Marks { get; set; }
        public virtual DbSet<Sells> Sells { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=Machine;Initial Catalog=tapeStreamDb_dev;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asks>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("money");

                entity.Property(e => e.RunDateTime)
                    .HasColumnName("runDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnName("symbol")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Bids>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("money");

                entity.Property(e => e.RunDateTime)
                    .HasColumnName("runDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnName("symbol")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Buys>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("money");

                entity.Property(e => e.PriceLevel).HasColumnName("priceLevel");

                entity.Property(e => e.RunDateTime)
                    .HasColumnName("runDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnName("symbol")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Marks>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("money");

                entity.Property(e => e.RunDateTime)
                    .HasColumnName("runDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnName("symbol")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sells>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("money");

                entity.Property(e => e.PriceLevel).HasColumnName("priceLevel");

                entity.Property(e => e.RunDateTime)
                    .HasColumnName("runDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnName("symbol")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
