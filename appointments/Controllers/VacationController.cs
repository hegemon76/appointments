using appointments.Services;
using Microsoft.AspNetCore.Mvc;
using vacations.Models.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace appointments.Controllers
{
    public class VacationController : Controller
    {
        private readonly IVacationService _vacationService;
        private readonly IHttpContextAccessor _context;
        public VacationController(IVacationService vacationService, IHttpContextAccessor context)
        {
            _vacationService = vacationService;
            _context = context;
        }

        [Authorize(Roles = "Admin, AppWorker")]
        public IActionResult Index()
        {
            if (User.IsInRole(RoleNames.Role_Admin))
            {
                ViewBag.WorkerList = _vacationService.GetWorkerList();
            }
            else
            {
                var currentUser = _vacationService.GetCurrentUser();

                ViewBag.CurrentUser = currentUser;
            }
            return View();
        }
         
        public IActionResult AllWorkerVacation()
        {
            ViewBag.WorkerList = _vacationService.GetWorkerList();
            return View();
        }
    }
}
