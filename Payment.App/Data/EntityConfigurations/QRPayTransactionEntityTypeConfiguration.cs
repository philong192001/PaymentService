namespace Payment.App.Data.EntityConfigurations;

public class QRPayTransactionEntityTypeConfiguration : IEntityTypeConfiguration<QRPayTransaction>
{
    public void Configure(EntityTypeBuilder<QRPayTransaction> builder)
    {
        builder.ToTable("QRPay.QRPayTransaction");
        builder.HasKey(p => p.TransId);
        builder.Property(p => p.TransId).HasColumnType("INT");
        builder.Property(p => p.BookId).HasColumnType("uniqueidentifier");
        builder.Property(p => p.PartnerTransId).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.VehiclePlate).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.PrivateCode).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.Amount).HasColumnType("FLOAT");
        builder.Property(p => p.Price).HasColumnType("FLOAT");
        builder.Property(p => p.GroupName).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.OperatorPriceName).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.CompanyId).HasColumnType("INT");
        builder.Property(p => p.PaymentType).HasColumnType("INT");
        builder.Property(p => p.Token).HasColumnType("NVARCHAR(500)");
        builder.Property(p => p.DateCreateQR).HasColumnType("DATETIME");
        builder.Property(p => p.DatePay).HasColumnType("DATETIME");
        builder.Property(p => p.Mobile).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.FeeMonth).HasColumnType("DATETIME");
        builder.Property(p => p.DriverName).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.DriverCode).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.CustomerName).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.QRCode).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.ExpiredFee).HasColumnType("DATETIME");
    }
}
