---
sidebar_position: 3
---

# Creating Commands

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