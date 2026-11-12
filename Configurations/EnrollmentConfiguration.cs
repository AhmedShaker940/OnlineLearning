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
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> b)
        {
            b.HasKey(e => new { e.StudentId, e.CourseId });   

            b.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            b.Property(e => e.FinalGrade).HasPrecision(5, 2);
            b.ToTable(t => t.HasCheckConstraint("CK_Enrollment_Progress", "[Progress] >= 0 AND [Progress] <= 100"));

            b.HasOne(e => e.Student)
             .WithMany(s => s.Enrollments)
             .HasForeignKey(e => e.StudentId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(e => e.Course)
             .WithMany(c => c.Enrollments)
             .HasForeignKey(e => e.CourseId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
