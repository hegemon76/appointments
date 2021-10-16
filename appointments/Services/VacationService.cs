using appointments.Data;
using appointments.Models;
using appointments.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace appointments.Services
{
    public class VacationService : IVacationService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
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

            if (model != null && model.Id > 0)
            {
                //update
                return 1;
            }
            else
            {
                //create
                Vacation vacation = new Vacation()
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = startDate,
                    EndDate = endDate,
                    Duration = model.Duration,
                    IsApproved = model.IsApproved,
                    AppWorkerId = model.AppWorkerId,
                    AdminId = model.AdminId
                };

                _db.Vacations.Add(vacation);
                await _db.SaveChangesAsync();
                return 2;
            }

        }

        public List<AppWorkerViewModel> GetWorkerList()
        {
            var workers = (from user in _db.Users
                           join userRoles in _db.UserRoles on user.Id equals userRoles.UserId
                           join roles in _db.Roles.Where(x => x.Name == Helper.Helper.AppWorker) on userRoles.RoleId equals roles.Id
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
            AppWorkerViewModel model = new AppWorkerViewModel()
            {
                Name = currentLogin.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value,
                Id = currentLogin.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value
            };
            return model;
        }

        public List<VacationViewModel> VacationsEventById(string workerId)
        {
            return _db.Vacations.Where(x => x.AppWorkerId == workerId).ToList().Select(c => new VacationViewModel()
            {
                Id = c.Id,
                Description = c.Description,
                StartDate = c.StartDate.ToString("yyyy-MM,dd"),
                EndDate = c.EndDate.ToString("yyyy-MM,dd"),
                Title = c.Title,
                Duration = c.Duration,
                IsApproved = c.IsApproved
            }).ToList();
        }

        public VacationViewModel GetById(int id)
        {
            return _db.Vacations.Where(x => x.Id == id).ToList().Select(c => new VacationViewModel()
            {
                Id = c.Id,
                Description = c.Description,
                StartDate = c.StartDate.ToString("yyyy-MM,dd"),
                EndDate = c.EndDate.ToString("yyyy-MM,dd"),
                Title = c.Title,
                Duration = c.Duration,
                IsApproved = c.IsApproved,
                AppWorkerId = c.AppWorkerId
            }).SingleOrDefault();
        }

        public async Task<int> DeleteEvent(int id)
        {
            var vacation = _db.Vacations.FirstOrDefault(x => x.Id == id);
            if (vacation != null)
            {
                _db.Vacations.Remove(vacation);
                return await _db.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> ConfirmEvent(int id)
        {
            var vacation = _db.Vacations.FirstOrDefault(x => x.Id == id);
            if (vacation != null)
            {
                vacation.IsApproved = true;
                return await _db.SaveChangesAsync();
            }
            return 0;
        }
    }

}
