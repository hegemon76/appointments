using appointments.Services;
using Microsoft.AspNetCore.Mvc;

namespace appointments.Controllers
{
    public class VacationController : Controller
    {
        private readonly IVacationService _appointmentService;
        public VacationController(IVacationService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        public IActionResult Index()
        {
           ViewBag.WorkerList = _appointmentService.GetWorkerList();
            return View();
        }
    }
}
