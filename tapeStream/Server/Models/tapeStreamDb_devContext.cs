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
        public virtual DbSet<Runs> Runs { get; set; }
        public virtual DbSet<Sells> Sells { get; set; }
        public virtual DbSet<Streamed> Streamed { get; set; }

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

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.StreamId).HasColumnName("streamId");

                entity.HasOne(d => d.Stream)
                    .WithMany(p => p.Asks)
                    .HasForeignKey(d => d.StreamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Asks_Streamed");
            });

            modelBuilder.Entity<Bids>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("money");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.StreamId).HasColumnName("streamId");

                entity.HasOne(d => d.Stream)
                    .WithMany(p => p.Bids)
                    .HasForeignKey(d => d.StreamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bids_Streamed");
            });

            modelBuilder.Entity<Buys>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("money");

                entity.Property(e => e.Size).HasColumnName("size");
                entity.Property(e => e.PriceLevel).HasColumnName("priceLevel");

                entity.Property(e => e.StreamId).HasColumnName("streamId");

                entity.HasOne(d => d.Stream)
                    .WithMany(p => p.Buys)
                    .HasForeignKey(d => d.StreamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Buys_Streamed");
            });

            modelBuilder.Entity<Marks>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("money");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.StreamId).HasColumnName("streamId");

                entity.HasOne(d => d.Stream)
                    .WithMany(p => p.Marks)
                    .HasForeignKey(d => d.StreamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Marks_Streamed");
            });

            modelBuilder.Entity<Runs>(entity =>
            {
                entity.HasKey(e => e.RunId);

                entity.Property(e => e.RunId).HasColumnName("runId");

                entity.Property(e => e.RunDate)
                    .HasColumnName("runDate")
                    .HasColumnType("date");

                entity.Property(e => e.Symbol)
                    .HasColumnName("symbol")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sells>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("money");

                entity.Property(e => e.Size).HasColumnName("size");
                entity.Property(e => e.PriceLevel).HasColumnName("priceLevel");

                entity.Property(e => e.StreamId).HasColumnName("streamId");

                entity.HasOne(d => d.Stream)
                    .WithMany(p => p.Sells)
                    .HasForeignKey(d => d.StreamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sells_Streamed");
            });

            modelBuilder.Entity<Streamed>(entity =>
            {
                entity.HasKey(e => e.StreamId);

                entity.Property(e => e.StreamId).HasColumnName("streamId");

                entity.Property(e => e.RunId).HasColumnName("runId");

                entity.Property(e => e.StreamTime)
                    .HasColumnName("streamTime")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Run)
                    .WithMany(p => p.Streamed)
                    .HasForeignKey(d => d.RunId)
                    .HasConstraintName("FK_Streamed_Runs");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
