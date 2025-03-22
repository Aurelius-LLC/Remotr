---
sidebar_position: 4
---

# Using Commands and Queries

To interact with Aggregates from non-aggregate code (like API controllers):

```csharp
public class ApiCustomerController
{
    private readonly IExternalQueryFactory _queryFactory;
    private readonly IExternalCommandFactory _commandFactory;

    public ApiCustomerController(
        IExternalQueryFactory queryFactory,
        IExternalCommandFactory commandFactory)
    {
        _queryFactory = queryFactory;
        _commandFactory = commandFactory;
    }

    public async Task<CustomerInfo> GetCustomerInfo(Guid customerId)
    {
        return await _queryFactory
            .GetAggregate<ICustomerAggregate>()
            .Ask<RetrieveCustomerInfo, CustomerInfo>()
            .Run(customerId);
    }

    public async Task<bool> UpdateCustomerInfo(Guid customerId, UpdateCustomerInfoInput input)
    {
        return await _commandFactory
            .GetAggregate<ICustomerAggregate>()
            .Tell<UpdateCustomerInfo, UpdateCustomerInfoInput, bool>(input)
            .Run(customerId);
    }
}
```