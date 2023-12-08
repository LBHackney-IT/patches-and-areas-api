using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Hackney.Shared.PatchesAndAreas.Domain;
using Hackney.Shared.PatchesAndAreas.Infrastructure;
using PatchesAndAreasApi.V1.Domain;
using PatchesAndAreasApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using EventData = PatchesAndAreasApi.V1.Domain.EventData;

namespace PatchesAndAreasApi.V1.Factories
{
    public class PatchSnsFactory : ISnsFactory
    {
        public PatchesAndAreasSns Update(PatchesDb updateResult, Token token, ResponsibleEntities previousResponsibleEntity)
        {
            return new PatchesAndAreasSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = updateResult.Id,
                Id = Guid.NewGuid(),
                EventType = PatchOrAreaResEntityEditedEventConstants.EVENTTYPE,
                Version = PatchOrAreaResEntityEditedEventConstants.V1VERSION,
                SourceDomain = PatchOrAreaResEntityEditedEventConstants.SOURCEDOMAIN,
                SourceSystem = PatchOrAreaResEntityEditedEventConstants.SOURCESYSTEM,
                User = new User
                {
                    Name = token.Name,
                    Email = token.Email
                },
                EventData = new EventData
                {
                    OldValues = previousResponsibleEntity,
                    NewValues = updateResult.ResponsibleEntities.FirstOrDefault()
                }
            };
        }
    }
}
