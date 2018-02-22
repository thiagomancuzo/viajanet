using System;
using System.Collections.Generic;
using System.Text;

namespace ViajaNet.ThiagoMancuzo.Core.Messaging
{
    public abstract class Event
    {
        protected Event()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public DateTime CreatedAt { get; }
    }
}
