namespace appointments.Models.ViewModels
{
    public class VacationViewModel
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Duration { get; set; }
        public string AppWorkerId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string AdminId { get; set; }
        public string AdminName { get; set; }
        public bool IsForClient { get; set; }
        public int StatusId { get; set; }
    }
}
