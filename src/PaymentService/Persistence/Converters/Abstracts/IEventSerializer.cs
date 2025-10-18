using PaymentService.Infrastructures.Contexts.StoreEntity;
using Shared.Core.Events;

namespace PaymentService.Persistence.Converters.Abstracts
{
    public interface IEventSerializer
    {
        IDomainEvent ToDomain(PaymentStore row); 
        (string Type, string PayloadJson, string? MetadataJson) ToStored(IDomainEvent @event); 
    }
}
