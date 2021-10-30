using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using vacations.Models.Helper;

namespace appointments.Helper
{
    public static class Helper
    {
        //public static string Admin = "Admin";
        //public static string AppWorker = "AppWorker";
        //public static string vacationAdded = "Urlop dodany poprawnie.";
        //public static string vacationUpdated = "Urlop zmodyfikowany poprawnie.";
        //public static string vacationDeleted = "Urlop usunięty poprawnie.";
        //public static string vacationExists = "Dla wybranych dat urlop juz istnieje.";
        //public static string vacationNotExists = "Urlop nie istnieje.";
        //public static string somethingWentWrong = "Ups. Cos poszło nie tak.";
        //public static string vacationConfirmed = "Urlop zatwierdzony.";
        //public static string operationNotAllowed = "Operacja nie jest dozwolona.";
        //public static string overlapDates = "W wybranym okresie istnieje już urlop.";
        //public static string minimumDate = "Wybrana data musi być minimum dzisiejsza.";

        //public static int success_code = 1;
        //public static int failure_code = 0;

        public static List<SelectListItem> GetRolesFromDropdown()
        {
            return new List<SelectListItem>
            {
                new SelectListItem{Value=RoleNames.Role_Admin,Text=RoleNames.Role_Admin},
                new SelectListItem{Value=RoleNames.Role_AppWorker,Text=RoleNames.Role_AppWorker}
            };
        }
    }
}
