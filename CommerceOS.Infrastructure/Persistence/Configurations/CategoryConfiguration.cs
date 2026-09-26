using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOS.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration
    : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(180);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        // A slug must uniquely identify a category.
        builder.HasIndex(x => x.Slug)
            .IsUnique();

        // Product ↔ Category many-to-many relationship.
        builder.HasMany(x => x.Products)
            .WithMany(x => x.Categories)
            .UsingEntity<Dictionary<string, object>>(
                "ProductCategories",
                right => right
                    .HasOne<Product>()
                    .WithMany()
                    .HasForeignKey("ProductId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left
                    .HasOne<Category>()
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("ProductId", "CategoryId");

                    join.ToTable("ProductCategories");

                    join.HasIndex("CategoryId");
                });
    }
}