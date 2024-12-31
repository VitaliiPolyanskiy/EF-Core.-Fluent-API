using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Many_to_many
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var db = new LanguageContext())
                {
                    Language lang1 = new Language { Name = "Английский" };
                    Language lang2 = new Language { Name = "Испанский" };
                    Language lang3 = new Language { Name = "Французский" };
                    Language lang4 = new Language { Name = "Португальский" };

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
                        Name = "Южная Америка",
                        Languages = new List<Language>() { lang2, lang4 }
                    };
                    Continent c3 = new Continent
                    {
                        Name = "Европа",
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
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-G30VB0K\MSSQLSERVER01;Database=Many_to_Many;Integrated Security=SSPI;TrustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Установим связь Один ко Многим между объектами Continent и объектами Language 
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
