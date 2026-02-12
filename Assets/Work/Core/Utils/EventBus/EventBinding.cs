using System;

namespace Work.Core.Utils.EventBus
{
    public interface IEventBinding : IDisposable
    {
        bool IsBound { get; }
        void Unbind();
    }

    public sealed class EventBinding<T> : IEventBinding where T : IEvent
    {
        private Action<T> _handler;
        private bool _isBound;

        public bool IsBound => _isBound;

        public EventBinding(Action<T> handler)
        {
            _handler = handler;
            _isBound = true;
            Bus<T>.Events += _handler;
        }

        public void Unbind()
        {
            if (!_isBound) return;
            _isBound = false;

            if (_handler != null)
            {
                Bus<T>.Events -= _handler;
                _handler = null;
            }
        }

        public void Dispose()
        {
            Unbind();
        }
    }

    public static class EventBinding
    {
        public static IEventBinding Bind<T>(Action<T> handler) where T : IEvent
        {
            return new EventBinding<T>(handler);
        }
    }
}

