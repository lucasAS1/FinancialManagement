using FinancialManagement.Application.Services;

namespace FinancialManagement.Application.Extensions;

public static class ApplicationExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<FinancialManagementService>();
        services.AddScoped<CreditCardService>();
    }
}