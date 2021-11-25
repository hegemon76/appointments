using appointments.Models.ViewModels;
using appointments.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using vacations.Models.Enums;
using vacations.Models.Helper;

namespace appointments.Controllers.API
{
    [Route("api/Vacation")]
    [ApiController]
    public class VacationApiController : Controller
    {
        private readonly IVacationService _vacationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StatusMessageHelper _messageHelper;
        private readonly string loginUserId;
        private readonly string role;
        public VacationApiController(IVacationService vacationService, IHttpContextAccessor httpContextAccessor,
            StatusMessageHelper messageHelper)
        {
            _vacationService = vacationService;
            _httpContextAccessor = httpContextAccessor;
            _messageHelper = messageHelper;
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
                commonResponse.message = _messageHelper.GetStatusMessage(commonResponse.status);
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = (int)EnumStatusMessage.failure_code;
                throw;
            }
            return Ok(commonResponse);
        }

        [HttpGet]
        [Route("GetCalendarData")]
        public IActionResult GetCalendarData(string workerId, int month)
        {
            CommonResponse<List<VacationViewModel>> commonResponse = new CommonResponse<List<VacationViewModel>>();
            try
            {
                if (role == RoleNames.Role_AppWorker)
                {
                    commonResponse.dataenum = _vacationService.VacationsEventById(loginUserId, month);
                    commonResponse.status = (int)EnumStatusMessage.success_code;
                }
                else
                {
                    commonResponse.dataenum = _vacationService.VacationsEventById(workerId, month);
                    commonResponse.status = (int)EnumStatusMessage.success_code;
                }
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = (int)EnumStatusMessage.failure_code;
            }
            return Ok(commonResponse);
        }

        [HttpGet]
        [Route("GetAllWorkersData")]
        public IActionResult GetAllWorkersData()
        {
            CommonResponse<List<VacationViewModel>> commonResponse = new CommonResponse<List<VacationViewModel>>();
            try
            {
                    commonResponse.dataenum = _vacationService.GetAllVacations();
                    commonResponse.status = (int)EnumStatusMessage.success_code;
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = (int)EnumStatusMessage.failure_code;
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
                commonResponse.status = (int)EnumStatusMessage.success_code;
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = (int)EnumStatusMessage.failure_code;
            }
            return Ok(commonResponse);
        }

        [HttpGet]
        [Route("GetCurrentUser/{id}")]
        public IActionResult GetCurrentUser(string id = "")
        {
            CommonResponse<AppWorkerViewModel> commonResponse = new CommonResponse<AppWorkerViewModel>();
            try
            {
                commonResponse.dataenum = _vacationService.GetCurrentUser(id);
                commonResponse.status = (int)EnumStatusMessage.success_code;
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = (int)EnumStatusMessage.failure_code;
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
                commonResponse.message = _messageHelper.GetStatusMessage(commonResponse.status);
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = (int)EnumStatusMessage.failure_code;
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
                commonResponse.status = result;
                commonResponse.message = _messageHelper.GetStatusMessage(commonResponse.status);
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = (int)EnumStatusMessage.failure_code;
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
                commonResponse.status = await _vacationService.RejectEvent(id);
                commonResponse.message = _messageHelper.GetStatusMessage(commonResponse.status);

            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = (int)EnumStatusMessage.failure_code;
            }
            return Ok(commonResponse);
        }
    }
}
