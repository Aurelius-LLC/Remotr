---
sidebar_position: 3
---

# Creating Commands

- Root commands can call entity commands or queries, and queries on other roots (but not commands).
- Entity commands can also read the entity's state, or call commands/queries of other entities, and query other aggregate roots.
- Both commands and queries can inject any dependencies they need through the constructor via DI.

## Available Methods and Properties

Root and entity commands have access to the same methods and properties as queries [which can be found here](creating-queries.md#available-methods-and-properties) with the addition of the `IInternalCommandFactory CommandFactory`. The `CommandFactory` can run both queries and commands; however, command handlers still have access to the `QueryFactory` for the use of doing queries on other aggregates. The `CommandFactory` for command handlers does not have access to other aggregates as an intentional and important property of Remotr.\
\
**Entity commands also have the addition of two methods:**

- `Task UpdateState(newState)`: Updates the state safely.
- `Task ClearState()`: "Clears" the state. Will do a hard deletion in the database. The state will have default values if accessed again. **TODO: ClearState implementation**

This differentiates commands in that they have the ability to change state. Notice how there is no `CreateState` because the state of entities (virtual actors) is always presumed to exist. Meaning that, rather than being null, the state will simply have default values if it has never been updated before.





Commands can modify entity state:

```csharp
// EntityCommandHandler Generic Forms:
// 1) EntityCommandHandler<TState>
// 2) EntityCommandHandler<TState, TOutput>
// 3) EntityCommandHandler<TState, TInput, TOutput>
public class UpdateCustomerInfo : EntityCommandHandler<CustomerState, UpdateCustomerInfoInput, bool>
{
    private readonly ILogger _logger;

    // Any registered services can be injected
    public UpdateCustomerInfo(ILogger logger)
    {
        _logger = logger;
    }

    public override async Task<bool> Execute(UpdateCustomerInfoInput input)
    {
        _logger.LogInformation("Updating user info for customer with ID: {}", this.GetAggregateId());

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