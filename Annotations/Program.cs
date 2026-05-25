using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Annotations
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
                    Student s1 = new Student { Ident = 1, FirstName = "Іван", LastName = "Іваненко", AverageScore = 11, Phone = "+380671234567", Address = "Садова, 3", Term = 1 };
                    Student s2 = new Student { Ident = 2, FirstName = "Петро", LastName = "Петренко", AverageScore = 12, Phone = "+380671234568", Address = "Садова, 3", Term = 2 };
                    Student s3 = new Student { Ident = 3, FirstName = "Олексій", LastName = "Олексієнко", AverageScore = 10, Phone = "+380671234569", Address = "Садова, 3", Term = 3 };

                    db.Students.Add(s1);
                    db.Students.Add(s2);
                    db.Students.Add(s3);

                    db.SaveChanges();

                    foreach (Student p in db.Students)
                        Console.WriteLine("{0, 8}{1, 12}{2, 4}{3, 15}{4, 4}{5, 13}", p.FirstName, p.LastName, p.AverageScore, p.Phone, p.Term, p.Address);
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
        [Key] // Для встановлення властивості в якості первинного ключа
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Вимкнути автогенерацію значення при додаванні
        public int Ident { get; set; }

        [Required] // Ця властивість обов'язкова для установки, тобто буде мати визначення NOT NULL у базі даних
        [MaxLength(20)]
        [Column("StudentName")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("StudentSurname")]
        public string LastName { get; set; }

        [Required]
        public double AverageScore { get; set; }

        [NotMapped] // Виключити певну властивість, щоб для неї не створювався стовпець у таблиці
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
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=Annotations;Integrated Security=SSPI;TrustServerCertificate=true");
        }
    }
}
