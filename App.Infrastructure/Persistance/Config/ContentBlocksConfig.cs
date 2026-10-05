using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistance.Config
{
    public class ContentBlocksConfig : IEntityTypeConfiguration<ContentBlock>
    {
        public void Configure(EntityTypeBuilder<ContentBlock> builder)
        {
            builder.HasKey(cb => cb.Id);
            builder.Property(cb => cb.Id).ValueGeneratedOnAdd();

            builder.Property(cb => cb.Type).HasConversion<string>();

            builder.HasOne(cb => cb.Blog)
                .WithMany(b => b.ContentBlocks)
                .HasForeignKey(cb => cb.BlogId)
                .IsRequired();

            builder.HasIndex(cb => new
            {
                cb.BlogId,
                cb.Order
            }).IsUnique();

            builder.HasQueryFilter(cb => !cb.IsDeleted);

        }
    }
}
