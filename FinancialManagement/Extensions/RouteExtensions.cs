using FinancialManagement.Application.Services;
using FinancialManagement.Domain.Entities;
using Microsoft.OpenApi.Models;

namespace FinancialManagement.Extensions;

public static class RouteExtensions
{
    public static void AddRoutes(this WebApplication app)
    {
        app.AddCreditCardRoutes();
        app.AddFinancialManagementServiceRoutes();
    }

    private static void AddCreditCardRoutes(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/credit-cards", async context =>
            {
                var creditCardService = context.RequestServices.GetRequiredService<CreditCardService>();
                var creditCards = creditCardService.GetAllAsync();
                
                await context.Response.WriteAsJsonAsync(creditCards);

                return;
            })
            .WithName("GetAllCreditCards")
            .WithMetadata(new OpenApiOperation
                { Summary = "Get all credit cards", Description = "Retrieves all credit cards" }).WithOpenApi();

        endpoints.MapGet("/credit-cards/{name}", async context =>
            {
                var creditCardService = context.RequestServices.GetRequiredService<CreditCardService>();
                var name = context.Request.RouteValues["name"] as string;
                var creditCard = creditCardService.GetCreditCardByNameAsync(name);
                await context.Response.WriteAsJsonAsync(creditCard);
            })
            .WithName("GetCreditCardByName")
            .WithMetadata(new OpenApiOperation
                { Summary = "Get credit card by name", Description = "Retrieves a credit card by its name" })
            .WithOpenApi();

        endpoints.MapPost("/credit-cards", async context =>
            {
                var creditCardService = context.RequestServices.GetRequiredService<CreditCardService>();
                var cardName = await context.Request.ReadFromJsonAsync<string>();
                await creditCardService.AddAsync(cardName);
                context.Response.StatusCode = StatusCodes.Status201Created;
            })
            .WithName("AddCreditCard")
            .WithMetadata(new OpenApiOperation
                { Summary = "Add a new credit card", Description = "Adds a new credit card" }).WithOpenApi();

        endpoints.MapPut("/credit-cards", async context =>
            {
                var creditCardService = context.RequestServices.GetRequiredService<CreditCardService>();
                var creditCard = await context.Request.ReadFromJsonAsync<CreditCard>();
                await creditCardService.UpdateAsync(creditCard);
                context.Response.StatusCode = StatusCodes.Status204NoContent;
            })
            .WithName("UpdateCreditCard")
            .WithMetadata(new OpenApiOperation
                { Summary = "Update a credit card", Description = "Updates an existing credit card" }).WithOpenApi();

        endpoints.MapDelete("/credit-cards/{rowKey}", async context =>
            {
                var creditCardService = context.RequestServices.GetRequiredService<CreditCardService>();
                var rowKey = Guid.Parse(context.Request.RouteValues["rowKey"] as string);
                await creditCardService.DeleteAsync(rowKey);
                context.Response.StatusCode = StatusCodes.Status204NoContent;
            })
            .WithName("DeleteCreditCard")
            .WithMetadata(new OpenApiOperation
                { Summary = "Delete a credit card", Description = "Deletes a credit card by its row key" })
            .WithOpenApi();
    }

    private static void AddFinancialManagementServiceRoutes(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/financial-management/{date}", async context =>
            {
                var financialManagementService =
                    context.RequestServices.GetRequiredService<FinancialManagementService>();
                var dateString = context.Request.RouteValues["date"] as string;
                var isValidDate = DateTime.TryParse(dateString, out var date);

                if (!isValidDate)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.Body = new MemoryStream("Invalid date format"u8.ToArray());

                    return;
                }

                var financialData = financialManagementService.QueryMonthlyBills(date);

                await context.Response.WriteAsJsonAsync(financialData);
            })
            .WithName("GetFinancialDataByDate")
            .WithMetadata(new OpenApiOperation
            {
                Summary = "Get financial data by date", Description = "Retrieves financial data for a specific date"
            }).WithOpenApi();

        endpoints.MapPost("/financial-management/purchase", async context =>
            {
                var financialManagementService =
                    context.RequestServices.GetRequiredService<FinancialManagementService>();
                var purchase = await context.Request.ReadFromJsonAsync<Purchase>();

                if (purchase is null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                await financialManagementService.AddPurchaseAsync(purchase);
                context.Response.StatusCode = StatusCodes.Status201Created;
            })
            .WithName("AddPurchase")
            .WithMetadata(new OpenApiOperation { Summary = "Add a new purchase", Description = "Adds a new purchase" })
            .WithOpenApi();

        endpoints.MapPost("/financial-management/credit-card/{creditCard}/purchase", async context =>
            {
                var financialManagementService =
                    context.RequestServices.GetRequiredService<FinancialManagementService>();
                var creditCard = context.Request.RouteValues["creditCard"] as string;
                var purchase = await context.Request.ReadFromJsonAsync<Purchase>();

                if (purchase is null || string.IsNullOrWhiteSpace(creditCard))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                await financialManagementService.AddPurchaseToCreditCardAsync(creditCard, purchase);
                context.Response.StatusCode = StatusCodes.Status201Created;
            })
            .WithName("AddPurchaseToCreditCard")
            .WithMetadata(new OpenApiOperation
            {
                Summary = "Add a purchase to a credit card",
                Description = "Adds a new purchase to a specific credit card"
            }).WithOpenApi();
    }
}