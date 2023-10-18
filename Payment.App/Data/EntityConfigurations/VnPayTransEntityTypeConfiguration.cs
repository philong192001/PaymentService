namespace Payment.App.Data.EntityConfigurations;

public class VnPayTransEntityTypeConfiguration : IEntityTypeConfiguration<VnPayTrans>
{
    public void Configure(EntityTypeBuilder<VnPayTrans> builder)
    {
        builder.ToTable("VNPayTrans");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnType("INT");
        builder.Property(p => p.TxnId).HasColumnType("NVARCHAR(100)");
        builder.Property(p => p.State).HasColumnType("INT");
        builder.Property(p => p.Amount).HasColumnType("NVARCHAR(100)");
        builder.Property(p => p.TerminalId).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.CreatedDate).HasColumnType("DATETIME");
        builder.Property(p => p.ErrorCode).HasColumnType("INT");
        builder.Property(p => p.Message).HasColumnType("NVARCHAR(100)");
    }
}
