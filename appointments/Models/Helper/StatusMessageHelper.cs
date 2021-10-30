using vacations.Models.Enums;

namespace vacations.Models.Helper
{
    public class StatusMessageHelper
    {
        public string GetStatusMessage(int statusCode)
        {
            EnumStatusMessage message = (EnumStatusMessage)statusCode;
            switch (message)
            {
                case EnumStatusMessage.VacationAdded:
                    return "Urlop dodany poprawnie.";
                case EnumStatusMessage.MinimumDate:
                    return "Wybrana data musi być minimum dzisiejsza.";
                case EnumStatusMessage.OperationNotAllowed:
                    return "Operacja nie jest dozwolona.";
                case EnumStatusMessage.OverlapDates:
                    return "W wybranym okresie istnieje już urlop.";
                case EnumStatusMessage.SomethingWentWrong:
                    return "Ups. Cos poszło nie tak.";
                case EnumStatusMessage.VacationConfirmed:
                    return "Urlop zatwierdzony.";
                case EnumStatusMessage.VacationDeleted:
                    return "Urlop usunięty poprawnie.";
                case EnumStatusMessage.VacationExists:
                    return "Dla wybranych dat urlop juz istnieje.";
                case EnumStatusMessage.VacationNotExists:
                    return "Urlop nie istnieje.";
                case EnumStatusMessage.VacationUpdated:
                    return "Urlop zmodyfikowany poprawnie.";
                case EnumStatusMessage.VacationRejected:
                    return "Urlop odrzucony.";
                default:
                    return "Ups. Cos poszło nie tak.";
            }
        }
    }
}
