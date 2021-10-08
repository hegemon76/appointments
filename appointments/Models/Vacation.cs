using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appointments.Models
{
    public class Vacation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public string AppWorkerId { get; set; }
        public bool IsApproved { get; set; }
        public string AdminId { get; set; }
    }
}
