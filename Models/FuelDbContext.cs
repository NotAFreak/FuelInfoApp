using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FuelInfoApp.Models;

public partial class FuelDbContext : DbContext
{
    public FuelDbContext()
    {
    }

    public FuelDbContext(DbContextOptions<FuelDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<Fuel> Fuels { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<StationFuelChange> StationFuelChanges { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPreference> UserPreferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source =/home/notafreak/Documents/School/Digital Solutions/IA2/FuelInfoApp/fuelDb.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("Brand");

            entity.Property(e => e.BrandId).HasColumnName("BrandID");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.ToTable("Car");

            entity.Property(e => e.CarId).HasColumnName("CarID");
            entity.Property(e => e.FuelConsumption)
                .HasDefaultValue(1.0)
                .HasColumnType("NUMERIC");
            entity.Property(e => e.FuelId).HasColumnName("FuelID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Fuel).WithMany(p => p.Cars)
                .HasForeignKey(d => d.FuelId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.Cars)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Fuel>(entity =>
        {
            entity.ToTable("Fuel");

            entity.Property(e => e.FuelId).HasColumnName("FuelID");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.ToTable("Station");

            entity.Property(e => e.StationId).HasColumnName("StationID");
            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.Latitude).HasColumnType("NUMERIC");
            entity.Property(e => e.Longitude).HasColumnType("NUMERIC");

            entity.HasOne(d => d.Brand).WithMany(p => p.Stations)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StationFuelChange>(entity =>
        {
            entity.HasKey(e => new { e.StationId, e.FuelId });

            entity.ToTable("StationFuelChange");

            entity.Property(e => e.StationId).HasColumnName("StationID");
            entity.Property(e => e.FuelId).HasColumnName("FuelID");
            entity.Property(e => e.IsAvailable).HasDefaultValue(1);

            entity.HasOne(d => d.Fuel).WithMany(p => p.StationFuelChanges)
                .HasForeignKey(d => d.FuelId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Station).WithMany(p => p.StationFuelChanges)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Latitude).HasColumnType("NUMERIC");
            entity.Property(e => e.Longitude).HasColumnType("NUMERIC");
        });

        modelBuilder.Entity<UserPreference>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserPreference");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.DistanceWeight).HasDefaultValue(1);
            entity.Property(e => e.PriceWeight)
                .HasDefaultValueSql("1")
                .HasColumnType("NUMERIC");

            entity.HasOne(d => d.User).WithOne(p => p.UserPreference)
                .HasForeignKey<UserPreference>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
