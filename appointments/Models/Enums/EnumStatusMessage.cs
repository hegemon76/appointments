namespace vacations.Models.Enums
{
    public enum EnumStatusMessage
    {
        failure_code = 0,
        success_code = 1,
        VacationAdded,
        VacationUpdated,
        VacationDeleted,
        VacationExists,
        VacationConfirmed,
        VacationRejected,
        VacationNotExists = -1,
        SomethingWentWrong =-2,
        OperationNotAllowed=-3,
        OverlapDates=-4,
        MinimumDate=-5
    }
}
