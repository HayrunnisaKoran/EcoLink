using Microsoft.EntityFrameworkCore;
using web_backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

using System.Collections.Generic;

namespace web_backend
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<WasteBin> WasteBins { get; set; }
        public DbSet<WasteRecord> WasteRecords { get; set; }
        public DbSet<WasteType> WasteTypes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<PointsTransaction> PointsTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // WasteBin Tablosu Hassasiyet Ayarları
            modelBuilder.Entity<WasteBin>(entity =>
            {
                entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
                entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
                entity.Property(e => e.CapacityLiter).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.FillLevelPercent).HasColumnType("decimal(5, 2)");
            });

            // WasteRecord Tablosu Hassasiyet Ayarları
            modelBuilder.Entity<WasteRecord>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            });

            // Düğümü Çözen Kısım: WasteRecords ve WasteTypes ilişkisi[cite: 2]
            modelBuilder.Entity<WasteRecord>()
                .HasOne(wr => wr.WasteType)
                .WithMany()
                .HasForeignKey(wr => wr.WasteTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Cascade yerine Restrict yaparak yolu temizledik[cite: 2]

            // Eğer hata devam ederse WasteBins yolu için de aynısını yapabilirsin:[cite: 2]
            modelBuilder.Entity<WasteRecord>()
                .HasOne(wr => wr.WasteBin)
                .WithMany()
                .HasForeignKey(wr => wr.WasteBinId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<WasteRecord>(entity =>
            {
                entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
                entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            });
        }

    }
}


