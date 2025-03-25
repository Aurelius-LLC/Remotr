---
sidebar_position: 4
---

import ChainExample from '@site/static/img/chain_example.webp';

# Using Commands and Queries

## Simple Usage

**To interact with Aggregates** from non-aggregate code (like API controllers) without using the source generator:

- Queries are called like `.Ask<Query, InputType, OutputType>(input)`
- Commands are called like `.Tell<Command, InputType, OutputType>(input)`

**Input/Output Types:** For both commands and queries, you don't have to have an input type. For commands, you can also lack an output type as well. Without an output, the return type would simply be `Task`.
\
\
**With the Source Generator:** Queries and commands are called like `.CommandName(Input)` or `.QueryName(Input)`. This drastically simplifies the required syntax. Learn more about the source generator [here](source-generation.md).
\
\
**Example usage without using the source generator:**
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

## Transactional Chaining

In Remotr, all commands against an aggregate have ACID properties; however, a unique property of Remotr is that commands and queries can be chained together.\
\
For example (using the source generator):

<img src={ChainExample} />
\
Both the command and query factory can chain requests; however, the command factory has access to both queries *and* commands.
