---
sidebar_position: 2
---

# Creating Queries

Queries can only read entity state. Both commands and queries can inject any dependencies they need through constructor injection.

```csharp
public class FindCustomerById : EntityQueryHandler<CustomerDirectoryState, CustomerInfo, string>
{
    private readonly ILogger<FindCustomerById> _logger;
    private readonly IAuthService _authService;

    public FindCustomerById(
        ILogger<FindCustomerById> logger,
        IAuthService _authService)
    {
        _logger = logger;
        this._authService = _authService;
    }

    public override async Task<CustomerInfo> Execute(string customerId)
    {
        var state = await GetState();

        _logger.LogInformation("Finding customer by ID: {CustomerId}", customerId);
        
        // Map the customer ID to a username using the auth service
        string username = await _authService.GetUsernameFromCustomerId(customerId);
        
        if (string.IsNullOrEmpty(username) || !state.CustomersByUsername.ContainsKey(username))
        {
            _logger.LogWarning("No customer found for ID: {CustomerId}", customerId);
            return CustomerInfo.NotFound;
        }
        
        return state.CustomersByUsername[username];
    }
}

[GenerateSerializer]
public sealed record CustomerInfo
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    
    public static CustomerInfo NotFound => new() 
    { 
        FirstName = "Not", 
        LastName = "Found" 
    };
}
```

## Available Methods

Query handlers have access to the following methods:

- `GetState()`: Retrieves the current state of the entity.
- `EntityId`: Gets the ID of the entity being queried.
- `EntityType`: Gets the type name of the entity.

## Queries with Input Parameters

Queries can also accept input parameters:

```csharp
// Query definition with input
public record GetCustomerPurchaseHistory(DateTime StartDate, DateTime EndDate);

// Query handler that accepts input parameters
public class RetrieveCustomerPurchaseHistory : 
    EntityQueryHandler<CustomerState, PurchaseHistoryResult, GetCustomerPurchaseHistory>
{
    private readonly IPurchaseRepository _purchaseRepository;
    
    public RetrieveCustomerPurchaseHistory(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }
    
    public override async Task<PurchaseHistoryResult> Execute(GetCustomerPurchaseHistory query)
    {
        var state = await GetState();
        
        // Use the input parameters in the query
        var purchases = await _purchaseRepository.GetPurchasesForCustomer(
            EntityId, 
            query.StartDate, 
            query.EndDate
        );
        
        return new PurchaseHistoryResult
        {
            CustomerId = EntityId,
            CustomerName = $"{state.FirstName} {state.LastName}",
            TimeRange = $"{query.StartDate:d} - {query.EndDate:d}",
            Purchases = purchases.ToList()
        };
    }
}

[GenerateSerializer]
public sealed record PurchaseHistoryResult
{
    public string CustomerId { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string TimeRange { get; init; } = string.Empty;
    public List<Purchase> Purchases { get; init; } = new();
}
```
