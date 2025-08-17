namespace EmployeeConfirmationApi.Models
{
    public class InsertScheduleDto
    {
        public int EmpId { get; set; }           // @MEmpID
        public int MajorProjectId { get; set; }  // @MProjectID
        public DateTime ConfirmDate { get; set; } // @ConfirmDate
    }
}
