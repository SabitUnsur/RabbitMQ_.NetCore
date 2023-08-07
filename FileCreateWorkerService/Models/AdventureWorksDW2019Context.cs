using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace FileCreateWorkerService.Models
{
    public partial class AdventureWorksDW2019Context : DbContext
    {
        public AdventureWorksDW2019Context()
        {
        }

        public AdventureWorksDW2019Context(DbContextOptions<AdventureWorksDW2019Context> options)
            : base(options)
        {
        }

        public virtual DbSet<DimProduct> DimProducts { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=AuthDb;Username=postgres;Password=123");
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<DimProduct>(entity =>
            {
                entity.HasKey(e => e.ProductKey)
                    .HasName("PK_DimProduct_ProductKey");

                entity.ToTable("DimProduct");

                entity.HasIndex(e => new { e.ProductAlternateKey, e.StartDate }, "AK_DimProduct_ProductAlternateKey_StartDate")
                    .IsUnique();

                entity.Property(e => e.ArabicDescription).HasMaxLength(400);

                entity.Property(e => e.ChineseDescription).HasMaxLength(400);

                entity.Property(e => e.Class)
                    .HasMaxLength(2)
                    .IsFixedLength();

                entity.Property(e => e.Color).HasMaxLength(15);

                entity.Property(e => e.DealerPrice).HasColumnType("money");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.EnglishDescription).HasMaxLength(400);

                entity.Property(e => e.EnglishProductName).HasMaxLength(50);

                entity.Property(e => e.FrenchDescription).HasMaxLength(400);

                entity.Property(e => e.FrenchProductName).HasMaxLength(50);

                entity.Property(e => e.GermanDescription).HasMaxLength(400);

                entity.Property(e => e.HebrewDescription).HasMaxLength(400);

                entity.Property(e => e.JapaneseDescription).HasMaxLength(400);

                entity.Property(e => e.ListPrice).HasColumnType("money");

                entity.Property(e => e.ModelName).HasMaxLength(50);

                entity.Property(e => e.ProductAlternateKey).HasMaxLength(25);

                entity.Property(e => e.ProductLine)
                    .HasMaxLength(2)
                    .IsFixedLength();

                entity.Property(e => e.Size).HasMaxLength(50);

                entity.Property(e => e.SizeRange).HasMaxLength(50);

                entity.Property(e => e.SizeUnitMeasureCode)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.SpanishProductName).HasMaxLength(50);

                entity.Property(e => e.StandardCost).HasColumnType("money");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(7);

                entity.Property(e => e.Style)
                    .HasMaxLength(2)
                    .IsFixedLength();

                entity.Property(e => e.ThaiDescription).HasMaxLength(400);

                entity.Property(e => e.TurkishDescription).HasMaxLength(400);

                entity.Property(e => e.WeightUnitMeasureCode)
                    .HasMaxLength(3)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
