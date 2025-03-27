using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace HotelManagement.Infrastructure.Data
{
    
    public class AppDbContext : IdentityDbContext<Guest, IdentityRole, string>
    {
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Guest>().ToTable("Guests");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            
            modelBuilder.Entity<Hotel>()
                .HasOne(h => h.Manager)
                .WithOne(m => m.Hotel)
                .HasForeignKey<Manager>(m => m.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.Rooms)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Guest>()
                .HasMany(g => g.Reservations)
                .WithOne(r => r.Guest)
                .HasForeignKey(r => r.GuestId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Reservations)
                .WithOne(res => res.Room)
                .HasForeignKey(res => res.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Reservations)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<Manager>()
                .HasIndex(m => m.Email)
                .IsUnique();

            modelBuilder.Entity<Manager>()
                .HasIndex(m => m.PersonalNumber)
                .IsUnique();

            modelBuilder.Entity<Guest>()
                .HasIndex(g => g.PersonalNumber)
                .IsUnique();

            modelBuilder.Entity<Guest>()
                .HasIndex(g => g.PhoneNumber)
                .IsUnique();
        }
    }
}