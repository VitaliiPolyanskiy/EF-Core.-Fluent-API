using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

namespace Fluent_API
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            try
            {
                using (FluentContext db = new FluentContext())
                {
                    Student s1 = new Student { FirstName = "Іван", LastName = "Іваненко", AverageScore = 11, Phone = "+380671234567", Address = "Садова, 3", Term = 1 };
                    Student s2 = new Student { FirstName = "Петро", LastName = "Петренко", AverageScore = 12, Phone = "+380671234568", Address = "Садова, 3", Term = 2 };
                    Student s3 = new Student { FirstName = "Олексій", LastName = "Олексієнко", AverageScore = 10, Phone = "+380671234569", Address = "Садова, 3", Term = 3 };

                    db.Students.Add(s1);
                    db.Students.Add(s2);
                    db.Students.Add(s3);

                    db.SaveChanges();

                    foreach (Student p in db.Students)
                        Console.WriteLine("{0, 8}{1, 12}{2, 4}{3, 4}{4, 15}{5, 3}", p.FirstName, p.LastName, p.Age,p.AverageScore, p.Phone, p.Term);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class Student
    {
        public int Ident { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public decimal AverageScore { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int Term { get; set; }
    }

    class FluentContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        public FluentContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Зіставлення класу з таблицею
            modelBuilder.Entity<Student>().ToTable("StudentsOfSTEP");

            // Перевизначення первинного ключа
            modelBuilder.Entity<Student>().HasKey(p => p.Ident);

            // Зіставлення властивостей
            modelBuilder.Entity<Student>().Property(p => p.FirstName).HasColumnName("StudentName");
            modelBuilder.Entity<Student>().Property(p => p.LastName).HasColumnName("StudentSurname");

            modelBuilder.Entity<Student>().Property(u => u.Age).HasDefaultValue(18);
            modelBuilder.Entity<Student>()
            .ToTable(t => t.HasCheckConstraint("Age", "Age > 0 AND Age < 120"));

            // Виняток зіставлення для властивості
            modelBuilder.Entity<Student>().Ignore(p => p.Address);

            // Значення для стовпця та властивості потрібно обов'язково
            modelBuilder.Entity<Student>().Property(p => p.FirstName).IsRequired();
            modelBuilder.Entity<Student>().Property(p => p.LastName).IsRequired();
            modelBuilder.Entity<Student>().Property(p => p.AverageScore).IsRequired();

            // Налаштування рядків
            modelBuilder.Entity<Student>().Property(p => p.FirstName).HasMaxLength(20);
            modelBuilder.Entity<Student>().Property(p => p.LastName).HasMaxLength(20);
            modelBuilder.Entity<Student>().Property(p => p.FirstName).IsUnicode(false);
            modelBuilder.Entity<Student>().Property(p => p.LastName).IsUnicode(false);

            // Налаштування чисел decimal
            modelBuilder.Entity<Student>().Property(p => p.AverageScore).HasPrecision(5, 2);

            // Налаштування типу стовпців
            modelBuilder.Entity<Student>().Property(p => p.Phone).HasColumnType("varchar").HasMaxLength(20);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=FluentAPI;Integrated Security=SSPI;TrustServerCertificate=true");

        }
    }
}
