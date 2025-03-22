---
sidebar_position: 2
---

# Simplified Concurrency

Remotr provides an elegant approach to concurrency management in distributed systems, addressing common challenges while maintaining high performance and data integrity.

## Understanding Aggregates in Remotr

**Definition of Virtual actors:**\
Distributed objects with several special properties:
- Single-Threaded
- Use a queue to process incoming requests in total isolation
- Are referenced agnostic to their location within a distributed cluster (the routing of requests is abstracted away from the developer)
- Have fully managed lifecycles, meaning developers never "create", "instantiate", "delete", or "destroy" virtual actors.

Remotr implements the virtual actor model through two key concepts: Aggregate Roots and Entities. Both are virtual actors, but they work together in a hierarchical way to provide a powerful and intuitive programming model.

### Aggregate Architecture

In Remotr, every piece of state belongs to an Aggregate:

1. **Aggregate Root**: A virtual actor that acts as the gateway to the aggregate, controlling access to all entities within it
2. **Aggregate Entities**: Individual actors that belong to an aggregate, each managing its own specific state
3. **Co-location**: Entities are always located on the same server as their Aggregate Root, enabling efficient communication

[Diagram: Remotr Aggregate Architecture]
<!-- Insert diagram showing:
     - Aggregate Root with its message queue
     - Multiple Entity actors within the aggregate
     - Message flow from external sources through Root to Entities
     - Co-location on a single server -->

### Message Processing and Concurrency

Remotr provides a unique blend of safety and performance through its message processing model:

1. **Aggregate-Level Ordering**: 
   - The Aggregate Root's message queue determines the overall order of requests to the aggregate
   - This ensures consistent processing of operations that affect multiple entities

2. **Entity-Level Parallelism**:
   - Each Entity is its own virtual actor with an independent message queue
   - Entities can process messages concurrently with other entities in the same aggregate
   - Single-threaded processing and request isolation per entity preserves state consistency as entities can't "share" state.

3. **Location Agnosticism**:
   - Developers work with Aggregate Roots and Entities through their logical identities
   - The runtime automatically handles placement and routing of messages
   - Aggregates are automatically placed within the distributed environment

### Key Benefits

- **Safe Concurrency by Design**: 
  - Entity state is protected by single-threaded processing
  - Aggregate-level consistency is guaranteed by the Root's message queue
  - Multi-threading within aggregates happens automatically where safe

- **Simplified Development**:
  - No need to manage actor lifecycle - the runtime handles creation and cleanup
  - No need to think about placement - aggregates are distributed automatically
  - Natural modeling of domain concepts as Roots and Entities

- **Scalability**:
  - Aggregates can be distributed across a cluster of servers
  - New servers can be added seamlessly

## CQRS-Powered Interleaving

*"Interleaving"* within an actor is when an actor allows for multiple requests to be processed at once; however, actors still retain their single-threadedness.\
\
Remotr implements a Command Query Responsibility Segregation (CQRS) pattern that allows commands and queries to operate with minimal interference:

- **Interleaved Queries**: Because queries are guaranteed to never result in state changes, queries are always able to interleave with ongoing commands or other queries. 
- **Transactional Isolation**: Commands are properly isolated using transaction metadata to maintain data consistency, meaning that queries will always view a consistent version of the Aggregate's state.

## Deadlock Prevention

Traditional actor systems often suffer from deadlocks when actors need to communicate with each other. Remotr's design prevents most deadlocks by organizing related entities into aggregates with clear boundaries and allowing for interleaving queries across different aggregates.

## Simplified Development Model

Remotr's concurrency model simplifies development by:

- **Compositional State Management**: Build complex domain models through composition while maintaining concurrency safety
- **Query/Command Separation**: Clean separation of read and write operations without requiring separate databases
- **Single-threaded Execution**: Aggregate roots and entities never execute on more than one thread simultaneously, eliminating the need for locks.

## Performance Benefits

The concurrency model in Remotr provides several performance advantages:

- **Improved Throughput**: Non-blocking queries allow the system to handle more requests concurrently
- **Reduced Latency**: Queries don't wait for commands to complete before executing
- **Scalability**: The architecture allows for horizontal scaling while maintaining consistency guarantees