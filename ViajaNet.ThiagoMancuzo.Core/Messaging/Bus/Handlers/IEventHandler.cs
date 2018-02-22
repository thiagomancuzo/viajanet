using System.Threading.Tasks;

namespace ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.Handlers
{
    public interface IEventHandler<TEvent>
        where TEvent : Event
    {
        Task Handle(TEvent @event);
    }
}
