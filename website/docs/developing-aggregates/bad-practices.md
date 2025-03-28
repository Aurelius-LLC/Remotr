---
sidebar_position: 6
---

# Bad Practices

## Common Pitfalls to Avoid

When creating with Remotr, be aware of these anti-patterns:

- **Injecting Command/Query Factories into Remotr handlers**: Don't inject external factories into Commands or Queries - use the built-in factories instead. This is *extremely* important as the internal factories have safeguards that injected factories lack.
- **Cross-Aggregate Commands**: Never run commands across aggregates; use orchestration/choreography instead. This can't happen anyways as long as external command factories aren't DIed into Remotr handlers. This is by design because distributed transactions aren't naturally supported through Remotr, and calling commands across aggregates could also result in deadlocks that are difficult to debug.
- **Large Aggregates**: Avoid designing aggregates that will have too much state in memory at once. Break down large aggregates into more granular parts. However, it's important to note that the state of an aggregate is loaded on demand and naturally disposed of through the lifecycle of its entities. This means that an aggregate could theoretically be responsible for many gigabytes of data without being problematic as long as only a portion of entities will be active at any one time. Read more about this [here](../features-and-advantages/compositional-state.md).
- **Entity to Self-Root Calls**: Don't call the aggregate root from its entities as this creates tight coupling and potential deadlocks. Entities should be reusable across different aggregate types. Consider the case of a message system where you may have friend chats, group chats, and even live chats, all of which could depend on the same fundamental building blocks (entities) but have different root types depending on the use case.
- **Not Awaiting Entity Commands**: Always await Entity commands to ensure data consistency.
