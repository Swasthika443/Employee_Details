using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Employee_Details.Models;

public partial class EmployeeContext : DbContext
{
    public EmployeeContext()
    {
    }

    public EmployeeContext(DbContextOptions<EmployeeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Designation> Designations { get; set; }

   

    public virtual DbSet<Employee> Employees { get; set; }

   

    public virtual DbSet<Gender> Genders { get; set; }

 

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //  => optionsBuilder.UseSqlServer("Server=ELW5339;Database=employee;Trusted_Connection=True;TrustServerCertificate=True;");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      
        modelBuilder.Entity<Designation>(entity =>
        {
            entity.HasKey(e => e.DesignationId);

            entity.Property(e => e.DesignationName)
                .HasMaxLength(50)
                .IsUnicode(false);

        });

     

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmpId);



            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Gender);
            entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Designation);

            entity.HasOne(e => e.DesignationNavigation)
          .WithMany(d => d.Employees)
          .HasForeignKey(e => e.Designation);

            entity.HasOne(e => e.GenderNavigation)
                .WithMany(g => g.Employees)
                .HasForeignKey(e => e.Gender);
        });

      
        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.GenderId);

            entity.Property(e => e.GenderName)
                .HasMaxLength(50)
                .IsUnicode(false);

        });

     
        

        OnModelCreatingPartial(modelBuilder);
    }
    

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


}
