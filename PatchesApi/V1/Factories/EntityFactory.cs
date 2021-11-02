using PatchesApi.V1.Domain;
using PatchesApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

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

        public static List<PatchEntity> ToDomain(this IEnumerable<PatchesDb> databaseEntity)
        {
            return databaseEntity.Select(p => p.ToDomain())
                                 .ToList();
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
                ResponsibleEntities = entity.ResponsibleEntities,
                VersionNumber = entity.VersionNumber
            };
        }

        public static List<PatchesDb> ToDatabase(this IEnumerable<PatchEntity> domain)
        {
            return domain.Select(p => p.ToDatabase())
                                 .ToList();
        }


    }
}
