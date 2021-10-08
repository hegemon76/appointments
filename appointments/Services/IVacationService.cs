using appointments.Models;
using appointments.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace appointments.Services
{
    public interface IVacationService
    {
        public List<AppWorkerViewModel> GetWorkerList();
        public Task<int> AddUpdate(VacationViewModel model);
        public List<Vacation> VacationsEventById(string workerId);
    }
}
