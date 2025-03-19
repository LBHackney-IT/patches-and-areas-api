using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Hackney.Core.DynamoDb;
using Hackney.Core.Sns;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Sns;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatchesAndAreasApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace PatchesAndAreasApi.Tests
{
    public class MockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly List<TableDef> _tables = new List<TableDef>
        {
            new TableDef {
                Name = "PatchesAndAreas",
                KeyName = "id",
                KeyType = ScalarAttributeType.S,
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>(new[]
                {
                    new GlobalSecondaryIndex
                    {
                        IndexName = "PatchByParentId",
                        KeySchema = new List<KeySchemaElement>(new[]
                        {
                            new KeySchemaElement("parentId", KeyType.HASH)
                        }),
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 10,
                            WriteCapacityUnits = 10
                        },
                        Projection = new Projection { ProjectionType = ProjectionType.ALL }
                    },

                    new GlobalSecondaryIndex
                    {
                        IndexName = "PatchByPatchName",
                        KeySchema = new List<KeySchemaElement>(new[]
                        {
                            new KeySchemaElement("patchName", KeyType.HASH)
                        }),
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 10,
                            WriteCapacityUnits = 10
                        },
                        Projection = new Projection { ProjectionType = ProjectionType.ALL }
                    }
                })
            }
        };

        public HttpClient Client { get; private set; }
        public IDynamoDbFixture DynamoDbFixture { get; private set; }

        public ISnsFixture SnsFixture { get; private set; }


        public MockWebApplicationFactory()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");
            EnsureEnvVarConfigured("DynamoDb_LocalServiceUrl", "http://localhost:8000");
            EnsureEnvVarConfigured("Sns_LocalMode", "true");
            EnsureEnvVarConfigured("Localstack_SnsServiceUrl", "http://localhost:4566");

            EnsureEnvVarConfigured("AWS_REGION", "eu-west-2");
            EnsureEnvVarConfigured("AWS_ACCESS_KEY_ID", "local");
            EnsureEnvVarConfigured("AWS_SECRET_ACCESS_KEY", "local");
            EnsureEnvVarConfigured("ASSET_ADMIN_GROUPS", "e2e-testing");

            Client = CreateClient();
        }

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (null != DynamoDbFixture)
                    (DynamoDbFixture as DynamoDbFixture).Dispose();
                if (null != SnsFixture)
                    SnsFixture.Dispose();
                if (null != Client)
                    Client.Dispose();

                base.Dispose(true);

                _disposed = true;
            }
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<Startup>();
            builder.ConfigureServices(services =>
            {
                services.ConfigureDynamoDB();
                services.ConfigureDynamoDbFixture();

                services.ConfigureSns();
                services.ConfigureSnsFixture();

                var serviceProvider = services.BuildServiceProvider();

                DynamoDbFixture = serviceProvider.GetRequiredService<IDynamoDbFixture>();
                DynamoDbFixture.EnsureTablesExist(_tables);

                SnsFixture = serviceProvider.GetRequiredService<ISnsFixture>();
                SnsFixture.CreateSnsTopic<PatchesAndAreasSns>("patchesandareas.fifo", "PATCHES_AND_AREAS_SNS_ARN");
            });
        }
    }
}
