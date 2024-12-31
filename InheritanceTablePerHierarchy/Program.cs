using System;
using Microsoft.EntityFrameworkCore;

namespace InheritanceTablePerHierarchy
{
    // При использовании подхода TPH (Table Per Hierarchy / Таблица на одну иерархию классов) для одной иерархии классов используется одна таблица. 
    // Данные базовых и производных классов сохраняются в одну таблицу, а для их отличия создается специальный столбец - Discriminator. 
    // Он имеет тип nvarchar и имеет длину в 128 символов. Данный столбец и будет определять относится строка к типу Person или Student.
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (StudentContext db = new StudentContext())
                {
                    Person p1 = new Person { FirstName = "Иван", LastName = "Иванов", Age = 20, Phone = "+380671234567", Address = "Садовая, 3" };
                    Person p2 = new Person { FirstName = "Петр", LastName = "Петров", Age = 30, Phone = "+380671234568", Address = "Садовая, 3" };

                    db.Persons.Add(p1);
                    db.Persons.Add(p2);

                    Student s1 = new Student { FirstName = "Алексей", LastName = "Алексеев", Age = 20, AverageScore = 11, Phone = "+380671234567", Address = "Садовая, 3", Term = 1 };
                    Student s2 = new Student { FirstName = "Сергей", LastName = "Сергеев", Age = 30, AverageScore = 12, Phone = "+380671234568", Address = "Садовая, 3", Term = 2 };

                    db.Students.Add(s1);
                    db.Students.Add(s2);

                    db.SaveChanges();

                    foreach (Person p in db.Persons)
                        Console.WriteLine("{0, 8}{1, 9}{2, 4}{3, 15}{4, 15}", p.FirstName, p.LastName, p.Age, p.Phone, p.Address);
                    Console.WriteLine();
                    foreach (Student p in db.Students)
                        Console.WriteLine("{0, 8}{1, 9}{2, 4}{3, 4}{4, 15}{5, 15}{6, 3}", p.FirstName, p.LastName, p.Age, p.AverageScore, p.Phone, p.Address, p.Term);
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
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-G30VB0K\MSSQLSERVER01;Database=Students;Integrated Security=SSPI;TrustServerCertificate=true");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Начиная с версии EF Core 7.0 также можно вызвать метод UseTphMappingStrategy для базовой сущности иерархии
            modelBuilder.Entity<Person>().UseTphMappingStrategy();
        }
    }
}
