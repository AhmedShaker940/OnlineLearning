using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearning.Entities;
using OnlineLearning.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OnlineLearning.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> b)
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Title).IsRequired().HasMaxLength(200);
            b.Property(c => c.Description).HasMaxLength(1000);
            b.Property(c => c.Price).HasPrecision(10, 2);
            b.Property(c => c.Level).HasConversion<string>().HasMaxLength(20);
            b.Property(c => c.IsPublished).HasDefaultValue(false);
            b.HasData(
                        new Course { Id = 1, Title = "C# Fundamentals", Price = 499.99m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2023, 1, 1), InstructorId = 1, CategoryId = 1 },
                        new Course { Id = 2, Title = "ASP.NET Core Web API", Price = 899.99m, Level = CourseLevel.Intermediate, IsPublished = true, PublishedDate = new DateTime(2023, 2, 1), InstructorId = 1, CategoryId = 5 },
                        new Course { Id = 3, Title = "Advanced EF Core", Price = 1200m, Level = CourseLevel.Advanced, IsPublished = true, PublishedDate = new DateTime(2023, 3, 1), InstructorId = 2, CategoryId = 3 },
                        new Course { Id = 4, Title = "HTML and CSS Basics", Price = 300m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2023, 4, 1), InstructorId = 2, CategoryId = 4 },
                        new Course { Id = 5, Title = "Microservices Architecture", Price = 1500m, Level = CourseLevel.Advanced, IsPublished = false, PublishedDate = null, InstructorId = 3, CategoryId = 3 },
                        new Course { Id = 6, Title = "UI/UX Design Principles", Price = 650m, Level = CourseLevel.Intermediate, IsPublished = false, PublishedDate = null, InstructorId = 3, CategoryId = 2 }
                      );
        }
    }
}
