using appointments.Models;
using appointments.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vacations.Models.ViewModels;

namespace appointments.Services
{
    public interface IVacationService
    {
        public List<AppWorkerViewModel> GetWorkerList();
        public Task<int> AddUpdate(VacationViewModel model);
        public List<VacationViewModel> VacationsEventById(string workerId, int month);
        public VacationViewModel GetById(int id);
        public Task<int> DeleteEvent(int id);
        public Task<int> ConfirmEvent(int id);
        public Task<int> RejectEvent(int id);
        public AppWorkerViewModel GetCurrentUser();
        public VacationViewModel GetVacationsDaysByMonth(string appUserId, int month);
        public VacationsDaysInfoVm GetVacationsDaysInfo(string appUserId, int month);
        public List<VacationViewModel> GetAllVacations();
    }
}
