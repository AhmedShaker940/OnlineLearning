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
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> b)
        {
            b.HasKey(i => i.Id);
            b.Property(i => i.FullName).IsRequired().HasMaxLength(100);
            b.Property(i => i.Email).IsRequired().HasMaxLength(150);
            b.Property(i => i.Bio).HasMaxLength(500);
            b.Property(i => i.JoinedAt).IsRequired();

            b.HasMany(i => i.Courses)
             .WithOne(c => c.Instructor)
             .HasForeignKey(c => c.InstructorId)
             .OnDelete(DeleteBehavior.Restrict);
            b.HasData(
                new Instructor { Id = 1, FullName = "Ahmed Hassan", Email = "ahmed@route.com", Bio = "Senior .NET developer", JoinedAt = new DateTime(2022, 1, 10) },
                new Instructor { Id = 2, FullName = "Sara Ali", Email = "sara@route.com", Bio = null, JoinedAt = new DateTime(2022, 6, 15) },
                new Instructor { Id = 3, FullName = "Omar Khaled", Email = "omar@route.com", Bio = "Architect and UI designer", JoinedAt = new DateTime(2023, 3, 1) }
                      );
        }

    }

}

