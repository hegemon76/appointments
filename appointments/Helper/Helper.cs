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
        public static string AppWorker = "AppWorker";
        public static string vacationAdded = "Urlop dodany poprawnie.";
        public static string vacationUpdated = "Urlop zmodyfikowany poprawnie.";
        public static string vacationDeleted = "Urlop usunięty poprawnie.";
        public static string vacationExists = "Dla wybranych dat urlop juz istnieje.";
        public static string vacationNotExists = "Urlop nie istnieje.";
        public static int success_code = 1;
        public static int failure_code = 0;

        public static List<SelectListItem> GetRolesFromDropdown()
        {
            return new List<SelectListItem>
            {
                new SelectListItem{Value=Helper.Admin,Text=Helper.Admin},
                new SelectListItem{Value=Helper.AppWorker,Text=Helper.AppWorker}
            };
        }

    }
}
