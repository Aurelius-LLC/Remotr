---
sidebar_position: 2
---

# Creating Queries

Root queries can only call other queries, entity queries can also read the entity's state. Both commands and queries can inject any dependencies they need through constructor injection.

## Available Methods and Properties

Root and entity query handlers have most of the same methods and properties, but entity queries have access to state:

<Tabs>
    <TabItem value="root-query-handler" label="RootQueryHandler" default>
        <p>Shared properties</p>
        <ul>
            <li>`IInternalQueryFactory QueryFactory`: Make Remotr queries on other Aggregate Roots or Entities.</li>
            <li>`IGrainFactory GrainFactory`: For using regular Orleans grains.</li>
        </ul>
    </TabItem>
    <TabItem value="entity-query-handler" label="EntityQueryHandler">
        <p>Shared properties</p>
        <ul>
            <li>`IInternalQueryFactory QueryFactory`: Make Remotr queries on other Aggregate Roots or Entities.</li>
            <li>`IGrainFactory GrainFactory`: For using regular Orleans grains.</li>
        </ul>
        <p>Unique properties</p>
        <ul>
            <li>`string EntityKey`: Gets the key of the entity being queried. Entity keys are always strings.</li>
            <li>`State GetState()`: Retrieves the current state of the entity.</li>
        </ul>
    </TabItem>
</Tabs>

## The Entity State

The following examples will work with this entity state:

```csharp
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

## Example Queries without Input

Remotr query handlers can work with or without input parameters. You can create simple query handlers that don't require input, or more complex ones that take parameters to filter or process the data, or to even make decisions as to what other queries to call.

import Tabs from '@theme/Tabs';
import TabItem from '@theme/TabItem';

<Tabs>
  <TabItem value="root-query" label="RootQueryHandler" default>


```csharp

public class GetVipCustomerDetails : RootQueryHandler<ICustomerRoot, List<CustomerInfo>> // This query is for "ICustomerRoot".
{
    private readonly IExternalQueryFactory _queryFactory;
    private readonly IVipService _vipService;

    public GetVipCustomerDetails(
        IExternalQueryFactory queryFactory,
        IVipService vipService)
    {
        _queryFactory = queryFactory;
        _vipService = vipService;
    }

    public override async Task<List<CustomerInfo>> Execute()
    {
        // Get all VIP customer IDs from a service
        var vipCustomerIds = await _vipService.GetAllVipCustomerIds();
        
        var results = new List<CustomerInfo>();
        
        foreach (var customerId in vipCustomerIds)
        {

            /*
            / Call the no-input entity query to get customer details
            / Note how we're using the query factory to get the aggregate
            / and then calling the query directly
            */

            var customerInfo = await _queryFactory
                .GetAggregate<ICustomerAggregate>()
                .GetCustomerDetails()  // This calls our no-input entity query
                .Run(customerId);      // Pass the entity ID to run against
                

            results.Add(customerInfo);
        }
        
        return results;
    }
}
```
  </TabItem>
  <TabItem value="entity-query" label="EntityQueryHandler">

```csharp
public class GetCustomerDetails : EntityQueryHandler<CustomerState, CustomerInfo>
{
    private readonly ILogger<GetCustomerDetails> _logger;

    public GetCustomerDetails(ILogger<GetCustomerDetails> logger)
    {
        _logger = logger;
    }

    public override async Task<CustomerInfo> Execute()
    {
        /*
        /
        / Entity queries can access entity state.
        /
        */
        var state = await GetState();
        

        if (state == null || string.IsNullOrEmpty(state.FirstName))
        {
            _logger.LogWarning("No customer found with ID: {CustomerId}", EntityId);
            return CustomerInfo.NotFound;
        }
        
        return new CustomerInfo
        {
            FirstName = state.FirstName,
            LastName = state.LastName,
            PhoneNumber = state.PhoneNumber,
            Address = state.Address
        };
    }
}
```

  </TabItem>
</Tabs>


## Example Queries with Input

Remotr query handlers can accept at most one input parameter to filter or process data. These input parameters are serialized and deserialized automatically, allowing you to pass complex objects between services.


<Tabs>
  <TabItem value="root-query-input" label="RootQueryHandler" default>

```csharp
public class CheckCustomersAtAddress : RootQueryHandler<AddressVerificationRequest, List<string>>
{
    private readonly IExternalQueryFactory _queryFactory;

    public CheckCustomersAtAddress(
        IExternalQueryFactory queryFactory)
    {
        _queryFactory = queryFactory;
    }

    public override async Task<List<string>> Execute(AddressVerificationRequest request)
    {
        // The input parameter contains everything we need to know:
        // - Which customers to check
        // - What address to verify against
        
        var matchingCustomerIds = new List<string>();
        
        foreach (var customerId in request.CustomerIds)
        {
            // Call an entity query that requires an input parameter
            var livesAtAddress = await _queryFactory
                .GetAggregate<ICustomerAggregate>()
                .DoesCustomerLiveAtAddress(new AddressCheckRequest { Address = request.Address })  // This calls our entity query with input
                .Run(customerId);
                
            // Only add customers who live at the specified address
            if (livesAtAddress)
            {
                matchingCustomerIds.Add(customerId);
            }
        }
        
        return matchingCustomerIds;
    }
}

[GenerateSerializer]
public record AddressVerificationRequest
{
    public List<string> CustomerIds { get; init; } = new();
    public Address Address { get; init; }
}
```

  </TabItem>
  <TabItem value="entity-query-input" label="EntityQueryHandler">

```csharp
public class DoesCustomerLiveAtAddress : EntityQueryHandler<CustomerDirectoryState, AddressCheckRequest, bool>
{
    private readonly ILogger<DoesCustomerLiveAtAddress> _logger;

    public DoesCustomerLiveAtAddress(
        ILogger<DoesCustomerLiveAtAddress> logger)
    {
        _logger = logger;
    }

    public override async Task<bool> Execute(AddressCheckRequest request)
    {
        var state = await GetState();
        
        _logger.LogInformation("Checking if customer lives at address: {Address}", request.Address);
        
        if (!state.CustomersByUsername.TryGetValue(state.CurrentUsername, out var customerInfo))
        {
            _logger.LogWarning("No customer found with username: {Username}", state.CurrentUsername);
            return false;
        }

        // Check if any of the customer's registered addresses match the provided one
        bool matchFound = customerInfo.Addresses.Any(addr => 
            string.Equals(addr.Street, request.Address.Street, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(addr.City, request.Address.City, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(addr.State, request.Address.State, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(addr.ZipCode, request.Address.ZipCode, StringComparison.OrdinalIgnoreCase));
        
        _logger.LogInformation("Address match result for customer {CustomerId}: {Result}", 
            customerInfo.CustomerId, matchFound);
            
        return matchFound;
    }
}

[GenerateSerializer]
public record AddressCheckRequest
{
    public Address Address { get; init; }
}

[GenerateSerializer]
public record Address
{
    public string Street { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string ZipCode { get; init; }
}
```

  </TabItem>
</Tabs>