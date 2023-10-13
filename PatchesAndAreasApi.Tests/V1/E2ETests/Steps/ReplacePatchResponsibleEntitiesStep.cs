using FluentAssertions;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
using Hackney.Shared.PatchesAndAreas.Infrastructure;
using Newtonsoft.Json;
using PatchesAndAreasApi.Tests.V1.E2ETests.Fixtures;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using Hackney.Shared.PatchesAndAreas.Infrastructure.Constants;
using System.Collections.Generic;

namespace PatchesAndAreasApi.Tests.V1.E2ETests.Steps
{
    public class ReplacePatchResponsibleEntitiesStep : BaseSteps
    {
        public ReplacePatchResponsibleEntitiesStep(HttpClient httpClient) : base(httpClient)
        { }

        /// <summary>
        /// You can use jwt.io to decode the token - it is the same one we'd use on dev, etc. 
        /// </summary>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CallAPI(Guid id, List<ResponsibleEntities> responsibleEntities, int? ifMatch)
        {
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";
            var uri = new Uri($"api/v1/patch/{id}/responsibleEntity", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Content = new StringContent(JsonConvert.SerializeObject(responsibleEntities), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Patch;
            message.Headers.Add("Authorization", token);
            message.Headers.TryAddWithoutValidation(HeaderConstants.IfMatch, $"\"{ifMatch?.ToString()}\"");


            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }

        public async Task WhenTheReplaceResponsibilityEntityApiIsCalled(Guid id, List<ResponsibleEntities> responsibleEntities)
        {
            await WhenTheReplaceResponsibilityEntityApiIsCalled(id, responsibleEntities, 0).ConfigureAwait(false);
        }

        public async Task WhenTheReplaceResponsibilityEntityApiIsCalled(Guid id, List<ResponsibleEntities> responsibleEntities, int? ifMatch)
        {
            _lastResponse = await CallAPI(id, responsibleEntities, ifMatch).ConfigureAwait(false);

        }

        public async Task ThenTheResponsibilityEntityIsReplacedWithEntitySentFromClient(PatchesFixtures patchFixture, List<ResponsibleEntities> responsibleEntities, ResponsibleEntities responsibleEntity)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var result = await patchFixture._dbContext.LoadAsync<PatchesDb>(patchFixture.PatchesDb.Id).ConfigureAwait(false);

            result.Should().BeEquivalentTo(patchFixture.PatchesDb,
                                           c => c.Excluding(y => y.ResponsibleEntities)
                                                 .Excluding(z => z.VersionNumber));
            result.VersionNumber.Should().Be(1);


            result.ResponsibleEntities.Should().BeEquivalentTo(responsibleEntities);

            _cleanup.Add(async () => await patchFixture._dbContext.DeleteAsync<PatchesDb>(result.Id).ConfigureAwait(false));
        }

        public async Task ThenConflictIsReturned(int? versionNumber)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            var sentVersionNumberString = (versionNumber is null) ? "{null}" : versionNumber.ToString();
            responseContent.Should().Contain($"The version number supplied ({sentVersionNumberString}) does not match the current value on the entity (0).");
        }

        public void ThenBadRequestIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void ThenNotFoundIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
