using PatchesApi.V1.Domain;
using PatchesApi.V1.Infrastructure;

namespace PatchesApi.V1.Factories
{
    public static class EntityFactory
    {
        public static PatchEntity ToDomain(this PatchesDb databaseEntity)
        {

            return new PatchEntity
            {
                Id = databaseEntity.Id,
                ParentId = databaseEntity.ParentId,
                Name = databaseEntity.Name,
                Domain = databaseEntity.Domain,
                PatchType = databaseEntity.PatchType,
                ResponsibleEntities = databaseEntity.ResponsibleEntities,
                VersionNumber = databaseEntity.VersionNumber
            };
        }



        public static PatchesDb ToDatabase(this PatchEntity entity)
        {

            return new PatchesDb
            {
                Id = entity.Id,
                ParentId = entity.ParentId,
                Name = entity.Name,
                Domain = entity.Domain,
                PatchType = entity.PatchType,
                ResponsibleEntities = entity.ResponsibleEntities.ToListOrEmpty(),
                VersionNumber = entity.VersionNumber
            };
        }
    }
}
