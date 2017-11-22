using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class BookDbContext : DbContext
    {
        public BookDbContext()
            : base("BookDbConnectionString")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<BookDbContext>());
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
