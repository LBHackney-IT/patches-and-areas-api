using Hackney.Core.Sns;
using Hackney.Shared.PatchesAndAreas.Domain;
using System;
using System.Collections.Generic;

namespace PatchesAndAreasApi.V1.Domain
{
    public class PatchesAndAreasSns
    {
        public Guid Id { get; set; }

        public string EventType { get; set; }

        public string SourceDomain { get; set; }

        public string SourceSystem { get; set; }

        public string Version { get; set; }

        public Guid CorrelationId { get; set; }

        public DateTime DateTime { get; set; }

        public User User { get; set; }

        public Guid EntityId { get; set; }

        public EventData EventData { get; set; } = new EventData();
    }

    public class EventData
    {
        public object OldData { get; set; }
        public object NewData { get; set; }

    }
}
