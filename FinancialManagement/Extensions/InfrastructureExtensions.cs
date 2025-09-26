using FinancialManagement.Infrastructure.Repositories;

namespace FinancialManagement.Extensions;

public static class InfrastructureExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<CreditCardRepository>();
        services.AddScoped<CreditCardBillRepository>();
        services.AddScoped<PurchaseRepository>();
    }
}