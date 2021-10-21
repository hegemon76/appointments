using appointments.Services;
using Microsoft.AspNetCore.Mvc;
using appointments.Helper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace appointments.Controllers
{
    public class VacationController : Controller
    {
        private readonly IVacationService _appointmentService;
        public VacationController(IVacationService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole(Helper.Helper.Admin))
            {
                ViewBag.WorkerList = _appointmentService.GetWorkerList();
            }
            else
            {
                ViewBag.CurrentUser = _appointmentService.GetCurrentUser();
            }
            return View();
        }
    }
}
