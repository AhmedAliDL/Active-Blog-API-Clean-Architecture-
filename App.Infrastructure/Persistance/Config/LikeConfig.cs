using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistance.Config
{
    public class LikeConfig : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedOnAdd();

            builder.HasOne(b => b.User)
             .WithMany(u => u.UserLikes)
             .HasForeignKey(b => b.UserId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Blog)
               .WithMany(u => u.BlogLikes)
               .HasForeignKey(b => b.BlogId)
               .IsRequired();

            builder.HasIndex(l => new
            {
                l.UserId,
                l.BlogId
            })
            .IsUnique();

            builder.HasQueryFilter(l => !l.IsDeleted);
        }
    }
}
