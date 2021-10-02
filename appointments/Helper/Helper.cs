using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appointments.Helper
{
    public static class Helper
    {
        public static string Admin = "Admin";
        public static string AppointmentOwner = "appointmentOwner";
        public static string AppointmentClient = "appointmentClient";

        public static List<SelectListItem> GetRolesFromDropdown()
        {
            return new List<SelectListItem>
            {
                new SelectListItem{Value=Helper.Admin,Text=Helper.Admin},
                new SelectListItem{Value=Helper.AppointmentClient,Text=Helper.AppointmentClient},
                new SelectListItem{Value=Helper.AppointmentOwner,Text=Helper.AppointmentOwner}
            };
        }
    }
}
