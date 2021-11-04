using appointments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using vacations.Models.Helper;

namespace appointments.Controllers
{
    public class VacationController : Controller
    {
        private readonly IVacationService _appointmentService;
        public VacationController(IVacationService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [Authorize(Roles = "Admin, AppWorker")]
        public IActionResult Index()
        {
            if (User.IsInRole(RoleNames.Role_Admin))
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
