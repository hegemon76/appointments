﻿using appointments.Data;
using appointments.Models;
using appointments.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appointments.Services
{
    public class VacationService : IVacationService
    {
        private readonly ApplicationDbContext _db;
        public VacationService(ApplicationDbContext db)
        {
            _db = db;
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
                    Duration=model.Duration,
                    IsApproved = model.IsApproved,
                    AppWorkerId=model.AppWorkerId,
                    AdminId=model.AdminId
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

        public List<Vacation> VacationsEventById(string workerId)
        {
           return _db.Vacations.Where(x => x.AppWorkerId == workerId).ToList().Select(c =>)
        }
    }
}
