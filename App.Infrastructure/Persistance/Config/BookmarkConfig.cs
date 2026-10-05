using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistance.Config
{
    public class BookmarkConfig : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedOnAdd();

            builder.HasOne(b => b.User)
                .WithMany(b => b.Bookmarks)
                .HasForeignKey(b => b.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Blog)
                .WithMany(b => b.Bookmarks)
                .HasForeignKey(b => b.BlogId)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.UserId,
                x.BlogId
            })
            .IsUnique();

            builder.HasQueryFilter(b => !b.IsDeleted);
        }

    }
}
