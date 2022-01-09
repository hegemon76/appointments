using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using appointments.Models;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using vacations.Models.Helper;

namespace appointments.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<VacationStatus> VacationStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<VacationStatus>().HasData(SeedVacationStatusData());

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = RoleNames.Role_Admin, NormalizedName = RoleNames.Role_Admin },
                new IdentityRole { Name = RoleNames.Role_AppWorker, NormalizedName= RoleNames.Role_AppWorker }
                );
        }

        public List<VacationStatus> SeedVacationStatusData()
        {
            var vacationStatus = new List<VacationStatus>();
            using (StreamReader r = new StreamReader(@"C:\Users\Adrian\source\repos\hegemon76\appointments\appointments\Data\seeders\vacationStatus.JSON"))
            {
                string json = r.ReadToEnd();
                vacationStatus = JsonConvert.DeserializeObject<List<VacationStatus>>(json);
            }
            return vacationStatus;
        }
    }
}
