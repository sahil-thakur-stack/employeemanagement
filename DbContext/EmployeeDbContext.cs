using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
public class EmployeeDbContext : DbContext
{
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) { }

   
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

       
       modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("tblEmployees");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.EmployeeCode).IsRequired().IsUnicode().IsUnicode();
            entity.Property(e => e.DateOfJoining).IsRequired();
            entity.Property(e => e.DateOfBirth).IsRequired();
            entity.Property(e => e.Salary).IsRequired();
        });


    }
}

