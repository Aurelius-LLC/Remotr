---
sidebar_position: 6
---

# Bad Practices

## Common Pitfalls to Avoid

When prototyping with Remotr, be aware of these anti-patterns:

- **Cross-Aggregate Commands**: Never run commands across aggregates; use orchestration instead.
- **Large Aggregates**: Avoid designing aggregates that will have too much state in memory at once. Break down large aggregates into more granular parts.
- **Entity to Self-Root Calls**: Don't call the AggregateRoot from its Entities as this creates tight coupling and potential deadlocks.
- **Injecting Command/Query Factories**: Don't inject external factories into Commands or Queries - use the built-in factories instead.
- **Not Awaiting Entity Commands**: Always await Entity commands to ensure transactional state updates.

For more details on these and other considerations, see the full [README documentation](../README.md).
