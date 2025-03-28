---
sidebar_position: 3
---

# Creating Commands

- Root commands can call entity commands or queries, and queries on other roots (but not commands on other roots).
- Entity commands can also read the entity's state, or call commands/queries of other entities, and query other aggregate roots.
- Both commands and queries can inject any dependencies they need through the constructor via DI.
- Read about how commands and queries are used in [Using CQRS](using-cqrs.md).

## Available Methods and Properties

Root and entity commands have access to the same methods and properties as queries [which can be found here](creating-queries.md#available-methods-and-properties) with the addition of the `IInternalCommandFactory CommandFactory`. The `CommandFactory` can run both queries and commands; however, command handlers still have access to the `QueryFactory` for the use of doing queries on other aggregates. The `CommandFactory` for command handlers does not have access to other aggregates as an intentional and important property of Remotr.\
\
**Entity commands also have the addition of two methods:**

- `Task UpdateState(newState)`: Updates the state safely.
- `Task ClearState()`: "Clears" the state. Will do a hard deletion in the database. The state will have default values if accessed again. **TODO: ClearState implementation**

This differentiates commands in that they have the ability to change state. Notice how there is no `CreateState` because the state of entities (virtual actors) is always presumed to exist. Meaning that, rather than being null, the state will simply have default values if it has never been updated before.


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

## Types of Commands

Remotr command handlers can work with or without input parameters and can produce an output or not.\
\
These examples all use the [source generator](source-generation.md) which allows for simplifying the regular syntax with generated extension methods which is discussed [here](using-cqrs.md).\
\
There are three basic patterns available:

import Tabs from '@theme/Tabs';
import TabItem from '@theme/TabItem';

### Commands with No Input and No Output

<Tabs>
  <TabItem value="root-command-no-io" label="RootCommandHandler" default>
*The below example includes a transactional chain of queries and commands which is discussed extensively in [Using Commands and Queries](using-cqrs.md#transactional-chaining).*
```csharp
[UseShortcuts]
public class ResetAllCustomers : RootCommandHandler<ICustomerRoot>
{
    private readonly ILogger<ResetAllCustomers> _logger;
    
    public ResetAllCustomers(ILogger<ResetAllCustomers> logger)
    {
        _logger = logger;
    }
    
    public override async Task Execute()
    {
        _logger.LogInformation("Resetting all customers for root {RootId}", GetRootKeyString());
        
        // Get a list of customers to reset
        var customerIds = await CommandFactory
            .GetEntity<CustomersListState>()
            .GetAllCustomerIds() // Getting all the customer IDs
            .ThenForEach( // Take the list of customer IDs and do something with each value.
                (b) => b.ThenResetCustomer() // Resetting the customer for each ID found.
            )
            .Run();
    }
}
```

  </TabItem>
  <TabItem value="entity-command-no-io" label="EntityCommandHandler">

```csharp
[UseShortcuts]
public class ResetCustomer : EntityCommandHandler<CustomerState>
{
    private readonly ILogger<ResetCustomer> _logger;
    
    public ResetCustomer(ILogger<ResetCustomer> logger)
    {
        _logger = logger;
    }
    
    public override async Task Execute()
    {
        _logger.LogInformation("Resetting customer with ID: {CustomerId}", EntityKey);
        
        var defaultState = new CustomerState
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            PhoneNumber = string.Empty,
            Address = string.Empty
        };
        
        await UpdateState(defaultState);
    }
}
```

  </TabItem>
</Tabs>

### Commands with No Input but with Output

<Tabs>
  <TabItem value="root-command-output" label="RootCommandHandler" default>

```csharp
[UseShortcuts]
public class CreateNewCustomer : RootCommandHandler<ICustomerRoot, string>
{
    private readonly ILogger<CreateNewCustomer> _logger;
    
    public CreateNewCustomer(ILogger<CreateNewCustomer> logger)
    {
        _logger = logger;
    }
    
    public override async Task<string> Execute()
    {
        var newCustomerId = Guid.NewGuid().ToString();
        _logger.LogInformation("Creating new customer with ID: {CustomerId}", newCustomerId);
        
        // Initialize the new customer
        await CommandFactory
            .GetEntity<CustomerState>()
            .InitializeCustomer()
            .Run(newCustomerId);
            
        return newCustomerId;
    }
}
```

  </TabItem>
  <TabItem value="entity-command-output" label="EntityCommandHandler">

```csharp
[UseShortcuts]
public class InitializeCustomer : EntityCommandHandler<CustomerState, bool>
{
    private readonly ILogger<InitializeCustomer> _logger;
    
    public InitializeCustomer(ILogger<InitializeCustomer> logger)
    {
        _logger = logger;
    }
    
    public override async Task<bool> Execute()
    {
        _logger.LogInformation("Initializing new customer with ID: {CustomerId}", EntityKey);
        
        var initialState = new CustomerState
        {
            FirstName = "New",
            LastName = "Customer",
            PhoneNumber = string.Empty,
            Address = string.Empty
        };
        
        await UpdateState(initialState);
        return true;
    }
}
```

  </TabItem>
</Tabs>

### Commands with Input and Output

<Tabs>
  <TabItem value="root-command-input-output" label="RootCommandHandler" default>

```csharp
[UseShortcuts]
public class UpdateCustomer : RootCommandHandler<ICustomerRoot, UpdateCustomerRequest, bool>
{
    private readonly ILogger<UpdateCustomer> _logger;
    
    public UpdateCustomer(ILogger<UpdateCustomer> logger)
    {
        _logger = logger;
    }
    
    public override async Task<bool> Execute(UpdateCustomerRequest request)
    {
        _logger.LogInformation("Updating customer with ID: {CustomerId}", request.CustomerId);
        
        // Call entity command to update customer info
        return await CommandFactory
            .GetEntity<CustomerState>()
            .UpdateCustomerInfo(
                new UpdateCustomerInfoInput(
                    request.FirstName,
                    request.LastName,
                    request.PhoneNumber,
                    request.Address))
            .Run(request.CustomerId);
    }
}

[GenerateSerializer]
public sealed record UpdateCustomerRequest(
    string CustomerId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Address);
```

  </TabItem>
  <TabItem value="entity-command-input-output" label="EntityCommandHandler">

```csharp
[UseShortcuts]
public class UpdateCustomerInfo : EntityCommandHandler<CustomerState, UpdateCustomerInfoInput, bool>
{
    private readonly ILogger<UpdateCustomerInfo> _logger;

    public UpdateCustomerInfo(ILogger<UpdateCustomerInfo> logger)
    {
        _logger = logger;
    }

    public override async Task<bool> Execute(UpdateCustomerInfoInput input)
    {
        _logger.LogInformation("Updating user info for customer with ID: {CustomerId}", EntityKey);

        var state = await GetState();
        var newState = state with
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            PhoneNumber = input.PhoneNumber,
            Address = input.Address
        };
        
        await UpdateState(newState);
        return true;
    }
}

[GenerateSerializer]
public sealed record UpdateCustomerInfoInput(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Address);
```

  </TabItem>
</Tabs>