namespace EmployeeConfirmationApi.Models
{
    public class HeadConfirmationDto
    {
        public int InstanceId { get; set; }
        public string IsForConfirmationGH { get; set; } = string.Empty; // Confirm/Extend/NotJoin
        public string? GHJustificationORReason { get; set; }
        public int Extension { get; set; } = 0; // default 0, if extend set 1
    }
}
