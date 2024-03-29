using FluentAssertions;
using Newtonsoft.Json;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
using Hackney.Shared.PatchesAndAreas.Infrastructure;
using Hackney.Shared.PatchesAndAreas.Infrastructure.Constants;
using PatchesAndAreasApi.Tests.V1.E2ETests.Fixtures;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.Tests.V1.E2ETests.Steps
{
    public class UpdatePatchResponsibilityStep : BaseSteps
    {
        public UpdatePatchResponsibilityStep(HttpClient httpClient) : base(httpClient)
        { }

        /// <summary>
        /// You can use jwt.io to decode the token - it is the same one we'd use on dev, etc.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="responsibileEntityId"></param>
        /// <param name="requestObject"></param>
        /// <param name="ifMatch"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> CallAPI(Guid id, Guid responsibileEntityId, UpdatePatchesResponsibilitiesRequestObject requestObject, int? ifMatch)
        {
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";
            var uri = new Uri($"api/v1/patch/{id}/responsibleEntity/{responsibileEntityId}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Content = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Patch;
            message.Headers.Add("Authorization", token);
            message.Headers.TryAddWithoutValidation(HeaderConstants.IfMatch, $"\"{ifMatch?.ToString()}\"");


            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }

        public async Task WhenTheUpdatePatchApiIsCalled(Guid id, Guid responsibileEntityId, UpdatePatchesResponsibilitiesRequestObject requestObject)
        {
            await WhenTheUpdatePatchApiIsCalled(id, responsibileEntityId, requestObject, 0).ConfigureAwait(false);
        }

        public async Task WhenTheUpdatePatchApiIsCalled(Guid id, Guid responsibileEntityId, UpdatePatchesResponsibilitiesRequestObject requestObject, int? ifMatch)
        {
            _lastResponse = await CallAPI(id, responsibileEntityId, requestObject, ifMatch).ConfigureAwait(false);

        }

        public async Task ThenANewResponsibilityEntityIsAdded(PatchesFixtures patchFixture, Guid responsibileEntityId, UpdatePatchesResponsibilitiesRequestObject request)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var result = await patchFixture._dbContext.LoadAsync<PatchesDb>(patchFixture.PatchesDb.Id).ConfigureAwait(false);

            result.Should().BeEquivalentTo(patchFixture.PatchesDb,
                                           c => c.Excluding(y => y.ResponsibleEntities)
                                                 .Excluding(z => z.VersionNumber));
            result.VersionNumber.Should().Be(1);

            var expected = new ResponsibleEntities()
            {
                Id = responsibileEntityId,
                Name = request.Name,
                ResponsibleType = request.ResponsibleType
            };
            result.ResponsibleEntities.Should().ContainEquivalentOf(expected);
            result.ResponsibleEntities.Except(result.ResponsibleEntities.Where(x => x.Id == responsibileEntityId))
                                   .Should()
                                   .BeEquivalentTo(patchFixture.PatchesDb.ResponsibleEntities);

            async void Item() => await patchFixture._dbContext.DeleteAsync<PatchesDb>(result.Id).ConfigureAwait(false);

            _cleanup.Add(Item);
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
