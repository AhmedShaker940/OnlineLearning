using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineLearning.Enums;
namespace OnlineLearning.Entities
{
    public class Instructor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public DateTime JoinedAt { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
