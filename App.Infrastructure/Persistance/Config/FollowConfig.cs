using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistance.Config
{
    public class FollowConfig : IEntityTypeConfiguration<Follow>
    {
        public void Configure(EntityTypeBuilder<Follow> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedOnAdd();

            builder
                 .HasOne(x => x.Blogger)
                 .WithMany(x => x.Followers)
                 .HasForeignKey(x => x.BloggerId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Follower)
                .WithMany(x => x.Blogger)
                .HasForeignKey(x => x.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(x => new
            {
                x.FollowerId,
                x.BloggerId
            })
            .IsUnique();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }

    }
}
