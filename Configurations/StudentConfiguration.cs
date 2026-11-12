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
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> b)
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.FullName).IsRequired().HasMaxLength(100);
            b.Property(s => s.Email).IsRequired().HasMaxLength(150);
            b.Property(s => s.DateOfBirth).IsRequired();
        }
    }
}
