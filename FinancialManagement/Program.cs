using FinancialManagement.Application.Extensions;
using FinancialManagement.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
//app.AddRoutes();
app.MapControllers();
app.UseCors(policyBuilder =>
{
    policyBuilder.AllowAnyOrigin();
    policyBuilder.AllowAnyMethod();
    policyBuilder.AllowAnyHeader();
});
app.Run();