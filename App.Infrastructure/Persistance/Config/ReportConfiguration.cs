using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistance.Config
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(b => b.Id).ValueGeneratedOnAdd();

            builder.Property(b => b.Status).HasConversion<string>();

            builder.Property(b => b.Reason).HasConversion<string>();

            builder.HasOne(r => r.Blog)
                .WithMany(b => b.Reports)
                .HasForeignKey(r => r.BlogId);

            builder.HasOne(r => r.Reporter)
                .WithMany(b => b.ReporterReports)
                .HasForeignKey(r => r.ReporterId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Reviewer)
                .WithMany(b => b.ReviewerReports)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(r => !r.IsDeleted);

        }
    }
}
