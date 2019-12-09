using System;
using System.Collections.Generic;
using System.Text;
using BackendCapstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BackendCapstone.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<StoryBoard> StoryBoards { get; set; }
        public DbSet<ClientPage> ClientPage { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ClientPageEvent> ClientPageEvents { get; set; }
        public DbSet<ClientPageUser> ClientPageUsers { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            modelBuilder.Entity<Message>()
                .Property(b => b.Timestamp)
                .HasDefaultValueSql("GETDATE()");

           
            modelBuilder.Entity<Task>()
                .Property(b => b.Timestampe)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<StoryBoard>()
               .Property(b => b.Timestamp)
               .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<UserType>().HasData(
               new UserType()
               {
                   Id = 1,
                   Type = "Admin"
               },
               new UserType()
               {
                   Id = 2,
                   Type = "Marketing Rep"
               },
               new UserType()
               {
                   Id = 3,
                   Type = "Client"
               }
           );

            ApplicationUser adminUser = new ApplicationUser
            {
                FirstName = "Admina",
                LastName = "Straytor",
                UserName = "admin@marketing.com",
                NormalizedUserName = "ADMIN@MARKETING.COM",
                Email = "admin@marketing.com",
                NormalizedEmail = "ADMIN@MARKETING.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = "7f434309-a4d9-48e9-9ebb-8803db794577",
                Id = "00000000-ffff-ffff-ffff-ffffffffffff",
                UserTypeId = 1
            };
            var adminPasswordHash = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = adminPasswordHash.HashPassword(adminUser, "Admin8*");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            modelBuilder.Entity < ClientPage>().HasData(
                new ClientPage()
                {
                    Id = 1,
                    Name = "Bink's Sports Bar & Grill",
                    Description = "Bink’s is Springfield’s hang out spot If you’re not hanging at Bink’s, you’re missing out. Come join the party!",
                },
                new ClientPage()
                {
                    Id = 2,
                    Name = "Corbin Creek Greenhouse",
                    Description = "At Corbin Creek we grow quality. Proven Winners makes up 99% of our inventory, so you're buying plants that are hearty, beautiful and engineered to stay that way! Growing plants is our first love and you can reap the benefits.  Our staff is always ready to help you with ideas and real experience to make your outdoor space easy to care for and a pleasure for you and your friends to enjoy.",
                }
            );

           

            modelBuilder.Entity<ClientPageUser>().HasData(
                new ClientPageUser()
                {
                    Id = 1,
                    UserId = adminUser.Id,
                    ClientPageId = 1
                },
                 new ClientPageUser()
                 {
                     Id = 2,
                     UserId = adminUser.Id,
                     ClientPageId = 2
                 }
            );
        }


    }
}
    

