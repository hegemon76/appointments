using appointments.Models.ViewModels;
using appointments.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
                if (data.AppWorkerId == null)
                    data.AppWorkerId = loginUserId;

                commonResponse.status = _vacationService.AddUpdate(data).Result;
                if (commonResponse.status == 1)
                    commonResponse.message = Helper.Helper.vacationUpdated;

                if (commonResponse.status == 2)
                    commonResponse.message = Helper.Helper.vacationAdded;

                if (commonResponse.status == 3)
                    commonResponse.message = Helper.Helper.operationNotAllowed;
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


        [HttpGet]
        [Route("GetCalendarDataById/{id}")]
        public IActionResult GetCalendarDataById(int id)
        {
            CommonResponse<VacationViewModel> commonResponse = new CommonResponse<VacationViewModel>();
            try
            {

                commonResponse.dataenum = _vacationService.GetById(id);
                commonResponse.status = Helper.Helper.success_code;

            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = Helper.Helper.failure_code;
            }
            return Ok(commonResponse);
        }

        [HttpGet]
        [Route("DeleteVacation/{id}")]
        public async Task<IActionResult> DeleteVacation(int id)
        {
            CommonResponse<int> commonResponse = new CommonResponse<int>();
            try
            {
                commonResponse.status = await _vacationService.DeleteEvent(id);
                commonResponse.message = commonResponse.status == 1
                    ? Helper.Helper.vacationDeleted : Helper.Helper.somethingWentWrong;
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = Helper.Helper.failure_code;
            }
            return Ok(commonResponse);
        }

        [HttpGet]
        [Route("ConfirmEvent/{id}")]
        public async Task<IActionResult> ConfirmEvent(int id)
        {
            CommonResponse<int> commonResponse = new CommonResponse<int>();
            try
            {
                var result = await _vacationService.ConfirmEvent(id);
                if (result > 0)
                {
                    commonResponse.status = Helper.Helper.success_code;
                    commonResponse.message = Helper.Helper.vacationConfirmed;
                }
                else
                {
                    commonResponse.status = Helper.Helper.failure_code;
                    commonResponse.message = Helper.Helper.somethingWentWrong;
                }
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = Helper.Helper.failure_code;
            }
            return Ok(commonResponse);
        }

        [HttpGet]
        [Route("RejectEvent/{id}")]
        public async Task<IActionResult> RejectEvent(int id)
        {
            CommonResponse<int> commonResponse = new CommonResponse<int>();
            try
            {
                var result = await _vacationService.RejectEvent(id);
                if (result == 1)
                {
                    commonResponse.status = Helper.Helper.success_code;
                    commonResponse.message = Helper.Helper.vacationConfirmed;
                }
                else if(result == 4)
                {
                    commonResponse.status = Helper.Helper.failure_code;
                    commonResponse.message = Helper.Helper.operationNotAllowed;
                }
                else
                {
                    commonResponse.status = Helper.Helper.failure_code;
                    commonResponse.message = Helper.Helper.somethingWentWrong;
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
