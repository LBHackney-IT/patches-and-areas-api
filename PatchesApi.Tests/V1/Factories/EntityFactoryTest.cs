using AutoFixture;
using PatchesApi.V1.Domain;
using PatchesApi.V1.Factories;
using PatchesApi.V1.Infrastructure;
using FluentAssertions;
using Xunit;

namespace PatchesApi.Tests.V1.Factories
{
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanMapADatabaseEntityToADomainObject()
        {
            //Arrange
            var databaseEntity = _fixture.Create<PatchesDb>();
            //Act
            var entity = databaseEntity.ToDomain();
            //Assert
            databaseEntity.Id.Should().Be(entity.Id);
            databaseEntity.Name.Should().Be(entity.Name);
            databaseEntity.ParentId.Should().Be(entity.ParentId);
            databaseEntity.PatchType.Should().Be(entity.PatchType);
            databaseEntity.ResponsibleEntities.Should().BeEquivalentTo(entity.ResponsibleEntities);
        }


        [Fact]
        public void CanMapADomainEntityToADatabaseObject()
        {
            //Arrange
            var entity = _fixture.Create<PatchEntity>();
            //Act
            var databaseEntity = entity.ToDatabase();
            //Assert
            entity.Id.Should().Be(databaseEntity.Id);
            entity.Name.Should().Be(databaseEntity.Name);
            entity.ParentId.Should().Be(databaseEntity.ParentId);
            entity.PatchType.Should().Be(databaseEntity.PatchType);
            entity.ResponsibleEntities.Should().BeEquivalentTo(databaseEntity.ResponsibleEntities);
        }
    }
}
