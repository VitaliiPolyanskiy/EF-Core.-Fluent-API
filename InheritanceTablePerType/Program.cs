using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

namespace InheritanceTablePerType
{

// Підхід TPT (Table Per Type / Таблиця на тип) передбачає збереження в загальній таблиці тільки тих властивостей, які є спільними для всіх класів-спадкоємців, 
// тобто які визначені в базовому класі. Ті властивості, які відносяться тільки до похідного класу, зберігаються в окремій таблиці.
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            try
            {
                using (StudentContext db = new StudentContext())
                {
                    Person p1 = new Person { FirstName = "Іван", LastName = "Іваненко", Age = 20, Phone = "+380671234567", Address = "Садова, 3" };
                    Person p2 = new Person { FirstName = "Петро", LastName = "Петренко", Age = 30, Phone = "+380671234568", Address = "Садова, 3" };

                    db.Persons.Add(p1);
                    db.Persons.Add(p2);

                    Student s1 = new Student { FirstName = "Олексій", LastName = "Олексієнко", Age = 20, AverageScore = 11, Phone = "+380671234567", Address = "Садова, 3", Term = 1 };
                    Student s2 = new Student { FirstName = "Сергій", LastName = "Сергієнко", Age = 30, AverageScore = 12, Phone = "+380671234568", Address = "Садова, 3", Term = 2 };

                    db.Students.Add(s1);
                    db.Students.Add(s2);

                    db.SaveChanges();

                    foreach (Person p in db.Persons)
                        Console.WriteLine("{0, 8}{1, 12}{2, 4}{3, 15}{4, 15}", p.FirstName, p.LastName, p.Age, p.Phone, p.Address);
                    Console.WriteLine();
                    foreach (Student p in db.Students)
                        Console.WriteLine("{0, 8}{1, 12}{2, 4}{3, 4}{4, 15}{5, 15}{6, 3}", p.FirstName, p.LastName, p.Age, p.AverageScore, p.Phone, p.Address, p.Term);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    //  [Table("People")]
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

    //   [Table("Students")]
    public class Student : Person
    {
        public decimal AverageScore { get; set; }
        public int Term { get; set; }
    }

    class StudentContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Student> Students { get; set; }

        public StudentContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=Students;Integrated Security=SSPI;TrustServerCertificate=true");

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("People");
            modelBuilder.Entity<Student>().ToTable("Students");

            modelBuilder.Entity<Person>().UseTptMappingStrategy();
        }
    }

}
