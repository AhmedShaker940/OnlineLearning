using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearning.Entities;
namespace OnlineLearning.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> b)
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Name).IsRequired().HasMaxLength(100);
            b.Property(c => c.Description).HasMaxLength(500);

            b.HasOne(c => c.ParentCategory)
             .WithMany(c => c.SubCategories)
             .HasForeignKey(c => c.ParentCategoryId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(c => c.Courses)
             .WithOne(c => c.Category)
             .HasForeignKey(c => c.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
            b.HasData(
                        new Category { Id = 1, Name = "Programming", Description = "All programming courses", ParentCategoryId = null },
                        new Category { Id = 2, Name = "Design", Description = null, ParentCategoryId = null },
                        new Category { Id = 3, Name = "Backend", Description = "Server-side development", ParentCategoryId = 1 },
                        new Category { Id = 4, Name = "Frontend", Description = "Client-side development", ParentCategoryId = 1 },
                        new Category { Id = 5, Name = "ASP.NET", Description = "ASP.NET Core", ParentCategoryId = 3 }
                      );
        }
    }
}
