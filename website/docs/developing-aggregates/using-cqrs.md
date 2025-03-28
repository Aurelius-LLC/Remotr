---
sidebar_position: 4
---

import ChainExample from '@site/static/img/chain_example.webp';

# Using Commands and Queries

## Simple Usage

**To interact with Aggregates** from non-aggregate code (like API controllers) without using the [source generator](source-generation.md):

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
For example *(using the source generator)*:

<img src={ChainExample} />
\
Both the command and query factory can chain requests; however, the command factory has access to both queries *and* commands.


## Advanced Chaining


## Why "Run"?

In [.Net Orleans](https://learn.microsoft.com/en-us/dotnet/orleans/overview), the framework that Remotr is built on, rather than calling `.Run(Id)` at the end of the request call, the "grains" are first fetched by reference, and then requests can be run against them. For example (roughly taken from [here](https://learn.microsoft.com/en-us/dotnet/orleans/grains/)):
```csharp
IPlayerGrain player = GrainFactory.GetGrain<IPlayerGrain>(playerId);
await player.JoinGame(game);
```
\
In Remotr, `.Run(Id)` occurs at the end of the chain of queries or commands, and this allows for storing a transactional chain in a local variable to reuse it for different roots or entities. It also allows for logically modifying the chain as needed.

**For example:**
```csharp
var documentCreator = CommandFactory
    .GetEntity<SharedDocumentState>()
    .CreateDocument() // Initially create the document.
    .SetPrivacyPolicy(documentsInput.ArePrivate); // Set the privacy policy.

if (input.AreGroupDocuments) {
    documentCreator = documentCreator
        .MergeSplit( // This allows us to return the original type as required.
            (b) => b.ForEach(
                input.GroupMembers!.Value,
                (b) => b.AddGroupMember()
            ),
            (b) => b, // Do nothing with this, it's just here to maintain the original type.
            new TakeSecond(), // Take the second returned value
        );
}

// Create the document for each passed request
for (var id in documentsInput.docIds) {
    await documentCreator.Run(id);
}
```