namespace Payment.App.Data.EntityConfigurations;

public class QRPayTerminalEntityTypeConfiguration : IEntityTypeConfiguration<QRPayTerminal>
{
    public void Configure(EntityTypeBuilder<QRPayTerminal> builder)
    {
        builder.ToTable("QRPay.Terminals");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnType("INT");
        builder.Property(p => p.DriverCode).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.UserName).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.Password).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.MasterMerchant).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.MerchantName).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.MerchantCode).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.MerchantType).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.TerminalName).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.TerminalCode).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.CompanyId).HasColumnType("INT");
        builder.Property(p => p.DriverName).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.TerminalType).HasColumnType("INT");
        builder.Property(p => p.UrlGetToken).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.PaymentType).HasColumnType("NCHAR(10)");
        builder.Property(p => p.UrlCreateQR).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.PathImage).HasColumnType("NVARCHAR(150)");
        builder.Property(p => p.HashCode).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.AccessCode).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.UrlDriverCompany).HasColumnType("NVARCHAR(150)");

    }
}