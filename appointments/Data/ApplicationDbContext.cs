using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using appointments.Models;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace appointments.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vacation> Vacations {get;set;}
        public DbSet<VacationStatus> VacationStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<VacationStatus>().HasData(SeedVacationStatusData());
        }

        public List<VacationStatus> SeedVacationStatusData()
        {
            var vacationStatus = new List<VacationStatus>();
            using (StreamReader r = new StreamReader(@"C:\Users\Adin\source\repos\appointments\appointments\Data\seeders\vacationStatus.JSON"))
            {
                string json = r.ReadToEnd();
               vacationStatus = JsonConvert.DeserializeObject<List<VacationStatus>>(json);
            }
            return vacationStatus;
        }
    }
}
