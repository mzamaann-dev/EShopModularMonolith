using Catalog.Products.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Products.EventHandlers;

public class ProductPriceChangeEventHandler : INotificationHandler<ProductPriceChangedEvent>
{
    private readonly ILogger<ProductPriceChangeEventHandler> _logger;

    public ProductPriceChangeEventHandler(ILogger<ProductPriceChangeEventHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}
