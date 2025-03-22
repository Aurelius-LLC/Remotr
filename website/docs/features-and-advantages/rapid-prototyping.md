---
sidebar_position: 1
---

# Rapid Prototyping with Remotr

Remotr enables rapid prototyping for applications that need robust, distributed state management while maintaining data integrity, structure, and cost-effective scalability.

## Key Benefits

### Simplified Application Architecture

- **CQRS Without the Complexity**: Remotr implements Command Query Responsibility Segregation patterns without requiring separate read/write databases. The CQRS pattern in Remotr reduces actor deadlocks and allows for performance improvements through query interleaving TODO: Link to [Simplified Concurrency](./simplified-concurrency.md).
- **Transactional Guarantees**: Build with confidence knowing that commands across an aggregate will succeed or fail together, ensuring data consistency.

### Reduced Development Friction

- **Compositional State Management**: Rapidly build complex domain models through composition with the ability to leverage underlying database features.
  - TODO: Link to [Compositional State](./compositional-state.md)

- **Interleaved Queries**: Queries can safely run alongside commands without blocking, improving responsiveness during development.
  - TODO: Link to [Simplified Concurrency](./simplified-concurrency.md)

- **Eliminate Common Actor Deadlocks**: Remotr's design prevents most actors deadlocks through its contagious CQRS model and aggregate-based architecture.
  - TODO: Link to [Deadlock Reduction](./deadlock-reduction.md)

### Accelerated Development Cycles

- **Source Generation**: Minimize boilerplate through code generation.
  - TODO: Link to [Source Generation](../developing-aggregates/source-generation.md)

- **Shared Functionality**: Reduce code duplication between commands and queries with extension methods or by using commands and queries with type constraints. TODO: Link to [Shared Code](../advanced/shared-code.md)

- **Built-in Scalability**: Start small and scale seamlessly as your prototype grows into a production system.
  - TODO: Link to [Built-in Scaling](./built-in-scaling.md)
