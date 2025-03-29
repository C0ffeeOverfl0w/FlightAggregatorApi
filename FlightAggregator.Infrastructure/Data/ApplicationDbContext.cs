namespace FlightAggregator.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Booking> Bookings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.PassengerName).IsRequired().HasMaxLength(100);
            entity.Property(b => b.PassengerEmail).IsRequired();
            entity.Property(b => b.BookingDate).IsRequired();
        });
    }
}