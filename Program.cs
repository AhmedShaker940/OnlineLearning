using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data;
using OnlineLearning.Entities;
using OnlineLearning.Enums;

namespace OnlineLearning;

public class Program
{
    public static void Main()
    {
        SeedStudentsAndEnrollments();

        Header("CREATE");
        AddInstructor();
        AddCourse("Docker for Developers", instructorId: 1, categoryId: 3, price: 700m, level: CourseLevel.Intermediate);
        AddCourse("Ghost Course", instructorId: 99, categoryId: 3, price: 100m, level: CourseLevel.Beginner); 
        AddSeveralStudents();
        EnrollStudent(4, 4);    
        EnrollStudent(1, 1);    
        EnrollStudent(1, 5);    
        EnrollStudent(99, 1);   

        Header("READ");
        FindCourse(1, "C# Fundamentals");
        FindCourse(999, "Unknown Course");
        FilterCourses(400m, 1000m);
        ShowCategories(1);
        CourseRoster(1);

        Header("UPDATE");
        ChangePrice(1, 549.99m);
        UpdateProgress(3, 4, 100);  
        UpdateProgress(2, 1, 150);   
        PublishCourse(5);

        Header("DELETE");
        Unenroll(1, 2);
        DeleteCategoryUnsafe(1);     
        DeleteCategorySafe(1);       
        DeleteCategorySafe(2);       
        DeleteCourse(3);

        Header("BULK");
        BulkPriceRaise(CourseLevel.Advanced);
        BulkDeleteDropped();
    }

    static void Header(string t) => Console.WriteLine($"\n===== {t} =====");

 // SEED
    static void SeedStudentsAndEnrollments()
    {
        using var db = new AppDbContext();

        if (!db.Students.Any())
        {
            db.Students.AddRange(
                new Student { FullName = "Mohamed Samir", Email = "mohamed@mail.com", DateOfBirth = new DateTime(2000, 5, 1) },
                new Student { FullName = "Nour Ahmed", Email = "nour@mail.com", DateOfBirth = new DateTime(1999, 8, 20) },
                new Student { FullName = "Youssef Adel", Email = "youssef@mail.com", DateOfBirth = new DateTime(2001, 2, 14) },
                new Student { FullName = "Mona Tarek", Email = "mona@mail.com", DateOfBirth = new DateTime(1998, 11, 3) },
                new Student { FullName = "Karim Hany", Email = "karim@mail.com", DateOfBirth = new DateTime(2002, 7, 9) }
            );
            db.SaveChanges();
            Console.WriteLine("Students seeded.");
        }

        if (!db.Enrollments.Any())
        {
            var d = new DateTime(2024, 1, 1);
            db.Enrollments.AddRange(
                new Enrollment { StudentId = 1, CourseId = 1, EnrolledAt = d, Progress = 100, Status = EnrollmentStatus.Completed, FinalGrade = 90m },
                new Enrollment { StudentId = 1, CourseId = 2, EnrolledAt = d, Progress = 40, Status = EnrollmentStatus.Active },
                new Enrollment { StudentId = 2, CourseId = 1, EnrolledAt = d, Progress = 60, Status = EnrollmentStatus.Active },
                new Enrollment { StudentId = 2, CourseId = 3, EnrolledAt = d, Progress = 20, Status = EnrollmentStatus.Dropped },
                new Enrollment { StudentId = 3, CourseId = 2, EnrolledAt = d, Progress = 100, Status = EnrollmentStatus.Completed, FinalGrade = 85.5m },
                new Enrollment { StudentId = 3, CourseId = 4, EnrolledAt = d, Progress = 10, Status = EnrollmentStatus.Active },
                new Enrollment { StudentId = 4, CourseId = 1, EnrolledAt = d, Progress = 5, Status = EnrollmentStatus.Dropped },
                new Enrollment { StudentId = 5, CourseId = 3, EnrolledAt = d, Progress = 75, Status = EnrollmentStatus.Active }
            );
            db.SaveChanges();
            Console.WriteLine("Enrollments seeded.");
        }
    }

 // CREATE
    static void AddInstructor()
    {
        using var db = new AppDbContext();
        var instructor = new Instructor
        {
            FullName = "Laila Mostafa",
            Email = "laila@route.com",
            Bio = "Cloud engineer",
            JoinedAt = DateTime.UtcNow
        };
        db.Instructors.Add(instructor);
        db.SaveChanges();
        Console.WriteLine($"Instructor added. Id = {instructor.Id}");
    }

    static void AddCourse(string title, int instructorId, int categoryId, decimal price, CourseLevel level)
    {
        using var db = new AppDbContext();

        if (!db.Instructors.Any(i => i.Id == instructorId))
        { Console.WriteLine($"Cannot add course: instructor {instructorId} does not exist."); return; }
        if (!db.Categories.Any(c => c.Id == categoryId))
        { Console.WriteLine($"Cannot add course: category {categoryId} does not exist."); return; }
        if (price <= 0)
        { Console.WriteLine("Cannot add course: price must be greater than 0."); return; }

        var course = new Course
        {
            Title = title,
            Price = price,
            Level = level,
            InstructorId = instructorId,
            CategoryId = categoryId,
            IsPublished = false
        };
        db.Courses.Add(course);
        db.SaveChanges();
        Console.WriteLine($"Course '{title}' added. Id = {course.Id}");
    }

    static void AddSeveralStudents()
    {
        using var db = new AppDbContext();
        var students = new List<Student>
        {
            new() { FullName = "Hana Ibrahim", Email = "hana@mail.com", DateOfBirth = new DateTime(2000, 1, 1) },
            new() { FullName = "Ali Mahmoud", Email = "ali@mail.com", DateOfBirth = new DateTime(1997, 3, 5) },
            new() { FullName = "Dina Fathy", Email = "dina@mail.com", DateOfBirth = new DateTime(2003, 9, 12) }
        };
        db.Students.AddRange(students);   
        db.SaveChanges();                 
        foreach (var s in students)
            Console.WriteLine($"Student '{s.FullName}' added. Id = {s.Id}");
    }

    static void EnrollStudent(int studentId, int courseId)
    {
        using var db = new AppDbContext();

        var student = db.Students.Find(studentId);
        if (student is null) { Console.WriteLine($"Enrollment refused: student {studentId} not found."); return; }

        var course = db.Courses.Find(courseId);
        if (course is null) { Console.WriteLine($"Enrollment refused: course {courseId} not found."); return; }

        if (!course.IsPublished)
        { Console.WriteLine($"Enrollment refused: course '{course.Title}' is not published."); return; }

        if (db.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
        { Console.WriteLine($"Enrollment refused: {student.FullName} is already enrolled in '{course.Title}'."); return; }

        db.Enrollments.Add(new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            Progress = 0,
            Status = EnrollmentStatus.Active
        });
        db.SaveChanges();
        Console.WriteLine($"{student.FullName} enrolled in '{course.Title}'.");
    }

 // READ
    static void FindCourse(int id, string title)
    {
        using var db = new AppDbContext();

        var byId = db.Courses.Find(id);
        Console.WriteLine(byId is null ? $"Course with Id {id}: not found" : $"By Id: {byId.Title}");

        var byTitle = db.Courses.FirstOrDefault(c => c.Title == title);
        Console.WriteLine(byTitle is null ? $"Course titled '{title}': not found" : $"By title: {byTitle.Title} (Id {byTitle.Id})");
    }

    static void FilterCourses(decimal min, decimal max)
    {
        using var db = new AppDbContext();
        var courses = db.Courses
            .Where(c => c.Price >= min && c.Price <= max)
            .OrderByDescending(c => c.Price)
            .Select(c => new { c.Title, c.Level, c.Price })
            .ToList();

        Console.WriteLine($"Courses priced {min} - {max}:");
        foreach (var c in courses)
            Console.WriteLine($"  {c.Title} | {c.Level} | {c.Price}");
    }

    static void ShowCategories(int parentId)
    {
        using var db = new AppDbContext();

        var subs = db.Categories.Where(c => c.ParentCategoryId == parentId).ToList();
        Console.WriteLine($"Sub-categories of category {parentId}:");
        if (subs.Count == 0) Console.WriteLine("  (none)");
        foreach (var s in subs) Console.WriteLine($"  {s.Id} - {s.Name}");

        var tops = db.Categories.Where(c => c.ParentCategoryId == null).ToList();
        Console.WriteLine("Top-level categories:");
        foreach (var t in tops) Console.WriteLine($"  {t.Id} - {t.Name}");
    }

    static void CourseRoster(int courseId)
    {
        using var db = new AppDbContext();

        var studentIds = db.Enrollments
            .Where(e => e.CourseId == courseId)
            .Select(e => e.StudentId)
            .ToList();

        Console.WriteLine($"Course {courseId} has {studentIds.Count} enrolled student(s):");


        var students = db.Students.Where(s => studentIds.Contains(s.Id)).ToList();
        foreach (var s in students) Console.WriteLine($"  {s.FullName}");
    }

 // UPDATE
    static void ChangePrice(int courseId, decimal newPrice)
    {
        using var db = new AppDbContext();
        var course = db.Courses.Find(courseId);
        if (course is null) { Console.WriteLine($"Course {courseId} not found."); return; }
        if (newPrice <= 0) { Console.WriteLine("Price must be greater than 0."); return; }

        var old = course.Price;
        course.Price = newPrice;
        db.SaveChanges();
        Console.WriteLine($"Price of '{course.Title}' changed from {old} to {newPrice}.");
    }

    static void UpdateProgress(int studentId, int courseId, int progress)
    {
        using var db = new AppDbContext();
        if (progress < 0 || progress > 100)
        { Console.WriteLine($"Progress {progress} rejected: must be between 0 and 100."); return; }

        var enrollment = db.Enrollments.Find(studentId, courseId);  
        if (enrollment is null) { Console.WriteLine("Enrollment not found."); return; }

        enrollment.Progress = progress;
        if (progress == 100) enrollment.Status = EnrollmentStatus.Completed;
        db.SaveChanges();
        Console.WriteLine($"Progress updated to {progress}% (status: {enrollment.Status}).");
    }

    static void PublishCourse(int courseId)
    {
        using var db = new AppDbContext();
        var course = db.Courses.Find(courseId);
        if (course is null) { Console.WriteLine($"Course {courseId} not found."); return; }
        if (course.IsPublished) { Console.WriteLine($"Course '{course.Title}' is already published."); return; }

        course.IsPublished = true;
        course.PublishedDate = DateTime.UtcNow;
        db.SaveChanges();
        Console.WriteLine($"Course '{course.Title}' published on {course.PublishedDate:yyyy-MM-dd}.");
    }

// DELETE
    static void Unenroll(int studentId, int courseId)
    {
        using var db = new AppDbContext();
        var enrollment = db.Enrollments.Find(studentId, courseId);
        if (enrollment is null) { Console.WriteLine("Enrollment not found."); return; }

        db.Enrollments.Remove(enrollment);
        db.SaveChanges();
        Console.WriteLine($"Student {studentId} unenrolled from course {courseId}.");
    }

    static void DeleteCategoryUnsafe(int categoryId)
    {
        using var db = new AppDbContext();
        var category = db.Categories.Find(categoryId);
        if (category is null) { Console.WriteLine("Category not found."); return; }

        try
        {
            db.Categories.Remove(category);
            db.SaveChanges();
            Console.WriteLine("Category deleted.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("DbUpdateException: " + (ex.InnerException?.Message ?? ex.Message));
        }
    }

    static void DeleteCategorySafe(int categoryId)
    {
        using var db = new AppDbContext();
        var category = db.Categories.Find(categoryId);
        if (category is null) { Console.WriteLine("Category not found."); return; }

        if (db.Categories.Any(c => c.ParentCategoryId == categoryId))
        { Console.WriteLine($"Cannot delete '{category.Name}': it still has sub-categories."); return; }
        if (db.Courses.Any(c => c.CategoryId == categoryId))
        { Console.WriteLine($"Cannot delete '{category.Name}': it still has courses."); return; }

        db.Categories.Remove(category);
        db.SaveChanges();
        Console.WriteLine($"Category '{category.Name}' deleted.");
    }

    static void DeleteCourse(int courseId)
    {
        using var db = new AppDbContext();
        var course = db.Courses.Find(courseId);
        if (course is null) { Console.WriteLine($"Course {courseId} not found."); return; }

        int before = db.Enrollments.Count(e => e.CourseId == courseId);
        Console.WriteLine($"Enrollments before delete: {before}");

        db.Courses.Remove(course);
        db.SaveChanges();

        int after = db.Enrollments.Count(e => e.CourseId == courseId);
        Console.WriteLine($"Enrollments after delete: {after}");


    }

 // BULK
    static void BulkPriceRaise(CourseLevel level)
    {
        using var db = new AppDbContext();
        int rows = db.Courses
            .Where(c => c.IsPublished && c.Level == level)
            .ExecuteUpdate(s => s.SetProperty(c => c.Price, c => c.Price * 1.10m));
        Console.WriteLine($"Bulk price raise: {rows} row(s) affected.");
    }

    static void BulkDeleteDropped()
    {
        using var db = new AppDbContext();
        int rows = db.Enrollments
            .Where(e => e.Status == EnrollmentStatus.Dropped)
            .ExecuteDelete();
        Console.WriteLine($"Bulk delete: {rows} row(s) affected.");
    }
}