﻿using Domain.Models.MainDomain;
using Microsoft.EntityFrameworkCore;

namespace Repository.Maps
{
    public static class ProductFiringMap
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductFiring>().HasKey(p => new { p.IdProduct, p.IdFiring });

            modelBuilder.Entity<ProductFiring>().HasKey(pf => new { pf.IdProduct, pf.IdFiring });
            modelBuilder.Entity<ProductFiring>().HasOne(pf => pf.Product).WithMany(f => f.ProductFiring).HasForeignKey(pf => pf.IdProduct);
            modelBuilder.Entity<ProductFiring>().HasOne(pf => pf.Firing).WithMany(f => f.ProductFiring).HasForeignKey(f => f.IdFiring);
        }
    }
}