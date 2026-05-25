using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Many_to_many
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            try
            {
                using (var db = new LanguageContext())
                {
                    Language lang1 = new Language { Name = "Англійська" };
                    Language lang2 = new Language { Name = "Іспанська" };
                    Language lang3 = new Language { Name = "Французька" };
                    Language lang4 = new Language { Name = "Португальська" };

                    db.Languages.Add(lang1);
                    db.Languages.Add(lang2);
                    db.Languages.Add(lang3);
                    db.Languages.Add(lang4);

                    Continent c1 = new Continent
                    {
                        Name = "Африка",
                        Languages = new List<Language>() { lang1, lang3, lang4 }
                    };
                    Continent c2 = new Continent
                    {
                        Name = "Південна Америка",
                        Languages = new List<Language>() { lang2, lang4 }
                    };
                    Continent c3 = new Continent
                    {
                        Name = "Європа",
                        Languages = new List<Language>() { lang1, lang2, lang3, lang4 }
                    };

                    db.Continents.Add(c1);
                    db.Continents.Add(c2);
                    db.Continents.Add(c3);
                    db.SaveChanges();

                    var query = from b in db.Languages
                                select b;
                    List<Language> list = query.ToList();
                    foreach (var l in list)
                    {
                        Console.WriteLine(l.Name);
                        foreach (var cont in l.Continents)
                        {
                            Console.WriteLine("\t" + cont.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    public class Continent
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Language> Languages { get; set; }
    }

    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Continent> Continents { get; set; }
    }

    public class LanguageContext : DbContext
    {
        public DbSet<Continent> Continents { get; set; }
        public DbSet<Language> Languages { get; set; }

        public LanguageContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=Many_to_Many;Integrated Security=SSPI;TrustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Встановимо зв'язок Багато до Багатьох між об'єктами Continent та об'єктами Language 
            modelBuilder.Entity<Continent>()
            .HasMany(p => p.Languages)
            .WithMany(c => c.Continents)
            .UsingEntity(m =>
            {
                m.ToTable("ContinentsLanguages");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
