using Hackney.Core.JWT;
using Hackney.Shared.PatchesAndAreas.Infrastructure;
using PatchesAndAreasApi.V1.Domain;
using PatchesAndAreasApi.V1.Infrastructure;

namespace PatchesAndAreasApi.V1.Factories
{
    public interface ISnsFactory
    {
        PatchesAndAreasSns Update(PatchesDb updateResult, Token token);
    }
}
