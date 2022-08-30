﻿using Domain.Models;
using Domain.Models.WorkshopDomaine;
using Microsoft.EntityFrameworkCore;

namespace Repository.Maps
{
    public static class WorkshopParameterMap
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkshopParameter>().HasKey(p => p.Id);

            modelBuilder.Entity<WorkshopParameter>()
                        .HasOne(s => s.Worksĥop)
                        .WithMany(g => g.WorkshopParameters)
                        .HasForeignKey(s => s.IdWorkshop);
        }
    }
}