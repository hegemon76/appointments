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
using Microsoft.AspNetCore.Identity;

namespace appointments.Services
{
    public class VacationService : IVacationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VacationService(ApplicationDbContext db, IHttpContextAccessor context, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> AddUpdate(VacationViewModel model)
        {
            if (model != null && model.Id > 0)
            {
                return await UpdateEvent(model);
            }
            else
            {
                return await AddEvent(model);
            }
        }

        private async Task<int> AddEvent(VacationViewModel model)
        {
            try
            {
                if (isOverlapWithOtherEvent(model.AppWorkerId, model.StartDate, model.EndDate))
                    return (int)EnumStatusMessage.OverlapDates;

                if (!isMinimumDateToday(model.StartDate))
                    return (int)EnumStatusMessage.MinimumDate;

                var startDate = DateTime.Parse(model.StartDate);
                var endDate = DateTime.Parse(model.EndDate);

                VacationStatus vacationStatus = _db.VacationStatus
                                                .FirstOrDefault(x => x.Id == (int)EnumVacationStatus.Pending);

                var duration = (endDate - startDate).Days;
                //create
                Vacation vacation = new Vacation()
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = startDate,
                    EndDate = endDate,
                    Duration = duration + 1,
                    IsApproved = false,
                    IsRejected = false,
                    AppWorkerId = model.AppWorkerId,
                    AdminId = model.AdminId,
                    VacationStatus = vacationStatus
                };

                var user = _db.Users.FirstOrDefault(x => x.Id == model.AppWorkerId);

                if (user != null)
                {
                    user.VacationDays -= vacation.Duration;
                    _db.Users.Update(user);
                    _db.Vacations.Add(vacation);
                    await _db.SaveChangesAsync();
                    return (int)EnumStatusMessage.VacationAdded;
                }
                return (int)EnumStatusMessage.failure_code;
            }
            catch (Exception)
            {
                return (int)EnumStatusMessage.failure_code;
            }
        }

        private async Task<int> UpdateEvent(VacationViewModel model)
        {
            try
            {
                var startDate = DateTime.Parse(model.StartDate);
                var endDate = DateTime.Parse(model.EndDate);

                var duration = (endDate - startDate).Days;
                //update
                var vacation = _db.Vacations.FirstOrDefault(x => x.Id == model.Id);
                if (!vacation.IsApproved && !vacation.IsRejected)
                {
                    vacation.Title = model.Title;
                    vacation.Description = model.Description;
                    vacation.StartDate = startDate;
                    vacation.EndDate = endDate;
                    vacation.Duration = duration + 1;
                    vacation.IsApproved = model.IsApproved;
                    vacation.AppWorkerId = model.AppWorkerId;
                    vacation.AdminId = model.AdminId;
                    await _db.SaveChangesAsync();
                    return (int)EnumStatusMessage.VacationUpdated;
                }
                return (int)EnumStatusMessage.OperationNotAllowed;
            }
            catch (Exception)
            {
                return (int)EnumStatusMessage.failure_code;
            }
        }

        public List<AppWorkerViewModel> GetWorkerList()
        {
            var workers = (from user in _db.Users
                           join userRoles in _db.UserRoles on user.Id equals userRoles.UserId
                           join roles in _db.Roles.Where(x => x.Name == RoleNames.Role_AppWorker)
                           on userRoles.RoleId equals roles.Id
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

        public AppWorkerViewModel GetCurrentUser(string id = "")
        {
            AppWorkerViewModel model = new AppWorkerViewModel();
            if (string.IsNullOrWhiteSpace(id))
            {
                var currentLogin = _context.HttpContext.User.Claims.ToList();
                string userId = _context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var user = _userManager.GetUserAsync(_context.HttpContext.User);
                int vacs = user.Result.VacationDays;

                model.Name = currentLogin.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
                model.Id = currentLogin.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                model.VacationDays = vacs;
            }
            else
            {
                var user = _userManager.FindByIdAsync(id);
                model.Name = user.Result.Name;
                model.Id = id;
                model.VacationDays = user.Result.VacationDays;
            }

            return model;
        }
        private int GetVacationsByMonth(string appUserId, int month)
        {
            var daysTaken = _db.Vacations.Where(x => x.AppWorkerId == appUserId)
                .Where(x => x.StartDate.Month == month || x.EndDate.Month == month)
                .ToList();

            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            TimeSpan diffResult;
            List<DateTime> daysList = new List<DateTime>();
            foreach (var day in daysTaken)
            {
                diffResult = day.EndDate - day.StartDate;
                for (int i = 0; i < diffResult.Days; i++)
                {
                    if (day.StartDate.Day != daysInMonth)
                        daysList.Add(day.StartDate.AddDays(i));
                }
            }
            return daysList.Count();
        }

        public List<VacationViewModel> VacationsEventById(string workerId, int month)
        {
            if (workerId == null)
                workerId = _context.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            GetVacationsByMonth(workerId, month);

            return _db.Vacations.Include(x => x.VacationStatus)
                .Where(x => x.AppWorkerId == workerId)
                .Where(x => x.StartDate.Month == month)
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

        public List<VacationViewModel> GetAllVacations()
        {
            return _db.Vacations.Include(x => x.VacationStatus)
                .Include(x => _db.Users.SingleOrDefault(c => c.Id == x.AppWorkerId))
                .Select(c => new VacationViewModel()
                {
                    Id = c.Id,
                    Description = c.Description,
                    StartDate = c.StartDate.ToString("yyyy-MM,dd"),
                    EndDate = c.EndDate.ToString("yyyy-MM,dd"),
                    Title = c.AppWorkerId,
                    Duration = c.Duration,
                    IsApproved = c.IsApproved,
                    IsRejected = c.IsRejected,
                    AppWorkerId = c.AppWorkerId,
                    StatusText = c.VacationStatus.StatusText
                }).ToList();
        }

        public async Task<int> DeleteEvent(int id)
        {
            var vacation = _db.Vacations.FirstOrDefault(x => x.Id == id);
            if (vacation != null)
            {
                var appUser = _db.Users.FirstOrDefault(x => x.Id == vacation.AppWorkerId);
                appUser.VacationDaysTaken -= vacation.Duration;
                appUser.VacationDays += vacation.Duration;

                _db.Update(appUser);
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
                var vacationStatus = _db.VacationStatus
                    .FirstOrDefault(x => x.Id == (int)EnumVacationStatus.Accepted);
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
                    var vacationStatus = _db.VacationStatus.FirstOrDefault(x => x.Id == (int)EnumVacationStatus.Rejected);
                    vacation.IsRejected = true;
                    vacation.VacationStatus = vacationStatus;
                    user.VacationDays += vacation.Duration;
                    user.VacationDaysTaken -= vacation.Duration;

                    _db.Update(user);
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

            Vacation isEventOverlap = _db.Vacations.Where(x => x.AppWorkerId == workerId)
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
