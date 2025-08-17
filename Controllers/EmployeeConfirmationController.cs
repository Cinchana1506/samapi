using Microsoft.AspNetCore.Mvc;
using EmployeeConfirmationApi.Models;
using EmployeeConfirmationApi.Services;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http; 
using System.IO;  // <-- needed for FileStream, Path, Directory
using System.Collections.Generic; // <-- needed for List<T>


namespace EmployeeConfirmationApi.Controllers
{
    [ApiController]
    [Route("api/employee-confirmation")]
    public class EmployeeConfirmationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IEmpConfirmationService _svc;

        public EmployeeConfirmationController(IEmpConfirmationService svc, IConfiguration configuration)
        {
            _svc = svc;
            _configuration = configuration;
        }


        /// <summary>Get list of employees to trigger workflow.</summary>
        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees(CancellationToken ct)
            => Ok(await _svc.GetEmployeesForConfirmationAsync(ct));

        /// <summary>Insert employee details before triggering workflow.</summary>
        [HttpPost("schedule")]
        public async Task<ActionResult<object>> InsertSchedularDetails([FromBody] InsertScheduleDto dto, CancellationToken ct)
        {
            var masterId = await _svc.InsertSchedularDetailsAsync(dto, ct);
            return Ok(new { message = "Inserted successfully", masterId });
        }
        // 3. Get Confirmation Details by MasterID (for RM Approval)
        [HttpGet("GetConfirmationDetailsByMasterId/{masterId}")]
        public IActionResult GetConfirmationDetailsByMasterId(int masterId)
        {
            // TODO: Call SP: EmpConfirmation_GetDetailsByMasterID
            // Pass parameter: @ECID = masterId
            return Ok(new
            {
                MasterId = masterId,
                Status = "Confirmed", // mock
                Evaluation = new { Score = 85, Comments = "Good performance" }
            });
        }

        // 4. Get Employee Details by ID
        [HttpGet("GetEmployeeById/{empId}")]
        public IActionResult GetEmployeeById(int empId)
        {
            // TODO: Call DB to get details for employee
            return Ok(new
            {
                EmpId = empId,
                Name = "John Doe",
                Designation = "Engineer",
                JoiningDate = DateTime.Now.AddYears(-1)
            });
        }

        // 5. Load Projects
        [HttpGet("GetProjects")]
        public IActionResult GetProjects()
        {
            // TODO: Call DB (master_projects table)
            var projects = new List<object>
    {
        new { ProjectId = 1, Name = "Project Alpha", Status = "Live", Year = 2024 },
        new { ProjectId = 2, Name = "Project Beta", Status = "Completed", Year = 2023 }
    };
            return Ok(projects);
        }
        [HttpGet("GetExtensionDetails")]
        public async Task<IActionResult> GetExtensionDetails(int empId, int instanceId, CancellationToken ct)
            => Ok(await _svc.GetExtensionDetailsAsync(empId, instanceId, ct));

        [HttpPost("UploadAttachment")]
        public async Task<IActionResult> UploadAttachment([FromForm] IFormFile file, int empId, int instanceId, CancellationToken ct)
            => Ok(await _svc.UploadAttachmentAsync(file, empId, instanceId, ct));

        [HttpGet("GetAttachments")]
        public async Task<IActionResult> GetAttachments(int empId, int instanceId, CancellationToken ct)
            => Ok(await _svc.GetAttachmentsAsync(empId, instanceId, ct));
        [HttpPost("ValidateSubmission")]
        public async Task<IActionResult> ValidateSubmission([FromBody] ValidateSubmissionDto dto, CancellationToken ct)
        => Ok(await _svc.ValidateSubmissionAsync(dto, ct));
        [HttpPost("SaveRMEvaluationFeedback")]
    public async Task<IActionResult> SaveRMEvaluationFeedback([FromBody] List<RMEvaluationDto> feedbacks, CancellationToken ct)
{
    if (feedbacks == null || feedbacks.Count == 0)
        return BadRequest("No feedback provided.");

    var result = await _svc.SaveRMEvaluationFeedbackAsync(feedbacks, ct);
    return Ok(new { Success = result });
}

[HttpPost("SaveHeadEvaluationFeedback")]
public async Task<IActionResult> SaveHeadEvaluationFeedback([FromBody] IEnumerable<HeadEvaluationDto> feedbacks, CancellationToken ct)
{
    var result = await _svc.SaveHeadEvaluationFeedbackAsync(feedbacks, ct);
    return Ok(new { Success = result });
}

[HttpPost("SaveHeadConfirmation")]
public async Task<IActionResult> SaveHeadConfirmation([FromBody] HeadConfirmationDto dto, CancellationToken ct)
{
    var result = await _svc.SaveHeadConfirmationAsync(dto, ct);
    return Ok(new { Success = result });
}
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

        // -------------------------
        // NEW MOCK IMPLEMENTATIONS
        // -------------------------

        public Task<object> GetExtensionDetailsAsync(int empId, int instanceId, CancellationToken ct)
        {
            return Task.FromResult<object>(new
            {
                EmpId = empId,
                InstanceId = instanceId,
                ExtensionMonths = 2,
                Remarks = "Mock extension details"
            });
        }

        public Task<object> UploadAttachmentAsync(IFormFile file, int empId, int instanceId, CancellationToken ct)
        {
            return Task.FromResult<object>(new
            {
                EmpId = empId,
                InstanceId = instanceId,
                FileName = file.FileName,
                Message = "Attachment uploaded (mock)"
            });
        }

        public Task<IEnumerable<object>> GetAttachmentsAsync(int empId, int instanceId, CancellationToken ct)
        {
            var mockList = new List<object>
            {
                new { EmpId = empId, InstanceId = instanceId, FileName = "mock1.pdf", FilePath = "/mock/path/mock1.pdf" },
                new { EmpId = empId, InstanceId = instanceId, FileName = "mock2.docx", FilePath = "/mock/path/mock2.docx" }
            };
            return Task.FromResult<IEnumerable<object>>(mockList);
        }

        public Task<object> ValidateSubmissionAsync(ValidateSubmissionDto dto, CancellationToken ct)
        {
            return Task.FromResult<object>(new
            {
                Success = true,
                Message = "Mock submission validated",
                MasterId = dto.MasterId,
                EmpId = dto.EmpId
            });
        }

        public Task<bool> SaveRMEvaluationFeedbackAsync(IEnumerable<RMEvaluationDto> feedbacks, CancellationToken ct)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SaveHeadEvaluationFeedbackAsync(IEnumerable<HeadEvaluationDto> feedbacks, CancellationToken ct)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SaveHeadConfirmationAsync(HeadConfirmationDto dto, CancellationToken ct)
        {
            return Task.FromResult(true);
        }
    }

    }
}
