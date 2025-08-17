using System.Data;
using Microsoft.Data.SqlClient;
using EmployeeConfirmationApi.Models;

namespace EmployeeConfirmationApi.Services
{
    public class SqlEmpConfirmationService : IEmpConfirmationService
    {
        private readonly string _connStr;
        public SqlEmpConfirmationService(string connStr) => _connStr = connStr;

        public async Task<IReadOnlyList<EmployeeDto>> GetEmployeesForConfirmationAsync(CancellationToken ct)
        {
            var results = new List<EmployeeDto>();

            await using var conn = new SqlConnection(_connStr);
            await using var cmd = new SqlCommand("Sch_EmpConfirmation_GetEmployeesForConfirmation", conn)
            { CommandType = CommandType.StoredProcedure };

            await conn.OpenAsync(ct);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);

            while (await rdr.ReadAsync(ct))
            {
                results.Add(new EmployeeDto
                {
                    EmpId = rdr["EmpID"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["EmpID"]),
                    Name = rdr["Name"] == DBNull.Value ? "" : rdr["Name"].ToString()!,
                    Designation = rdr["Designation"] == DBNull.Value ? "" : rdr["Designation"].ToString()!,
                    Manager = rdr["Manager"] == DBNull.Value ? "" : rdr["Manager"].ToString()!,
                    JoiningDate = rdr["JoiningDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rdr["JoiningDate"])
                });
            }
            return results;
        }

        public async Task<int?> InsertSchedularDetailsAsync(InsertScheduleDto dto, CancellationToken ct)
        {
            await using var conn = new SqlConnection(_connStr);
            await using var cmd = new SqlCommand("Sch_EmpConfirmation_InsertSchedularDetails", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add("@MEmpID", SqlDbType.Int).Value = dto.EmpId;
            cmd.Parameters.Add("@MProjectID", SqlDbType.Int).Value = dto.MajorProjectId;
            cmd.Parameters.Add("@ConfirmDate", SqlDbType.DateTime).Value = dto.ConfirmDate;

            var outParam = new SqlParameter("@ECID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outParam);

            await conn.OpenAsync(ct);
            await cmd.ExecuteNonQueryAsync(ct);

            return outParam.Value == DBNull.Value ? null : (int?)Convert.ToInt32(outParam.Value);
        }
        public async Task<object> GetExtensionDetailsAsync(int empId, int instanceId, CancellationToken ct)
{
    await using var conn = new SqlConnection(_connStr);
    await using var cmd = new SqlCommand("EmpConfirmation_GetExtensionDetails", conn)
    { CommandType = CommandType.StoredProcedure };

    cmd.Parameters.AddWithValue("@MEMPID", empId);
    cmd.Parameters.AddWithValue("@InstanceID", instanceId);

    await conn.OpenAsync(ct);
    var table = new DataTable();
    using (var da = new SqlDataAdapter(cmd))
    {
        da.Fill(table);
    }

    return table; // return DataTable or map to object
}

public async Task<object> UploadAttachmentAsync(IFormFile file, int empId, int instanceId, CancellationToken ct)
{
    // NOTE: Adjust this to your DB/SP logic
    return new { Message = "Upload to DB not implemented yet", EmpId = empId, InstanceId = instanceId };
}

        public async Task<IEnumerable<object>> GetAttachmentsAsync(int empId, int instanceId, CancellationToken ct)
        {
            await using var conn = new SqlConnection(_connStr);
            await using var cmd = new SqlCommand("EmpConfirmation_GetAttachments", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@EmpId", empId);
            cmd.Parameters.AddWithValue("@InstanceId", instanceId);

            await conn.OpenAsync(ct);
            var results = new List<object>();
            using (var rdr = await cmd.ExecuteReaderAsync(ct))
            {
                while (await rdr.ReadAsync(ct))
                {
                    results.Add(new
                    {
                        EmpId = rdr["EmpId"],
                        InstanceId = rdr["InstanceId"],
                        FileName = rdr["FileName"],
                        FilePath = rdr["FilePath"]
                    });
                }
            }
            return results;
        }
public async Task<object> ValidateSubmissionAsync(ValidateSubmissionDto dto, CancellationToken ct)
{
    await using var conn = new SqlConnection(_connStr);
    await using var cmd = new SqlCommand("EmpConfirmation_UpdateDetails", conn)
    { CommandType = CommandType.StoredProcedure };

    cmd.Parameters.AddWithValue("@ECID", dto.MasterId);
    cmd.Parameters.AddWithValue("@MEMPID", dto.EmpId);
    cmd.Parameters.AddWithValue("@ConfirmationDate", dto.DateOfJoining.AddMonths(3));
    cmd.Parameters.AddWithValue("@IsForConfirmation", dto.Action);
    cmd.Parameters.AddWithValue("@Extension", dto.Action == "Extend" ? dto.ExtensionMonths : 0);
    cmd.Parameters.AddWithValue("@Remarks", dto.Remarks ?? "");

    await conn.OpenAsync(ct);
    await cmd.ExecuteNonQueryAsync(ct);

    return new {
        Success = true,
        Message = "Submission saved successfully",
        MasterId = dto.MasterId,
        EmpId = dto.EmpId
    };
}
        public async Task<bool> SaveRMEvaluationFeedbackAsync(IEnumerable<RMEvaluationDto> feedbacks, CancellationToken ct)
        {
            using var conn = new SqlConnection(_connStr);
            await conn.OpenAsync(ct);

            foreach (var fb in feedbacks)
            {
                using var cmd = new SqlCommand("EmpConfirmation_UpdateRMFeedback", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@ECID", fb.MasterId);
                cmd.Parameters.AddWithValue("@ParamID", fb.ParamId);
                cmd.Parameters.AddWithValue("@RMRating", fb.RMRating);
                cmd.Parameters.AddWithValue("@RMRemarks", fb.RMRemarks ?? (object)DBNull.Value);

                await cmd.ExecuteNonQueryAsync(ct);
            }

            return true;
        }
public async Task<bool> SaveHeadEvaluationFeedbackAsync(IEnumerable<HeadEvaluationDto> feedbacks, CancellationToken ct)
{
    await using var conn = new SqlConnection(_connStr);
    await conn.OpenAsync(ct);

    foreach (var fb in feedbacks)
    {
        await using var cmd = new SqlCommand("EmpConfirmation_UpdateHeadFeedback", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@ECID", fb.MasterId);
        cmd.Parameters.AddWithValue("@ParamID", fb.ParamId);
        cmd.Parameters.AddWithValue("@HeadRating", fb.HeadRating);
        cmd.Parameters.AddWithValue("@HeadRemarks", (object?)fb.HeadRemarks ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    return true;
}

public async Task<bool> SaveHeadConfirmationAsync(HeadConfirmationDto dto, CancellationToken ct)
{
    await using var conn = new SqlConnection(_connStr);
    await conn.OpenAsync(ct);

    await using var cmd = new SqlCommand("EmployeeConfirmation_UpdateIsForConfirmationGH", conn)
    {
        CommandType = CommandType.StoredProcedure
    };

    cmd.Parameters.AddWithValue("@InstanceID", dto.InstanceId);
    cmd.Parameters.AddWithValue("@IsForConfirmationGH", dto.IsForConfirmationGH);
    cmd.Parameters.AddWithValue("@GHJustificationORReason", (object?)dto.GHJustificationORReason ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@Extension", dto.Extension);

    await cmd.ExecuteNonQueryAsync(ct);

    return true;
}



    }
}
