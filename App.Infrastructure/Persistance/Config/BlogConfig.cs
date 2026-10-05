using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistance.Config
{
    public class BlogConfig : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedOnAdd();

            builder.HasOne(b => b.User)
               .WithMany(u => u.UserBlogs)
               .HasForeignKey(b => b.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Category)
               .WithMany(u => u.Blogs)
               .HasForeignKey(b => b.CategoryId)
               .IsRequired();

            builder.HasQueryFilter(b => !b.IsDeleted);

        }
    }
}
