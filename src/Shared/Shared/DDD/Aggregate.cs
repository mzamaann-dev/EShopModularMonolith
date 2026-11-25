using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DDD;

public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvents(IDomainEvent domainEvent)
    { _domainEvents.Add(domainEvent); }

    public IDomainEvent[] ClearDomainEvents()
    {
        //returns cleared events
        var dequeuedEvents = _domainEvents.ToArray();
        _domainEvents.Clear();
        return dequeuedEvents;
    }
}
