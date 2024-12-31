using Microsoft.EntityFrameworkCore;
using System;

namespace Fluent_API
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (FluentContext db = new FluentContext())
                {
                    Student s1 = new Student { FirstName = "Иван", LastName = "Иванов", Age = 20, AverageScore = 11, Phone = "+380671234567", Address = "Садовая, 3", Term = 1 };
                    Student s2 = new Student { FirstName = "Петр", LastName = "Петров", Age = 19, AverageScore = 12, Phone = "+380671234568", Address = "Садовая, 3", Term = 2 };
                    Student s3 = new Student { FirstName = "Алексей", LastName = "Алексеев", AverageScore = 10, Phone = "+380671234569", Address = "Садовая, 3", Term = 3 };

                    db.Students.Add(s1);
                    db.Students.Add(s2);
                    db.Students.Add(s3);

                    db.SaveChanges();

                    foreach (Student p in db.Students)
                        Console.WriteLine("{0, 8}{1, 9}{2, 4}{3, 4}{4, 15}{5, 3}", p.FirstName, p.LastName, p.Age,p.AverageScore, p.Phone, p.Term);
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
            // Сопоставление класса с таблицей
            modelBuilder.Entity<Student>().ToTable("StudentsOfSTEP");

            // Переопределение первичного ключа
            modelBuilder.Entity<Student>().HasKey(p => p.Ident);

            // Сопоставление свойств
            modelBuilder.Entity<Student>().Property(p => p.FirstName).HasColumnName("StudentName");
            modelBuilder.Entity<Student>().Property(p => p.LastName).HasColumnName("StudentSurname");

            modelBuilder.Entity<Student>().Property(u => u.Age).HasDefaultValue(18);
            modelBuilder.Entity<Student>()
            .ToTable(t => t.HasCheckConstraint("Age", "Age > 0 AND Age < 120"));

            // Исключение сопоставления для свойства
            modelBuilder.Entity<Student>().Ignore(p => p.Address);

            // Значение для столбца и свойства требуется обязательно
            modelBuilder.Entity<Student>().Property(p => p.FirstName).IsRequired();
            modelBuilder.Entity<Student>().Property(p => p.LastName).IsRequired();
            modelBuilder.Entity<Student>().Property(p => p.AverageScore).IsRequired();

            // Настройка строк
            modelBuilder.Entity<Student>().Property(p => p.FirstName).HasMaxLength(20);
            modelBuilder.Entity<Student>().Property(p => p.LastName).HasMaxLength(20);
            modelBuilder.Entity<Student>().Property(p => p.FirstName).IsUnicode(false);
            modelBuilder.Entity<Student>().Property(p => p.LastName).IsUnicode(false);

            // Настройка чисел decimal
            modelBuilder.Entity<Student>().Property(p => p.AverageScore).HasPrecision(5, 2);

            // Настройка типа столбцов
            modelBuilder.Entity<Student>().Property(p => p.Phone).HasColumnType("varchar").HasMaxLength(20);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-G30VB0K\MSSQLSERVER01;Database=FluentAPI;Integrated Security=SSPI;TrustServerCertificate=true");

        }
    }
}
