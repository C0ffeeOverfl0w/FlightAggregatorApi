using FlightAggregator.Domain.Entities;
using FlightAggregator.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FlightAggregator.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Airline> Airlines { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureAirline(modelBuilder);
        ConfigureFlight(modelBuilder);
        ConfigureBooking(modelBuilder);
    }

    private static void ConfigureAirline(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airline>(a =>
        {
            a.HasKey(x => x.Id);
            a.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
        });
    }

    private static void ConfigureFlight(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flight>(f =>
        {
            f.HasKey(x => x.Id);

            f.ComplexProperty(x => x.Price, p =>
            {
                p.Property(m => m.Amount)
                    .HasPrecision(18, 2);
                p.Property(m => m.Currency)
                    .HasMaxLength(3);
            });

            f.Property(x => x.FlightNumber)
                .IsRequired()
                .HasMaxLength(20);
        });
    }

    private static void ConfigureBooking(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.PassengerName)
                .IsRequired()
                .HasMaxLength(100);

            b.Property(x => x.PassengerEmail)
                .IsRequired()
                .HasMaxLength(255);

            b.Property(x => x.BookingDate)
                .IsRequired();

            b.Property(x => x.FlightNumber)
                .IsRequired()
                .HasMaxLength(20);

            b.HasIndex(x => x.FlightNumber);
            b.HasIndex(x => x.PassengerEmail);
            b.HasIndex(x => x.BookingDate);
        });
    }
}