using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Renavam.Repository.Models;
using Salao.Repository.Models.Account;

namespace Salao.Repository.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<MenuRole> MenuRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações padrão
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.GenericPassword)
            .HasMaxLength(100)
            .IsRequired(false);

            modelBuilder.Entity<MenuRole>()
        .HasKey(mr => new { mr.Id, mr.MenuId, mr.RoleId });

            modelBuilder.Entity<MenuRole>()
                .HasOne(mr => mr.Menu)
                .WithMany(m => m.MenuRoles)
                .HasForeignKey(mr => mr.MenuId);

            modelBuilder.Entity<MenuRole>()
                .HasOne(mr => mr.Role)
                .WithMany()
                .HasForeignKey(mr => mr.RoleId);
        }
    }
}
