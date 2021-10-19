using System;
using System.ComponentModel.DataAnnotations;

namespace appointments.Models
{
    public class Vacation
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public string AppWorkerId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string AdminId { get; set; }
        public VacationStatus VacationStatus { get; set; }
    }
}
