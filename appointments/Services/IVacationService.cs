using appointments.Models;
using appointments.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace appointments.Services
{
    public interface IVacationService
    {
        public List<AppWorkerViewModel> GetWorkerList();
        public AppWorkerViewModel GetCurrentUser();
        public Task<int> AddUpdate(VacationViewModel model);
        public List<VacationViewModel> VacationsEventById(string workerId);
        public VacationViewModel GetById(int id);
        public Task<int> DeleteEvent(int id);
        public Task<int> ConfirmEvent(int id);
    }
}
