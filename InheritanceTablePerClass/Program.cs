using Microsoft.EntityFrameworkCore;
using System.Text;


namespace InheritanceTablePerClass
{
    // Починаючи з версії EF Core 7.0, у фреймворк було додано підтримку нового підходу до
    // успадкування — TPC (Table Per Concrete Type / Таблиця на кожен конкретний тип).
    // Цей підхід передбачає створення для кожної моделі окремої таблиці.
    // Стовпці в кожній таблиці створюються за всіма властивостями, зокрема й успадкованими.
    // TPC працює оптимальніше порівняно з TPT для багатьох типів запитів,
    // оскільки кількість таблиць, до яких необхідно звертатися, зменшена.
    // Крім того, результати з кожної таблиці об'єднуються за допомогою sql-команди UNION ALL,
    // що може бути значно швидше, ніж об'єднання таблиць за допомогою INNER JOIN, яке застосовується в TPT.

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

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

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
            modelBuilder.Entity<Person>().UseTpcMappingStrategy();  
        }
    }

}
