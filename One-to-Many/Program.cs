using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace One_to_Many
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (FluentContext db = new FluentContext())
                {
                    var academygroup1 = new AcademyGroup { Name = "БПУ-1421" };
                    var academygroup2 = new AcademyGroup { Name = "ВПД-1411" };
                    db.Groups.Add(academygroup1);
                    db.Groups.Add(academygroup2);
                    var student1 = new Student
                    {
                        FirstName = "Дмитрий",
                        LastName = "Морозов",
                        Age = 20,
                        PointAverage = 10.5,
                        AcademyGroup = academygroup2
                    };
                    var student2 = new Student
                    {
                        FirstName = "Екатерина",
                        LastName = "Малова",
                        Age = 27,
                        PointAverage = 11.5,
                        AcademyGroup = academygroup2
                    };
                    var student3 = new Student
                    {
                        FirstName = "Максим",
                        LastName = "Москалик",
                        Age = 23,
                        PointAverage = 12,
                        AcademyGroup = academygroup1
                    };
                    db.Students.Add(student1);
                    db.Students.Add(student2);
                    db.Students.Add(student3);

                    db.SaveChanges();
                    foreach (Student p in db.Students)
                        Console.WriteLine("{0, 10}{1, 10}{2, 5}{3, 7}",
                            p.FirstName, p.LastName, p.Age, p.PointAverage);
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
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public double PointAverage { get; set; }
        [ForeignKey("AcademyGroupIdent")]
        public AcademyGroup AcademyGroup { get; set; }

    }

    public class AcademyGroup
    {
        public AcademyGroup()
        {
            Student = new HashSet<Student>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Student> Student { get; set; }
    }

    class FluentContext : DbContext
    {
        public DbSet<AcademyGroup> Groups { get; set; }
        public DbSet<Student> Students { get; set; }

        public FluentContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-G30VB0K\MSSQLSERVER01;Database=One_to_Many;Integrated Security=SSPI;TrustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Установим связь Один ко Многим между объектом AcademyGroup и объектами Student 

            modelBuilder.Entity<Student>().HasOne(p => p.AcademyGroup).WithMany(t => t.Student).OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
