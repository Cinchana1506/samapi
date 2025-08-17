using EmployeeConfirmationApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to DI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Service injection (mock or SQL based on config)
builder.Services.AddScoped<IEmpConfirmationService>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var useMock = cfg.GetValue<bool>("UseMock");
    if (useMock) return new MockEmpConfirmationService();
    var cs = cfg.GetConnectionString("Default")
             ?? throw new InvalidOperationException("Missing connection string.");
    return new SqlEmpConfirmationService(cs);
});

var app = builder.Build();

//  Added global error handler AFTER app is built
app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext httpContext) =>
{
    var exception = httpContext.Features
        .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

    return Results.Problem(
        title: "An unexpected error occurred",
        detail: exception?.Message,
        statusCode: StatusCodes.Status500InternalServerError
    );
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
