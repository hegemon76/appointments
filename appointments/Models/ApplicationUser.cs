using Microsoft.AspNetCore.Identity;

namespace appointments.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string Name { get; set; }
        public int VacationDays { get; set; }
        public int VacationDaysTaken { get; set; }
    }
}
