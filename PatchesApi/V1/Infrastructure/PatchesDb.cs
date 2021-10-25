using Amazon.DynamoDBv2.DataModel;
using Hackney.Core.DynamoDb.Converters;
using PatchesApi.V1.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatchesApi.V1.Infrastructure
{
    [DynamoDBTable("Patches", LowerCamelCaseProperties = true)]
    public class PatchesDb
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty]
        public Guid ParentId { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbEnumConverter<PatchType>))]
        public PatchType PatchType { get; set; }

        [DynamoDBProperty]
        public string Domain { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<ResponsibleEntities>))]
        public ResponsibleEntities ResponsibleEntities { get; set; }

    }
}
