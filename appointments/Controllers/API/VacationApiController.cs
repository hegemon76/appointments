using appointments.Models.ViewModels;
using appointments.Services;
using Microsoft.AspNetCore.Http;
using appointments.Helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace appointments.Controllers.API
{
    [Route("api/Vacation")]
    [ApiController]
    public class VacationApiController : Controller
    {
        private readonly IVacationService _vacationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string loginUserId;
        private readonly string role;
        public VacationApiController(IVacationService vacationService, IHttpContextAccessor httpContextAccessor)
        {
            _vacationService = vacationService;
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
                commonResponse.status = _vacationService.AddUpdate(data).Result;
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

        [HttpGet]
        [Route("GetCalendarData")]
        public IActionResult GetCalendarData(string workerId)
        {
            CommonResponse<List<VacationViewModel>> commonResponse = new CommonResponse<List<VacationViewModel>>();
            try
            {
                if (role == Helper.Helper.AppWorker)
                {
                    commonResponse.dataenum = _vacationService.VacationsEventById(loginUserId);
                    commonResponse.status = Helper.Helper.success_code;
                }
                else
                {
                    commonResponse.dataenum = _vacationService.VacationsEventById(workerId);
                    commonResponse.status = Helper.Helper.success_code;
                }
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = Helper.Helper.failure_code;
            }
            return Ok(commonResponse);
        }
    }
}
