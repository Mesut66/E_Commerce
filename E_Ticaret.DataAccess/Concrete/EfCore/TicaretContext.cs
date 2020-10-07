using System;
using System.Collections.Generic;
using System.Text;
using E_Ticaret.Entities;
using E_Ticaret.Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace E_Ticaret.DataAccess.Concrete.EfCore
{
   public class TicaretContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>().HasKey(x => new {x.CategoryId, x.ProductId});
        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; }

    }
}
