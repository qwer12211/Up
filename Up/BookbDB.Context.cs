﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Up
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BookDBEntities2 : DbContext
    {
        public BookDBEntities2()
            : base("name=BookDBEntities2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Books> Books { get; set; }
        public virtual DbSet<Genre> Genre { get; set; }
        public virtual DbSet<Positions> Positions { get; set; }
        public virtual DbSet<Sales> Sales { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<Statuses> Statuses { get; set; }
        public virtual DbSet<Stores> Stores { get; set; }
        public virtual DbSet<StoresBooks> StoresBooks { get; set; }
        public virtual DbSet<StoresSupplier> StoresSupplier { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<Supply> Supply { get; set; }
    }
}
