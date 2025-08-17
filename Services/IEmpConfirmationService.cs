using EmployeeConfirmationApi.Models;

namespace EmployeeConfirmationApi.Services
{
    public interface IEmpConfirmationService
    {
        Task<IReadOnlyList<EmployeeDto>> GetEmployeesForConfirmationAsync(CancellationToken ct);
        Task<int?> InsertSchedularDetailsAsync(InsertScheduleDto dto, CancellationToken ct);
        Task<object> GetExtensionDetailsAsync(int empId, int instanceId, CancellationToken ct);
        Task<object> UploadAttachmentAsync(IFormFile file, int empId, int instanceId, CancellationToken ct);
        Task<IEnumerable<object>> GetAttachmentsAsync(int empId, int instanceId, CancellationToken ct);
        Task<object> ValidateSubmissionAsync(ValidateSubmissionDto dto, CancellationToken ct);
        Task<bool> SaveRMEvaluationFeedbackAsync(IEnumerable<RMEvaluationDto> feedbacks, CancellationToken ct);
        Task<bool> SaveHeadEvaluationFeedbackAsync(IEnumerable<HeadEvaluationDto> feedbacks, CancellationToken ct);
        Task<bool> SaveHeadConfirmationAsync(HeadConfirmationDto dto, CancellationToken ct);


    }
}
