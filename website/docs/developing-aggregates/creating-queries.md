---
sidebar_position: 2
---

# Creating Queries

- Root queries can only call other queries
- Entity queries can also read the entity's state
- Both commands and queries can inject any dependencies they need through the constructor via DI.

## Available Methods and Properties

Root and entity query handlers have most of the same methods and properties, but entity queries have access to state:

<Tabs>
    <TabItem value="root-query-handler" label="RootQueryHandler" default>
        <p><b>Shared properties</b></p>
        <ul>
            <li>`IInternalQueryFactory QueryFactory`: Make Remotr queries on other aggregate roots or entities.</li>
            <li>`IGrainFactory GrainFactory`: For using regular Orleans grains.</li>
        </ul>
        <p><b>Unique properties: key fetchers</b></p>
        <p>Unlike entities, roots can have a number of different ways to choose how they will be addressed including via Guid, string, long, or a compound of Guid+string or Guid+long. Here are the available methods for fetching the key depending on the key strategy chosen:</p>
        <ul>
            <li>`Guid GetRootKey()`: Gets the key of the root being as a `Guid`. Will throw an exception if a Guid can't be formed from the key.</li>
            <li>`string GetRootKeyString()`: Gets the key of the root being queried. In this case, it will return the `string` value of the key regardless of what the actual type of the key is.</li>
            <li>`Guid GetRootKey(out string keyExt)`: Gets the key of the root being as a `Guid`, and outputs a string variable for the secondary key. Will throw an exception if a Guid can't be formed from the key.</li>
            <li>`long GetRootKey()`: Gets the key of the root being as a `long`. Will throw an exception if a long can't be formed from the key.</li>
            <li>`long GetRootKeyLong(out string keyExt)`: Gets the key of the root being as a `long`, and outputs a string variable for the secondary key. Will throw an exception if a long can't be formed from the key.</li>
        </ul>
    </TabItem>
    <TabItem value="entity-query-handler" label="EntityQueryHandler">
        <p><b>Shared properties</b></p>
        <ul>
            <li>`IInternalQueryFactory QueryFactory`: Make Remotr queries on other Aggregate Roots or Entities.</li>
            <li>`IGrainFactory GrainFactory`: For using regular Orleans grains.</li>
        </ul>
        <p><b>Unique properties</b></p>
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
}
```

## Example Queries without Input

Remotr query handlers can work with or without input parameters. You can create simple query handlers that don't require input, or more complex ones that take parameters to filter or process the data, or to even make decisions as to what other queries to call.

<Tabs>
  <TabItem value="root-query" label="RootQueryHandler" default>


```csharp

public class GetVipCustomerDetails : RootQueryHandler<ICustomerRoot, List<CustomerInfo>> // This query is for "ICustomerRoot".
{
    private readonly IVipService _vipService;

    public GetVipCustomerDetails(IVipService vipService)
    {
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

            var customerInfo = await QueryFactory
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
            throw new Exception("Customer not found");
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

    public override async Task<List<string>> Execute(AddressVerificationRequest request)
    {
        // The input parameter contains everything we need to know:
        // - Which customers to check
        // - What address to verify against
        
        var matchingCustomerIds = new List<string>();
        
        foreach (var customerId in request.CustomerIds)
        {
            // Call an entity query that requires an input parameter
            var livesAtAddress = await QueryFactory
                .GetAggregate<ICustomerAggregate>()
                .DoesCustomerLiveAtAddress(
                    new AddressCheckRequest { Address = request.Address })  // This calls our entity query with input
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