namespace EmployeeConfirmationApi.Models
{
    public class ValidateSubmissionDto
    {
        public int MasterId { get; set; }    // @ECID
        public int EmpId { get; set; }       // @MEMPID
        public DateTime DateOfJoining { get; set; }
        public string Action { get; set; } = string.Empty;   // Confirm / Extend / Not confirmed
        public int ExtensionMonths { get; set; } = 0; // optional
        public string? Remarks { get; set; }   // nullable to remove warnings
    }
}
