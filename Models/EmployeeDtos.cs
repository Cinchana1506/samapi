namespace EmployeeConfirmationApi.Models
{
    public class EmployeeDto
    {
        public int EmpId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
    }
}
