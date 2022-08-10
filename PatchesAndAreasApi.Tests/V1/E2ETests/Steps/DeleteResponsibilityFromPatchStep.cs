using FluentAssertions;
using PatchesAndAreas.Boundary.Request;
using PatchesAndAreas.Infrastructure;
using PatchesAndAreasApi.Tests.V1.E2ETests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.Tests.V1.E2ETests.Steps
{
    public class DeleteResponsibilityFromPatchStep : BaseSteps
    {
        public DeleteResponsibilityFromPatchStep(HttpClient httpClient) : base(httpClient)
        { }

        public async Task WhenDeleteResponsibilityFromPatchApiIsCalledAsync(DeleteResponsibilityFromPatchRequest query)
        {
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";

            // setup request
            var uri = new Uri($"api/v1/patch/{query.Id}/responsibleEntity/{query.ResponsibileEntityId}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            message.Method = HttpMethod.Delete;
            message.Headers.Add("Authorization", token);

            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _lastResponse = await _httpClient.SendAsync(message).ConfigureAwait(false);
        }

        public void NotFoundResponseReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void NoContentResponseReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        public async Task ResponsibilityRemovedFromPatch(Guid id, Guid responsibilityId, PatchesFixtures fixture)
        {
            var patch = await fixture._dbContext.LoadAsync<PatchesDb>(id).ConfigureAwait(false);

            patch.ResponsibleEntities.Should().NotContain(x => x.Id == responsibilityId);
        }


    }
}
