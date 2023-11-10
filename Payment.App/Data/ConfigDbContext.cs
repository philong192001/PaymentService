namespace Payment.App.Data;

public class ConfigDbContext : DbContext
{
    public DbSet<PaymentConfig> PaymentConfigs { get; set; }

    public ConfigDbContext(DbContextOptions<ConfigDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PaymentConfigEntitiyTypeConfiguration());
    }
}