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
using vacations.Models.ViewModels;

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
            if (isOverlapWithOtherEvent(model.AppWorkerId, model.StartDate, model.EndDate))
                return (int)EnumStatusMessage.OverlapDates;

            if (!isMinimumDateToday(model.StartDate))
                return (int)EnumStatusMessage.MinimumDate;

            var startDate = DateTime.Parse(model.StartDate);
            var endDate = DateTime.Parse(model.EndDate);

            int duration = evaluateDuration(startDate, endDate);

            VacationStatus vacationStatus = _db.VacationStatus
                                            .FirstOrDefault(x => x.Id == (int)EnumVacationStatus.Pending);
            //create
            Vacation vacation = new Vacation()
            {
                Title = model.Title,
                Description = model.Description,
                StartDate = startDate,
                EndDate = endDate,
                Duration = duration,
                IsApproved = false,
                IsRejected = false,
                AppWorkerId = model.AppWorkerId,
                AdminId = model.AdminId,
                VacationStatus = vacationStatus
            };

            var user = _db.Users.FirstOrDefault(x => x.Id == model.AppWorkerId);
            if (model.AppWorkerId == null)
                user = await _userManager.GetUserAsync(_context.HttpContext.User);

            user.VacationDays -= duration;
            _db.Users.FirstOrDefault(x => x.Id == user.Id).VacationDays = user.VacationDays;
            _db.Vacations.Add(vacation);
            await _db.SaveChangesAsync();
            return (int)EnumStatusMessage.VacationAdded;
        }

        private async Task<int> UpdateEvent(VacationViewModel model)
        {
            var startDate = DateTime.Parse(model.StartDate);
            var endDate = DateTime.Parse(model.EndDate);

            var duration = evaluateDuration(startDate, endDate);
            //update
            var vacation = _db.Vacations.FirstOrDefault(x => x.Id == model.Id);
            if (!vacation.IsApproved && !vacation.IsRejected)
            {
                vacation.Title = model.Title;
                vacation.Description = model.Description;
                vacation.StartDate = startDate;
                vacation.EndDate = endDate;
                vacation.Duration = duration;
                vacation.IsApproved = model.IsApproved;
                vacation.AppWorkerId = model.AppWorkerId;
                vacation.AdminId = model.AdminId;
                await _db.SaveChangesAsync();
                return (int)EnumStatusMessage.VacationUpdated;
            }
            return (int)EnumStatusMessage.OperationNotAllowed;
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
                               Name = user.Name
                           }
                          ).ToList();

            return workers;
        }

        public AppWorkerViewModel GetCurrentUser()
        {
            var currentLogin = _context.HttpContext.User.Claims.ToList();
            string userId = _context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _userManager.GetUserAsync(_context.HttpContext.User);

            int vacs = user.Result.VacationDays;

            //var vacsDays = GetVacationsByMonth(userId, month);

            AppWorkerViewModel model = new AppWorkerViewModel()
            {
                Name = currentLogin.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value,
                Id = currentLogin.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value,
                VacationDays = vacs
            };
            return model;
        }
        public VacationViewModel GetVacationsDaysByMonth(string appUserId, int month)
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
            return new VacationViewModel { DaysTakenInMonth = daysInMonth };
        }

        public List<VacationViewModel> VacationsEventById(string workerId, int month)
        {
            if (workerId == null)
                workerId = _context.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            return _db.Vacations.Include(x => x.VacationStatus)
                .Where(x => x.AppWorkerId == workerId)
                .Where(x => (x.StartDate.Month == month) || (x.EndDate.Month == month))
                 .Select(c => new VacationViewModel()
                 {
                     Id = c.Id,
                     Description = c.Description,
                     StartDate = c.StartDate.ToString("yyyy-MM-dd"),
                     EndDate = c.EndDate.ToString("yyyy-MM-dd"),
                     Title = c.Title,
                     Duration = c.Duration,
                     IsApproved = c.IsApproved,
                     IsRejected = c.IsRejected,
                     StatusText = c.VacationStatus.StatusText,
                 }).ToList();
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

        public async Task<int> DeleteEvent(int eventId)
        {
            var vacation = _db.Vacations.FirstOrDefault(x => x.Id == eventId);
            if (vacation != null)
            {
                var user = _db.Users.FirstOrDefault(x => x.Id == vacation.AppWorkerId);
                user.VacationDays += vacation.Duration;

                _db.Vacations.Remove(vacation);
                _db.Update(user);

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
                var user = _db.Users.FirstOrDefault(x => x.Id == vacation.AppWorkerId);
                if (!vacation.IsApproved)
                {
                    var vacationStatus = _db.VacationStatus.FirstOrDefault(x => x.Id == (int)EnumVacationStatus.Rejected);
                    vacation.IsRejected = true;
                    vacation.VacationStatus = vacationStatus;
                    user.VacationDays += vacation.Duration;
                    _db.Update(user);
                    await _db.SaveChangesAsync();
                    return (int)EnumStatusMessage.VacationRejected;
                }
                return (int)EnumStatusMessage.OperationNotAllowed;
            }
            return (int)EnumStatusMessage.failure_code;
        }

        public VacationsDaysInfoVm GetVacationsDaysInfo(string userId, int month)
        {
            var user = _db.Users
                .FirstOrDefault(x => x.Id == userId);

            var vacations = _db.Vacations
                .Where(x => x.AppWorkerId == user.Id)
                .Where(z => z.IsRejected != true)
                .Where(c => (c.StartDate.Month == month) || (c.EndDate.Month == month));

            int takenInMonth = 0;
            foreach (var item in vacations)
            {
                takenInMonth += evaluateDuration(item.StartDate, item.EndDate, month);
            }

            return new VacationsDaysInfoVm()
            {
                VacationsDaysLeft = user.VacationDays,
                VacationsDaysTaken = takenInMonth
            };
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

        private int evaluateDuration(DateTime startDate, DateTime endDate, int month = 0)
        {
            var daysList = Enumerable.Range(0, 1 + endDate.Date
                .Subtract(startDate).Days)
                .Select(offset => startDate.Date.AddDays(offset))
                .ToList();

            int duration = 0;
            if (month != 0)
                daysList = daysList.Where(x => x.Month == month).ToList();

            foreach (var item in daysList)
            {
                if (item.DayOfWeek != DayOfWeek.Saturday && item.DayOfWeek != DayOfWeek.Sunday)
                    duration += 1;
            }

            return duration;
        }
    }

}
