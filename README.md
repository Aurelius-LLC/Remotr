# Remotr Introduction

<p>Remotr is a wrapper framework built on top of .Net Orleans.</br>
Remotr was created to simplify 3 common scenarios:</p>


1. You want to use a database which allows for querying data using a rich SQL-like syntax.
2. You want transactions that can span different grains without compromising on the previous point.
3. You want to eliminate all actor deadlocks which aren't infinite cyclical deadlocks.

---


## Index  
   1. Quickstart  
   2. Why Remotr was Created?  
   3. Remotr Docs  
   4. Orleans Feature Differences  
   5. BAD PRACTICES  
   6. Patterns  
   7. Testing with Remotr  
   8. Examples

## Quickstart

### Installing  

### Setting Up IPersistentState Injection  
1. Set up for a single partition type (i.e. a single Cosmos container)  
   1) Creating a Manager Grain  
   2) DIing the IPersistentStore  
   3) Note: don’t put methods on the Manager Grain  
2. Multiple partition type setup with Keyed DI  (i.e. multiple Cosmos containers)
3. [View the API documentation for more.](#Remotr-Docs)


  
## Why Remotr was Created?  
1. **Composite Transactional State**
   1. Child grain ids can be reused safely because of grain partitioning, meaning that there will only be one instance per partition of a child grain state with a particular id  
      1) Singleton id pattern within a partition.  
   2. Child grains can treat other children grains as “dependencies” that together form composite and/or hierarchical states which behave transactionally (commands all succeed or fail together across the grain partition).
 
2. **Database Structure and Domain Driven Design**
   1. TODO

3. **Deadlocking Solved by Remotr** 
   1. In Remotr, much like “async” is in many languages, CQRS is an infectious programming model. This means that once something is made as a query, not only can it not execute commands itself, but any downstream queries that it calls can’t execute commands either. This gives you a guarantee that executing a query on a Manager Grain will *never* result in any state being updated for the entire grain partition. This can drastically reduce the scope that a developer needs to reason about when considering if a particular method could safely write against state without causing upstream or downstream issues.   
   2. [Why can’t Remotr queries deadlock?]()  
   3. That being said, an incredibly important design principle of Remotr is that you should never do cross-partition commands.  
      1) The obvious issue with cross-partition commands is that they won’t happen transactionally, one could succeed even though the other fails.  
      2) Another issue is that cross-partition commands cannot be interleaved (with no exceptions), which means that grain partitions can have deadlocks among each other if they attempt to use commands amongst themselves.   
   4. Alternatively, cross-partition queries are not only safe but even encouraged.   
   5. As a means of circumventing the restriction on cross-partition commands, any necessary transactions that must span partitions should be decoupled with an orchestration or choreography system. We recommend using Temporal to handle these with the durable execution model of long-running transactions.  
   6. If cross-partition commands are always avoided, it should only be possible to create cyclical deadlocks with Remotr where a request causes an infinite call chain. These are much more difficult to cause on accident than the deadlocks which can result from non-interleaving calls happening between multiple grains.
  
4. **Performance Improvements with Remotr**  
   1. Not only can queries safely interleave with both other queries as well as commands, but Remotr introduces safe and straightforward multithreading in the form of having composite states which are formed with multiple grains _(technically, Orleans StatelessWorker grains)_.   
   2. Again, queries against a Manager Grain can interleave with ongoing commands, even when those commands cause state changes. Because of this, queries executed against Manager Grains simply can't cause non-cyclical deadlocks. Queries are also guaranteed to have a consistent view of the grain partition state throughout all inter-partition calls for the entirety of the query execution due to a timestamp managed state system that creates ephemeral copies of the state which allow transactions to manipulate different versions of the state that what queries are viewing.  
   3. However, commands can’t be started if there are ongoing query executions, meaning that if a command is queued, it must wait for all active queries to finish before it can start. This is something that could be fixed in the future. It does imply that, for now, there could be write starvation if a partition constantly has interleaving queries, an unlikely scenario.
   4. **Infinite Cyclical Deadlocks:** These are deadlocks which Remotr can't fix that occur due to an infinite chain of messages that rotates between different grains such as A -> B -> A. It would be fine for a cyclical call to happen as long as the chain is eventually broken; however, if the calls simply go on forever, then there's nothing Remotr will do to fix that.

5. **Child grain placement**\
Because child grains are always technically Stateless Worker Grains (an Orleans feature), they are always co-located with their manager grain, without exception. This means that while a grain partition could theoretically hold many gigabytes of data that isn’t in memory, developers should ensure that a large (many gigabytes in size) grain partition should never be active all at once. Child grains should be instantiated when necessary. If this is unavoidable, then the partition should probably be broken down into more granular parts anyways.
  


## Remotr Docs  
1. **First, you need to know…**

   1. **Orleans**

      1) [https://learn.microsoft.com/en-us/dotnet/orleans/overview](https://learn.microsoft.com/en-us/dotnet/orleans/overview)   

      2) Regular Orleans grains, as well as other Orleans features, can be used alongside Remotr grains. It’s important to understand what a regular grain is before creating Remotr grains.  

   3. **Grain partitions**  

      1) **Definition:** A grain partition is group of grains, all co-located on the same silo, which correspond to an actual database partition, such as a Cosmos partition. This allows commands across a grain partition to happen transactionally.  

      2) **Manager Grains:** each grain partition has a singular Manager Grain, also known as an API Grain, which serves as the only entry point to the partition for all requests. It also manages the transactions which affect the partition as well. While this may *seem* like a bottleneck, it’s important to remember that [all queries against an API grain can be interleaved](?tab=t.0#bookmark=id.dkjyddi41ox7), and grain partitions are also multithreaded since they are composed of many grains working together with each Child Grain in charge of managing its own state.  
      3) **Child Grains:** Also known as Stateful Grains, grain partitions can have a potentially infinite number of child grains associated with them. The only restriction to the number of child grains is how large a grain partition can become which is imposed by the underlying datastore. And as implied by the alternative name “Stateful Grains”, Child Grains are unlike Manager Grains in that they can *only* be accessed by their own Manager Grain and they also can hold and manage state.  
         1) **Important Takeaways:**   
            1) Child Grains can only be accessed by their Manager Grain.  
            2) Two Child Grains with the same type and Id can exist in different grain partitions, and there aren’t any problems with that. The Child Grains are unique per Manager Grain.  

   4. **Designing a Grain Partition**  

      1) **Proper Use Case:** Grain partitions are a perfect fit for any complex entity which could potentially have many gigabytes of underlying data, but only some of it would need to be accessed at any given time. For example, a video game player may have gigabytes of data about their game history, records, preferences, purchases, points, replays, highlights, etc, but only a small amount of that data would need to be active at any given time. Social media users are another great example. Often, browsing history, timelines, reaction history, and other activity data are being constantly recorded, but only a small percentage of that needs to be active during a user session.   

      2) [**Bad Use Case**](#BAD-PRACTICES) part 4.

3. **Remotr Transactions**  
   1. Remotr supports ACID transactions across the states of Child Grains via commands executed against Manager Grains.  


4. **Remotr API documentation**  
   1. CQRS  
      1) Creating a Manager Grain  
      2) Creating a Child Grain  
      3) Creating Commands and Queries  
         1) Any registered services can be DIed into commands and queries  
      4) Reading and Writing State  
         1) Only for Child Grains  
      5) IExternalCommandFactory and IExternalQueryFactory  
      6) The appropriate Query and Command factories are always on the Command and Query handlers.  
         1) Don’t inject external factors into any commands or queries, this is *always* a bad practice and will likely trigger a runtime or build exception in future versions.  
         2) [Other bad practices](#BAD-PRACTICES)  
      7) Call Chaining  
         1) Useful, but perhaps underutilized feature  
         2) Remote execution of method chains  
         3) Reusing methods for common data types  
         4) Note: name originally from this concept of not just RPC, but RCPC (Remotely Chaining Procedural Calls).   
      8) Future API changes  
         1) What metaprogramming could enable.  
      9) Folder Structure  
   2. Grain Keys  
      1) Manager Grains can be addressed with any of the normal Orleans grain key types (Guids, strings, longs, etc.)  
      2) Child Grains are always addressed with a string. This is because Remotr actually uses a JSON string address which combines whatever address you give the Child Grain with the address of the Manager Grain which controls it. This is how different Manager Grains can have Child Grains with identical string keys without any conflicts happening.  
   3. Persistence  
      1) Cosmos is the only supported database right now  
      2) Setting up Cosmos  
   4. Extension Methods  
      1) Extension methods are an extremely useful way to share functionality between different commands or queries that operate on the same Manager or Child Grain.  
   5. Other Useful Methods  
      1) GetManagerId()  
         1) This shouldn’t be used to call the Manager Grain ever, and it should only be used in rare situations anyways, but it is sometimes useful. The downside of using this is that it’s possible to make Child Grains less reusable for other Manager Grain types if they are tightly coupled with a particular Manager Grain type.  
      2) GetPrimaryKey()  
         1) Differences between Manager and Child Grain C\&Qs.  
5. Orleans Feature Differences  
   1. Summary: While many Orleans features can be used with Remotr, there are some that specifically don’t work within Grain Partitions.  
   2. Pitfalls of Remotr  
      1. No possibility for sharing in-memory state between commands and queries other than the state of a Child Grain itself.  
         1) Note: this is something that will most likely be added in the future.  
   3. Reminders/Timers (don’t do it)



## BAD PRACTICES 

1. **Injecting Command/Query factories into a Command or Query:** Don’t inject the ExternalCommandFactory or any other factories into a Command or Query. If a command or query doesn’t have access to a type of factory, this is by design. Only use the CommandFactory or QueryFactory which is found in the commands and queries by default.  

2. **Cross Partition Commands:** NEVER run cross-partition commands. This shouldn’t even be possible without injecting the ExternalCommandFactory into a Command or Query.  

3. **Child to Self-Manager Calls:** Calling the Manager Grain of a Child Grain from the Child Grain itself. This is an inversion of the expected control flow, and usually makes it impossible to reuse Child Grains for other types of Manager Grains. It will also cause a deadlock (on itself) if you run a command against the Manager Grain from one of that Manager Grain’s Child Grains.

4. **Large Grain Partitions:** Having a huge grain partition that will likely have much of the partition in memory at once (by nature of having ChildGrains called). Because grain partitions exist altogether on a single silo, this will likely cause performance issues.
   - **Example:** The IoT security sensors for an entire building should most likely not go directly through a single grain partition, even if it would be convenient for them to do so. This is assuming that they are constantly feeding data, which would slow the grain partition to a halt given hundreds of commands per second. That’s not to say that Remotr couldn’t be utilized, but it’s important to design the grain partitions with this in mind.  



## Common Useful Remotr Patterns  
1. **Singleton Child Grain IDs:** TODO
2. **Empty Commands:** TODO


## TODO: Testing with Remotr  
## TODO: Examples
