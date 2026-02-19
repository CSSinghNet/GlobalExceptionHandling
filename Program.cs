using GlobalExceptionHandling.Api.Handlers;
using GlobalExceptionHandling.Handlers;
using GlobalExceptionHandling.Services;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddProblemDetails();
// Configure ProblemDetails with global customization
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
        ctx.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
        ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
    };
});


// Register exception handlers in order (first match wins)
// Uncomment the specialized handlers to see handler chaining in action
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
 builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register the dummy product service as scoped
builder.Services.AddScoped<IProductService, DummyProductService>();



var app = builder.Build();
app.UseExceptionHandler();
//app.UseMiddleware<ExceptionHandlingMiddleware>(); // pre .net 8

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
