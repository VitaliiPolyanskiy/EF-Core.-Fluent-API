using System;
using Microsoft.EntityFrameworkCore;

namespace InheritanceTablePerType
{
    // Подход TPT(Table Per Type / Таблица на тип) предполагает сохранение в общей таблице только тех свойств, которые общие для всех классов-наследников, 
    // т.е. которые определены в базовом классе. Те свойства, которые относятся только к производному классу, сохраняются в отдельной таблице.
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
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-G30VB0K\MSSQLSERVER01;Database=Students;Integrated Security=SSPI;TrustServerCertificate=true");

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("People");
            modelBuilder.Entity<Student>().ToTable("Students");

            // Начиная с версии EF Core 7.0 также можно вызвать метод UseTptMappingStrategy для базовой сущности иерархии
            modelBuilder.Entity<Person>().UseTptMappingStrategy();
        }
    }

}
