using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<User> Users { get; set; }
  public DbSet<UserRoles> UserRoles { get; set; }


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("tblUsers");  
            entity.HasKey(e => e.ID);
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.UserName).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.HasOne(e => e.UserRole)
                  .WithMany()
                  .HasForeignKey(e => e.Role);
        });

        modelBuilder.Entity<UserRoles>(entity =>
        {
            entity.ToTable("tblUsersRoles");  
            entity.HasKey(e => e.ID);
            entity.Property(e => e.UserRole).IsRequired();
        });

     


  }

}





