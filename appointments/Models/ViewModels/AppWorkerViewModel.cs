namespace appointments.Models.ViewModels
{
    public class AppWorkerViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int VacationDays { get; set; }
        public int VacationDaysLeft { get; set; }
        public int VacationDaysTaken { get; set; }
    }
}
