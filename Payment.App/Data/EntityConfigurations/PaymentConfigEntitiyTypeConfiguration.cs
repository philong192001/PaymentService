namespace Payment.App.Data.EntityConfigurations;

public class PaymentConfigEntitiyTypeConfiguration : IEntityTypeConfiguration<PaymentConfig>
{
    public void Configure(EntityTypeBuilder<PaymentConfig> builder)
    {
        builder.ToTable("Payment.Config");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnType("BIGINT");
        builder.Property(p => p.CompanyId).HasColumnType("INT");
        builder.Property(p => p.UrlGetToken).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.PaymentType).HasColumnType("NCHAR(10)");
        builder.Property(p => p.UrlCreateQR).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.UserName).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.Password).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.PathImage).HasColumnType("NVARCHAR(150)");
        builder.Property(p => p.HashCode).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.AccessCode).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.Merchant).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.UrlDriverCompany).HasColumnType("NVARCHAR(150)");
    }
}