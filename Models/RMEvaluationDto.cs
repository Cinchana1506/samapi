namespace EmployeeConfirmationApi.Models
{
    public class RMEvaluationDto
    {
        public int MasterId { get; set; }
        public int ParamId { get; set; }
        public int RMRating { get; set; }
        public string RMRemarks { get; set; } = string.Empty;
    }
}
