using EmployeeConfirmationApi.Models;

namespace EmployeeConfirmationApi.Services
{
    public class MockEmpConfirmationService : IEmpConfirmationService
    {
        public Task<IReadOnlyList<EmployeeDto>> GetEmployeesForConfirmationAsync(CancellationToken ct)
        {
            var list = new List<EmployeeDto>
            {
                new() { EmpId = 1001, Name = "Sample One", Designation = "Engineer", Manager = "Manager A", JoiningDate = DateTime.UtcNow.AddYears(-1) },
                new() { EmpId = 1002, Name = "Sample Two", Designation = "Analyst",  Manager = "Manager B", JoiningDate = DateTime.UtcNow.AddYears(-2) }
            };
            return Task.FromResult<IReadOnlyList<EmployeeDto>>(list);
        }

        public Task<int?> InsertSchedularDetailsAsync(InsertScheduleDto dto, CancellationToken ct)
        {
            var ecid = Random.Shared.Next(10000, 99999); // simulate ECID
            return Task.FromResult<int?>(ecid);
        }
        public Task<object> GetExtensionDetailsAsync(int empId, int instanceId, CancellationToken ct)
{
    return Task.FromResult<object>(new {
        EmpId = empId,
        InstanceId = instanceId,
        ExtensionMonths = 6,
        Reason = "Performance improvement",
        ApprovedBy = "Mock Manager"
    });
}

public Task<object> UploadAttachmentAsync(IFormFile file, int empId, int instanceId, CancellationToken ct)
{
    return Task.FromResult<object>(new {
        Success = true,
        Message = "Mock upload successful",
        FileName = file?.FileName ?? "mockfile.pdf",
        Path = "/mock/path/mockfile.pdf",
        EmpId = empId,
        InstanceId = instanceId
    });
}

        public Task<IEnumerable<object>> GetAttachmentsAsync(int empId, int instanceId, CancellationToken ct)
        {
            var mockAttachments = new List<object>
    {
        new { EmpId = empId, InstanceId = instanceId, FileName = "mock_report.pdf", FilePath = "/mock/path/mock_report.pdf" },
        new { EmpId = empId, InstanceId = instanceId, FileName = "mock_notes.docx", FilePath = "/mock/path/mock_notes.docx" }
    };
            return Task.FromResult<IEnumerable<object>>(mockAttachments);
        }
public Task<object> ValidateSubmissionAsync(ValidateSubmissionDto dto, CancellationToken ct)
{
    return Task.FromResult<object>(new {
        Success = true,
        Message = "Mock submission validated and saved.",
        MasterId = dto.MasterId,
        EmpId = dto.EmpId,
        Action = dto.Action,
        ConfirmationDate = dto.DateOfJoining.AddMonths(3),
        ExtensionApplied = dto.Action == "Extend" ? dto.ExtensionMonths : 0,
        Remarks = dto.Remarks
    });
}

        public Task<bool> SaveRMEvaluationFeedbackAsync(IEnumerable<RMEvaluationDto> feedbacks, CancellationToken ct)
        {
            // Just simulate success
            return Task.FromResult(true);
        }
public Task<bool> SaveHeadEvaluationFeedbackAsync(IEnumerable<HeadEvaluationDto> feedbacks, CancellationToken ct)
{
    // Simulate success
    return Task.FromResult(true);
}

public Task<bool> SaveHeadConfirmationAsync(HeadConfirmationDto dto, CancellationToken ct)
{
    // Simulate success
    return Task.FromResult(true);
}


    }
}
