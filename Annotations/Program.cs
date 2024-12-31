using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Microsoft.EntityFrameworkCore;

namespace Annotations
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (FluentContext db = new FluentContext())
                {
                    Student s1 = new Student { Ident = 1, FirstName = "Иван", LastName = "Иванов", AverageScore = 11, Phone = "+380671234567", Address = "Садовая, 3", Term = 1 };
                    Student s2 = new Student { Ident = 2, FirstName = "Петр", LastName = "Петров", AverageScore = 12, Phone = "+380671234568", Address = "Садовая, 3", Term = 2 };
                    Student s3 = new Student { Ident = 3, FirstName = "Алексей", LastName = "Алексеев", AverageScore = 10, Phone = "+380671234569", Address = "Садовая, 3", Term = 3 };

                    db.Students.Add(s1);
                    db.Students.Add(s2);
                    db.Students.Add(s3);

                    db.SaveChanges();

                    foreach (Student p in db.Students)
                        Console.WriteLine("{0, 8}{1, 9}{2, 4}{3, 15}{4, 4}{5, 13}", p.FirstName, p.LastName, p.AverageScore, p.Phone, p.Term, p.Address);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    [Table("StudentsOfSTEP")]
    public class Student
    {
        [Key] // Для установки свойства в качестве первичного ключа
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Отключить автогенерацию значения при добавлении
        public int Ident { get; set; }

        [Required] // Данное свойство обязательно для установки, т.е. будет иметь определение NOT NULL в БД
        [MaxLength(20)]
        [Column("StudentName")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("StudentSurname")]
        public string LastName { get; set; }

        [Required]
        public double AverageScore { get; set; }

        [NotMapped] // Исключить определенное свойство, чтобы для него не создавался столбец в таблице
        public string Address { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [Required]
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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-G30VB0K\MSSQLSERVER01;Database=Annotations;Integrated Security=SSPI;TrustServerCertificate=true");
        }
    }
}
