# Remotr Introduction

<p>Remotr is a wrapper framework built on top of .Net Orleans.</br>
Remotr was created to simplify 3 common scenarios:</p>


1. You want to use a database which allows for querying data using a rich SQL-like syntax.
2. You want transactions that can span different grains without compromising on the previous point.
3. You want to eliminate *most* actor deadlocks which aren't infinite cyclical deadlocks.

---


## Index  
   1. [Quickstart](#Quickstart)  
   2. [Why Remotr was Created?](#Why-Remotr-was-Created?)
   3. [Remotr Docs](#Remotr-Docs)  
   4. [Orleans Feature Differences](#Orleans-Feature-Differences)  
   5. [BAD PRACTICES](#BAD-PRACTICES)  
   6. [Patterns](#Patterns)
   7. [Testing with Remotr](#Testing-with-Remotr)
   8. [Examples](#Examples)

## Quickstart

### Installing and Set up
1. Add the Nuget package "Remotr" as a dependency of the project.
2. Set up Orleans: [Orleans Quickstart Docs](https://learn.microsoft.com/en-us/dotnet/orleans/quickstarts/build-your-first-orleans-app?tabs=visual-studio)
4. Set up Program.cs for Remotr

   - Right now, only Cosmos is supported as a database; however, other databases which support batch JSON transactions could be added in the future.
   </br>
   
   ```csharp

      // Set up Cosmos.
      var cosmosClient = new CosmosClient(
   
         // Your Cosmos connection string.
         builder.Configuration["COSMOS_CONNECTION_STRING"],
   
         new CosmosClientOptions()
         {
            // Define how Cosmos serializes properties.
            SerializerOptions = new()
            {
               PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
         }
      );

      // Set up Remotr
      builder.Host.UseOrleans(siloBuilder =>
      {
          siloBuilder.UseRemotr(remotrBuilder =>
          {
              remotrBuilder.UseCosmosPersistence(cosmosClient, "trackr", cosmosBuilder =>
              {
                  // Transform GrainIds into corresponding Cosmos item ids for each Cosmos Container.
                  // Cosmos item ids must be strings.
                  cosmosBuilder.AddContainer("Customer", "/customerId", grainId =>  grainId.GetGuidKey().ToString());
                  cosmosBuilder.AddContainer("Store", "/storeId", grainId => grainId.ToString());

                  // Your Json serializer options.
                  cosmosBuilder.UseJsonSerializer(jsonSerializerOptions);
              });
          });

         ///
         /// Other Orleans setup code
         ///
      });

      // Don't forget to add the CosmosClient to the IServiceCollection as a singleton dependency.
      serviceCollection.AddSingleton(cosmosClient);
   ```
6. Set up for a single partition type (i.e. a single Cosmos container)  
   1) Creating a Manager Grain

      ```csharp
         // Create your grain interface.
         using Remotr;

         namespace MyNamespace;

         // Make the Manager Grain interface.
         // Using "Api" or "Manager" before Grain can signal that it's a Manager grain
         public interface ICustomerApiGrain : IAggregateRoot, IGrainWithGuidKey // For Manager grains, you can define what type of Grain key they will use.
         {
            // This will ALWAYS be empty.
            // The interface simply defines how the Manager grain is accessed.
         }

         // Make the Manager Grain implementation.
         // Need to have Manager Grains inherit from AggregateRoot<IType> and IType
         public class CustomerApiGrain : AggregateRoot<ICustomerApiGrain>, ICustomerApiGrain
         {
            // Define what persistent store to use
            // This comes from the previous code block cosmosBuilder.AddContainer("Customer" ...
            public CustomerApiGrain([FromKeyedServices("Customer")] IPersistentStore persistentStore) : base(persistentStore)
            {
               // This will ALWAYS be empty.
            }
         }
      ```

      **That's it. You will never add methods to the Manager Grain itself. Instead, you will make queries and commands which can operate on it. More on that later.**

   3) DIing the IPersistentStore  
   4) Note: don’t put methods on the Manager Grain  
7. Multiple partition type setup with Keyed DI  (i.e. multiple Cosmos containers)
8. [View the API documentation for more.](#Remotr-Docs)


  
## Why Remotr was Created?  
1. **Composite Transactional State**
   1. Child grain ids can be reused safely because of grain partitioning, meaning that there will only be one instance per partition of a child grain state with a particular id  
      1) Singleton id pattern within a partition.  
   2. Child grains can treat other children grains as “dependencies” that together form composite and/or hierarchical states which behave transactionally (commands all succeed or fail together across the grain partition).

 
2. **Database Structure and Domain Driven Design**
   1. TODO


3. **Deadlocking Solved by Remotr** 
   1. **Contagious Programming Model:** In Remotr, much like “async” is in many languages, CQRS is an contagious programming model. This means that once something is made as a query, not only can it not execute commands itself, but any downstream queries that it calls can’t execute commands either. This gives you a guarantee that executing a query on a Manager Grain will *never* result in any state being updated for the entire grain partition. This can drastically reduce the scope that a developer needs to reason about when considering if a particular method could safely write against state without causing upstream or downstream issues.

   2. **Cross-Partition Commands:** With the previous point in mind, an incredibly important design principle of Remotr is that **_you should never do cross-partition commands_**.  
      1) The obvious issue with cross-partition commands is that they won’t happen transactionally, one could succeed even though the other fails.  
      2) Another issue is that cross-partition commands cannot be interleaved (with no exceptions), which means that grain partitions can have deadlocks among each other if they attempt to use commands amongst themselves.
      3) [Other bad practices](#BAD-PRACTICES) 

   3. **Cross-Partition Queries:** Unlike the previous point, cross-partition queries are not only safe but even encouraged.   

   4. **Alternative to Cross-Partition Commands:** As a means of circumventing the restriction on cross-partition commands, any necessary transactions that must span partitions should be decoupled with an orchestration or choreography system. We recommend using Temporal to handle these with the durable execution model of long-running transactions.  

   5. **Potential for Deadlocks in Remotr:** If cross-partition commands are always avoided, it should only be possible to create cyclical deadlocks with Remotr where a request causes an infinite call chain. These are much more difficult to cause on accident than the deadlocks which can result from non-interleaving calls happening between multiple grains.

  
5. **Performance Improvements with Remotr**  

   1. Not only can queries safely interleave with both other queries as well as commands, but Remotr introduces safe and straightforward multithreading in the form of having composite states which are formed with multiple grains _(technically, Orleans StatelessWorker grains)_.   

   2. Again, queries against a Manager Grain can interleave with ongoing commands, even when those commands cause state changes. Because of this, queries executed against Manager Grains simply can't cause deadlocks that aren't infinite call cycles. Queries are also guaranteed to have a consistent view of the grain partition state throughout all inter-partition calls for the entirety of the query execution due to a timestamp managed state system that creates ephemeral copies of the state which allow transactions to manipulate different versions of the state that what queries are actively viewing. Technically, this implies that queries are eventually consistent; however, the states of queries will ever only be able to lag behind the ongoing manager command. This is almost always innocuous for all purposes.

   3. While queries can interleave with ongoing commands, commands can’t be started if there are ongoing query executions. This means that if a command is queued, it must wait for all active queries to finish before it can start. This is something that could be fixed in the future. It does imply that, for now, there could be write starvation if a partition constantly has interleaving queries, an unlikely scenario.
  
   4. **Command Reentrancy:** In *.Net Orleans*, there are many ways that grain [reentrancy](https://learn.microsoft.com/en-us/dotnet/orleans/grains/request-scheduling) can be achieved, one of them is [call chain reentrancy](https://learn.microsoft.com/en-us/dotnet/orleans/grains/request-scheduling#call-chain-reentrancy). Remotr uses Orleans call chain reentrancy for all inter-partition operations, including commands. This means that commands can't cause deadlocks within a grain partition; however, it also means that data loss could happen if a stateful command writes over another when both are operating on the same state at the same time. 

   5. **Infinite Cyclical Deadlocks:** These are deadlocks which Remotr can't solve that occur due to an infinite chain of messages that rotates between different grains such as A -> B -> A. It would be fine for a cyclical call to happen as long as the message chain is eventually stopped; however, if the calls simply go on forever, then there's nothing Remotr will do to fix that.


7. **Child grain placement**\
Because child grains are always technically StatelessWorker Grains (an Orleans feature), they are always co-located on a silo with their Manager Grain, without exception. This means that while a grain partition could theoretically hold many gigabytes of data that isn’t in memory, developers should ensure that a large (many gigabytes in size) grain partition should never be active all at once. Child grains should be instantiated only when needed. If it's impossible to keep the in-memory portion of the grain partition relatively small (relative to the amount of available memory in a given cluster node), then the partition should probably be broken down into more granular parts anyways.
  


## Remotr Docs  
1. **First, you need to know…**

   1. **Orleans**

      1) [https://learn.microsoft.com/en-us/dotnet/orleans/overview](https://learn.microsoft.com/en-us/dotnet/orleans/overview)   

      2) Regular Orleans grains, as well as other Orleans features, can be used alongside Remotr grains. It’s important to understand what a regular grain is before creating Remotr grains.  

   2. **CQRS Differences**

      1) **Not Event Sourced:** CQRS is often used alongside event sourcing where queries are optimized by using a separate read-only cache or database. Remotr does use CQRS as a means of optimizing Manager Grain queries by allowing them to interleave with other queries as well as commands, but it does not use a separate cache or database for this as the state is held in-memory with the grains.

      2) **Commands can (and should) Run Queries:** If a query is run from a command, the state that the query accesses will be consistent with any modifications that any preceding commands in the transaction caused. Commands can even call queries on other Grain Partitions; however, as is stated in other locations, commands should never cause cross-partition commands. Read more about that in [Bad Practices](#Bad-Practices)

   3. **Grain partitions**  

      1) **Definition:** A grain partition is group of grains, all co-located on the same silo, which correspond to an actual database partition, such as a Cosmos partition. The co-location of a grain partition, along with a few other Remotr specific features, allows commands across a grain partition to happen transactionally without any sort of middle tier 2PC/2PL transaction system.  

      2) **Manager Grains:** each grain partition has a singular Manager Grain, also known as an API Grain, which serves as the only entry point to the partition for all requests. It also manages the transactions which affect the partition as well. While this may *seem* like a bottleneck, it’s important to remember that [all queries against an API grain can be interleaved](?tab=t.0#bookmark=id.dkjyddi41ox7), and grain partitions are also multithreaded since they are composed of many grains working together with each Child Grain in charge of managing its own state.  

      3) **Child Grains:** Also known as Stateful Grains, grain partitions can have a potentially infinite number of child grains associated with them. The only restriction to the number of child grains is how large a grain partition can become which is imposed by the underlying datastore. And as implied by the alternative name “Stateful Grains”, Child Grains are unlike Manager Grains in that they hold and manage state. They are also different from Manager Grains in that a Child Grain can *only* be accessed via queries or commands from its own Manager Grain or another Child Grain within its partition. 
         1) **Important Takeaways:**   
            1) Child Grains can only be accessed by other grains within their partition, including the Manager Grain and other Child Grains.  
            2) Two Child Grains with the same type and Id can exist in different grain partitions, and there aren’t any problems with that. The Child Grains are unique per Manager Grain.  

   4. **Designing a Grain Partition**  

      1) **Proper Use Case:** Grain partitions are a perfect fit for any complex entity which could potentially have many gigabytes of underlying data, but only some of it would need to be accessed at any given time. For example, a video game player may have gigabytes of data about their game history, records, preferences, purchases, points, replays, highlights, etc, but only a small amount of that data would need to be active at any given time. Social media users are another great example. Often, browsing history, timelines, reaction history, and other activity data are being constantly recorded, but only a small percentage of that needs to be active during a user session.   

      2) [**Bad Use Case**](#BAD-PRACTICES) part 4.

3. **Remotr Transactions**  
   1. Remotr supports ACID transactions across the states of Child Grains via commands executed against Manager Grains.  


4. **Remotr API documentation**  
   1. **CQRS** (but not event sourcing)
      1) Creating a Manager Grain: Refer to the [Quickstart](#Quickstart) for how to create Manager Grains.
         - **Note:** never add methods to Manager Grains. Instead, queries and commands should be written separately.
 
      2) Creating a Child Grain 
         1) Child grains are created by defining the state of the grain.
            ```csharp
            // Don't forget the GenerateSerializer attribute.
            [GenerateSerializer]
            public sealed record CustomerState
            {
                // An Id property is required for Cosmos Items. 
                // Child Grains are unique by Id.
                [Id(0)]
                public string Id { get; set; } 
                
                // Needed to differentiate partitions (depends on the Cosmos Container).
                [Id(1)]
                public Guid CustomerId { get; set; } 

                // Always add the "Id" attribute to state properties (an Orleans requirement).
                [Id(2)]
                public string FirstName { get; set; } = string.Empty;

                [Id(3)]
                public string LastName { get; set; } = string.Empty;

                [Id(4)]
                public string PhoneNumber {get; set; } = string.Empty;

                [Id(5)]
                public string Address { get; set; } = string.Empty;
            }
            ```
            ## **IMPORTANT NOTES:** 
            - Always add the GenerateSerializer attribute to child grain states.
            - States should always have a string Id as well as another Id to differentiate partitions
              

      3) **Creating Commands and Queries** 
         1) **Creating Commands:** Extend StatefulCommandHandler to create a command
            ```csharp

            // StatefulCommandHandler Generic Forms:
            // 1) StatefulCommandHandler<TState>
            // 2) StatefulCommandHandler<TState, TOuput>
            // 3) StatefulCommandHandler<TState, TInput, TOutput>
            //
            // - TState is the state of the child grain the command is acting on.
            // - TOutput is return value from executing the command.
            // - TInput is what is passed to the command.
            //
            // Because updating customer info requires input, we are using the third
            // generic form.
            public class UpdateCustomerInfo : StatefulCommandHandler<CustomerState, UpdateCustomerInfoInput, bool>
            {
                private readonly ILogger _logger;

                // Any registered services can be DIed into commands and queries
                public UpdateCustomerInfo(ILogger logger)
                {
                  _logger = logger;
                }
                public override async Task<bool> Execute(UpateCustomerInfoInput input)
                {
                  _logger.LogInformation("Updating user info for customer with ID: {}", this.GetAggregateId());

                  // Update customer info
                  ...
                }
            }
            
            [GenerateSerializer]
            public sealed record UpdateCustomerInfoInput(
                string FirstName,
                string LastName,
                string PhoneNumber,
                string Address);
            ```
            - Only the first generic type parameter is required. This is the case if the command does not take input and returns nothing.
            - If the command does take input, all three type parameters are required. This is because the two type parameters are treated as the state of the child grain and the return value of Execute.
            - If you do need an input, but do not want to return anything, a best practice is to return a bool the signals success or failure.
         2) **Creating Queries:** Extend StatefulQueryHandler to create a query
            - The generic type parameters are the same as StatefulCommandHandler and have the same constraints.
            - The only difference is that StatefulQueryHandler cannot update state or call commands.

      4) **Reading and Writing State**
         1) Only for Child Grains
            ```csharp
            public class UpdateCustomerInfo : StatefulCommandHandler<CustomerState, UpdateCustomerInfoInput, bool>
            {
                public override async Task<bool> Execute(UpateCustomerInfoInput input)
                {
                    var state = await GetState();
                    var newState = state with
                    {
                        FirstName = input.FirstName,
                        LastName = input.LastName,
                        PhoneNumber = input.PhoneNumber,
                        Address = input.Address
                    };
                    await UpdateState(newState);
                }
            }
            ```
            - Reading state can be done by calling GetState()
            - Writing state can be done by calling UpdateState with the updated state. Only commands have access to UpdateState.

      5) IExternalCommandFactory and IExternalQueryFactory
         ```csharp
         public class ApiCustomerRetrievalRouteHandler
         {
             private readonly IExternalQueryFactory _queryFactory;

             public Query(IExternalQueryFactory queryFactory)
             {
                 _queryFactory = queryFactory;
             }

             public async Task<CustomerInfo> GetCustomerInfo(Guid customerId)
             {
                 return await _queryFactory
                     .GetAggregate<ICustomerApiGrain>()
                     .Ask<RetrieveCustomerInfo, CustomerInfo>()
                     .Run(customerId);
             }
             
             ...
         }

         public class ApiCustomerUpdateRouteHandler
         {
             private readonly IExternalCommandFactory _commandFactory;

             private readonly IExternalQueryFactory _queryFactory;

             public Mutation(IExternalCommandFactory commandFactory, IExternalQueryFactory queryFactory)
             {
                 _commandFactory = commandFactory;
                 _queryFactory = queryFactory;
             }

             public Task UpdateCustomerInformation(Guid customerId, UpdateCustomerInfoInput input)
             {
                 await _commandFactory
                     .GetAggregate<ICustomerApiGrain>()
                     .Tell<UpdateCustomerInfo, UpdateCustomerInfoInput, bool>(input)
                     .Run(customerId);
             }

             ...
         }
         ```
         - External command and query factories should be injected into classes that will call
           Api/Stateless commands and queries.
         - They should never be injected into stateful or stateless commands and queries.
           - If external commands or queries are required, use workflows.

      6) The appropriate Query and Command factories are always on the Command and Query handlers.  
         1) Don’t inject external factors into any commands or queries, this is *always* a bad practice and will likely trigger a runtime or build exception in future versions.  
         2) [Other bad practices](#BAD-PRACTICES)    
      
      <!-- Add back in when API is simplified -->
      <!-- 7) Call Chaining  
         1) Useful, but perhaps underutilized feature  
         2) Remote execution of method chains  
         3) Reusing methods for common data types  
         4) Note: name originally from this concept of not just RPC, but RCPC (Remotely Chaining Procedural Calls). -->

      7) Future API changes  
         1) What metaprogramming could enable.  


   2. Grain Keys  

      1) **Manager Grains** can be addressed with any of the normal Orleans grain key types (Guids, strings, longs, etc.)  

      2) **Child Grains** must always addressed with a string.


   3. Persistence  
      1) Cosmos is the only supported database right now  

      2) Setting up Cosmos: Refer to the [Quickstart](#Quickstart) for how to set up Cosmos.


   4. Extension Methods  
      Extension methods are an extremely useful way to share functionality between different commands or queries that operate on the same Manager or Child Grain.
         ```csharp
         public static class CustomerCommandExtensions
         {
             public static async Task<CustomerInfo> GetCustomerInfo(this BaseStatefulCommandHandler<CustomerState> target)
             {
                 var state = await target.GetState();
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
         - Extension methods can be added for queries and commands of a specific type.
         - In the above example, the mapping of CustomerState to CustomerInfo may be used by multiple different CustomerState commands; however, because it extends the `BaseStatefulCommandHandler<TState>`, it can only be used by commands. Extensions on `BaseStatefulQueryHandler<TState>` can be used by both commands and queries.



   8. Other Useful Methods  
      1) **GetAggregateId() on Child Grains**\
         **This shouldn’t be used to call the Manager Grain ever**, and it should only be used in rare situations anyways, but it is sometimes useful. The downside of this method being accessible is that it’s possible to make Child Grains less reusable for other Manager Grain types if they are tightly coupled with a particular Manager Grain type.  

      2) **`String GetPrimaryKey()` for Child Grains**\
         Similar to Orleans' `GetPrimaryKey()`, `GetPrimaryKey()` for Child Grains will always return their String key address
      
      3) **Getting the key(s) for Manager Grains**\
         Because Manager Grains have more flexibility in their address, there are more methods just like in Orleans:
         - `Guid GetPrimaryKey()`
         - `Guid GetPrimaryKey(out string keyExt)`
         - `long GetPrimaryKeyLong()`
         - `long GetPrimaryKeyLong(out string keyExt)`
         - `string GetPrimaryKeyString()`

6. Orleans Feature Differences  
   1. **Summary:** While many Orleans features can be used with Remotr, there are some that specifically don’t work within Grain Partitions.  

   2. **Sharing State Between Commands and Queries of a Manager or Child Grain**
      - As of now, there is no possibility for sharing in-memory state between commands and queries other than the state of a Child Grain itself.<br />
      **Note:** this is something that will most likely be added in the future.  

   3. **Reminders/Timers (don’t do it)**\
      Even if you could use Reminders or Timers within grain partitions, the problem is that if you don't call Manager Grains via Commands or Queries, then transactions will no longer work and state won't be loaded properly even for queries. The only way that Reminders or Timers can work is if they are used outside the grain partition and then they call a command or query on the grain partition from their callbacks.



## BAD PRACTICES 

1. **Injecting Command/Query factories into a Command or Query:** Don’t inject the ExternalCommandFactory or any other factories into a Command or Query. If a command or query doesn’t have access to a type of factory, this is by design. Only use the CommandFactory or QueryFactory which is found in the commands and queries by default.  

2. **Cross Partition Commands:** NEVER run cross-partition commands. This shouldn’t even be possible without injecting the ExternalCommandFactory into a Command or Query.  

3. **Not Awaiting Child Commands:** It's okay to fan out (and then await) child command calls as long as they won't cause deadlocks, but if you don't await a Child Grain command, then the state most likely won't be updated transactionally and it will cause an exception.

4. **Child to Self-Manager Calls:** Calling the Manager Grain of a Child Grain from the Child Grain itself. This is an inversion of the expected control flow, and usually makes it impossible to reuse Child Grains for other types of Manager Grains. It will also cause a deadlock (on itself) if you run a command against the Manager Grain from one of that Manager Grain’s Child Grains.

5. **Large Grain Partitions:** Having a huge grain partition that will likely have much of the partition in memory at once (by nature of having EntityGrains called). Because grain partitions exist altogether on a single silo, this will likely cause performance issues.
   - **Example:** The IoT security sensors for an entire building should most likely not go directly through a single grain partition, even if it would be convenient for them to do so. This is assuming that they are constantly feeding data, which would slow the grain partition to a halt given hundreds of commands per second. That’s not to say that Remotr couldn’t be utilized, but it’s important to design the grain partitions with this in mind.  



## Common Useful Remotr Patterns  
1. **Singleton Child Grain IDs:**
  ```csharp
  public sealed class ApiUpdateCustomerInfo : StatelessCommandHandler<ICustomerApiGrain, UpdateCustomerInfoInput, bool>
  {
      public override async Task<Post?> Execute(SaveStreaksInput input)
      {
         var customerId = this.GetAggregateId(); // This will be the the manager grain's Id.
          return await CommandFactory
              .GetEntity<CustomerState>()
              .Tell<UpdateCustomerInof, UpdateCustomerInfoInput, bool>(input)
              .Run("Customer"); // Customer will be the Id of all CustomerState objects.
              // This is a singleton on a per-grain-partition basis.
      }
  }
  ```
  - The actual Orleans Grain Id for a child grain is a combinaton of the manager grain Id and the child grain Id.
  - In the above example, it was okay to have a child grain with Id of "Customer" because each customer should only ever have one child grain maintaining CustomerState.
2. **Empty Commands:** TODO


## TODO: Testing with Remotr  
## TODO: Examples
