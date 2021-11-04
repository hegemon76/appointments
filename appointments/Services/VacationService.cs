using appointments.Data;
using appointments.Models;
using appointments.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using vacations.Models.Enums;
using vacations.Models.Helper;

namespace appointments.Services
{
    public class VacationService : IVacationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _context;

        public VacationService(ApplicationDbContext db, IHttpContextAccessor context)
        {
            _db = db;
            _context = context;
        }

        public async Task<int> AddUpdate(VacationViewModel model)
        {
            var startDate = DateTime.Parse(model.StartDate);
            var endDate = DateTime.Parse(model.EndDate);
            var user = _db.Users.FirstOrDefault(x => x.Id == model.AppWorkerId);

            if (model != null && model.Id > 0)
            {
                //update
                var vacation = _db.Vacations.FirstOrDefault(x => x.Id == model.Id);
                if (!vacation.IsApproved && !vacation.IsRejected)
                {
                    vacation.Title = model.Title;
                    vacation.Description = model.Description;
                    vacation.StartDate = startDate;
                    vacation.EndDate = endDate;

                    user.VacationDays += vacation.Duration;
                    user.VacationDaysTaken -= vacation.Duration;
                    vacation.Duration = model.Duration;
                    user.VacationDays -= model.Duration;
                    user.VacationDaysTaken += vacation.Duration;

                    vacation.IsApproved = model.IsApproved;
                    vacation.AppWorkerId = model.AppWorkerId;
                    vacation.AdminId = model.AdminId;


                    await _db.SaveChangesAsync();
                    return (int)EnumStatusMessage.VacationUpdated;
                }
                return (int)EnumStatusMessage.OperationNotAllowed;
            }
            else
            {
                if (isOverlapWithOtherEvent(model.AppWorkerId, model.StartDate, model.EndDate))
                    return (int)EnumStatusMessage.OverlapDates;

                if (!isMinimumDateToday(model.StartDate))
                    return (int)EnumStatusMessage.MinimumDate;

                VacationStatus vacationStatus = _db.VacationStatus
                                                .FirstOrDefault(x => x.Id == (int)EnumVacationStatus.Pending);
                //create
                Vacation vacation = new Vacation()
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = startDate,
                    EndDate = endDate,
                    Duration = model.Duration,
                    IsApproved = false,
                    IsRejected = false,
                    AppWorkerId = model.AppWorkerId,
                    AdminId = model.AdminId,
                    VacationStatus = vacationStatus
                };

                user.VacationDays -= model.Duration;
                user.VacationDaysTaken += vacation.Duration;

                _db.Vacations.Add(vacation);
                await _db.SaveChangesAsync();
                return (int)EnumStatusMessage.VacationAdded;
            }
        }

        public List<AppWorkerViewModel> GetWorkerList()
        {
            var workers = (from user in _db.Users
                           join userRoles in _db.UserRoles on user.Id equals userRoles.UserId
                           join roles in _db.Roles.Where(x => x.Name == RoleNames.Role_AppWorker) on userRoles.RoleId equals roles.Id
                           select new AppWorkerViewModel
                           {
                               Id = user.Id,
                               Name = user.Name,
                               VacationDays = user.VacationDays,
                               VacationDaysTaken = user.VacationDaysTaken
                           }
                          ).ToList();

            return workers;
        }

        public AppWorkerViewModel GetCurrentUser()
        {
            var currentLogin = _context.HttpContext.User.Claims.ToList();
            AppWorkerViewModel model = new AppWorkerViewModel()
            {
                Name = currentLogin.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value,
                Id = currentLogin.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value
            };
            return model;
        }

        public List<VacationViewModel> VacationsEventById(string workerId, int month)
        {
            if (workerId == null)
                workerId = GetCurrentUser().Id;
            return _db.Vacations.Include(x => x.VacationStatus)
                .Where(x => x.AppWorkerId == workerId && x.StartDate.Month == month)
                .ToList().Select(c => new VacationViewModel()
                {
                    Id = c.Id,
                    Description = c.Description,
                    StartDate = c.StartDate.ToString("yyyy-MM,dd"),
                    EndDate = c.EndDate.ToString("yyyy-MM,dd"),
                    Title = c.Title,
                    Duration = c.Duration,
                    IsApproved = c.IsApproved,
                    IsRejected = c.IsRejected,
                    StatusText = c.VacationStatus.StatusText
                }).ToList();
        }

        public VacationViewModel GetById(int id)
        {
            return _db.Vacations.Include(x => x.VacationStatus)
                .Where(x => x.Id == id).ToList().Select(c => new VacationViewModel()
                {
                    Id = c.Id,
                    Description = c.Description,
                    StartDate = c.StartDate.ToString("yyyy-MM,dd"),
                    EndDate = c.EndDate.ToString("yyyy-MM,dd"),
                    Title = c.Title,
                    Duration = c.Duration,
                    IsApproved = c.IsApproved,
                    IsRejected = c.IsRejected,
                    AppWorkerId = c.AppWorkerId,
                    StatusText = c.VacationStatus.StatusText
                }).SingleOrDefault();
        }

        public async Task<int> DeleteEvent(int id)
        {
            var vacation = _db.Vacations.FirstOrDefault(x => x.Id == id);

            if (vacation != null)
            {
                var user = _db.Users.FirstOrDefault(x => x.Id == vacation.AppWorkerId);
                user.VacationDays += vacation.Duration;
                user.VacationDaysTaken -= vacation.Duration;

                _db.Vacations.Remove(vacation);
                await _db.SaveChangesAsync();
                return (int)EnumStatusMessage.VacationDeleted;
            }
            return (int)EnumStatusMessage.failure_code;
        }

        public async Task<int> ConfirmEvent(int id)
        {
            var vacation = _db.Vacations.FirstOrDefault(x => x.Id == id);
            if (vacation != null)
            {
                var vacationStatus = _db.VacationStatus.FirstOrDefault(x => x.Id == (int)EnumVacationStatus.Accepted);
                vacation.IsApproved = true;
                vacation.VacationStatus = vacationStatus;
                await _db.SaveChangesAsync();
                return (int)EnumStatusMessage.VacationConfirmed;
            }
            return (int)EnumStatusMessage.failure_code;
        }

        public async Task<int> RejectEvent(int id)
        {
            var vacation = _db.Vacations.FirstOrDefault(x => x.Id == id);
            if (vacation != null)
            {
                if (!vacation.IsApproved)
                {
                    var user = _db.Users.FirstOrDefault(x => x.Id == vacation.AppWorkerId);
                    user.VacationDays += vacation.Duration;
                    user.VacationDaysTaken -= vacation.Duration;

                    var vacationStatus = _db.VacationStatus.FirstOrDefault(x => x.Id == (int)EnumVacationStatus.Rejected);
                    vacation.IsRejected = true;
                    vacation.VacationStatus = vacationStatus;
                    await _db.SaveChangesAsync();
                    return (int)EnumStatusMessage.VacationRejected;
                }
                return (int)EnumStatusMessage.OperationNotAllowed;
            }
            return (int)EnumStatusMessage.failure_code;
        }

        private bool isOverlapWithOtherEvent(string workerId, string startDate, string endDate)
        {
            DateTime _startDate = Convert.ToDateTime(startDate);
            DateTime _endDate = Convert.ToDateTime(endDate);

            Vacation isEventOverlap = _db.Vacations
                .Where(x => x.AppWorkerId == workerId)
                .Where(x => x.StartDate >= _startDate && x.EndDate <= _endDate).FirstOrDefault();

            if (isEventOverlap == null)
                return false;
            else
                return true;
        }

        private bool isMinimumDateToday(string startDate)
        {
            DateTime _startDate = Convert.ToDateTime(startDate).Date;
            DateTime now = DateTime.Now.Date;
            if (_startDate < now)
                return false;

            return true;
        }
    }

}
