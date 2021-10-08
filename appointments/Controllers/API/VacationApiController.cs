using appointments.Models.ViewModels;
using appointments.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace appointments.Controllers.API
{
    [Route("api/Vacation")]
    [ApiController]
    public class VacationApiController : Controller
    {
        private readonly IVacationService _appointmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string loginUserId;
        private readonly string role;
        public VacationApiController(IVacationService appointmentService, IHttpContextAccessor httpContextAccessor)
        {
            _appointmentService = appointmentService;
            _httpContextAccessor = httpContextAccessor;
            loginUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
        }

        [HttpPost]
        [Route("SaveCalendarData")]
        public IActionResult SaveCalendarData(VacationViewModel data)
        {
            CommonResponse<int> commonResponse = new CommonResponse<int>();
            try
            {
                commonResponse.status = _appointmentService.AddUpdate(data).Result;
                if (commonResponse.status == 1)
                    commonResponse.message = Helper.Helper.vacationUpdated;

                if (commonResponse.status == 2)
                    commonResponse.message = Helper.Helper.vacationAdded;
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = Helper.Helper.failure_code;
                throw;
            }
            return Ok(commonResponse);
        }
    }
}
