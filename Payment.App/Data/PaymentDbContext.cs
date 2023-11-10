namespace Payment.App.Data;

public class PaymentDbContext : DbContext
{
    public DbSet<QRPayTerminal> QRPayTerminals { get; set; }
    public DbSet<QRPayTransaction> QRPayTransactions { get; set; }
    public DbSet<VnPayTrans> VnPayTrans { get; set; }

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new QRPayTerminalEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new QRPayTransactionEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VnPayTransEntityTypeConfiguration());
    }
}