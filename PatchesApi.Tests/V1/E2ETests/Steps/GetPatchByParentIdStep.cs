using FluentAssertions;
using PatchesApi.V1.Boundary.Response;
using PatchesApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PatchesApi.Tests.V1.E2ETests.Steps
{
    public class GetPatchByParentIdStep : BaseSteps
    {
        public GetPatchByParentIdStep(HttpClient httpClient) : base(httpClient)
        { }

        public async Task WhenPatchDetailsAreRequested(string parentId)
        {
            var uri = new Uri($"api/v1/patch?parentId={parentId}", UriKind.Relative);
            _lastResponse = await _httpClient.GetAsync(uri).ConfigureAwait(false);
        }

        public async Task ThenThePatchDetailsAreReturned(List<PatchesDb> patchesDb)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiPatch = JsonSerializer.Deserialize<List<PatchesResponseObject>>(responseContent, CreateJsonOptions());

            apiPatch.Should().BeEquivalentTo(patchesDb, config => config.Excluding(x => x.VersionNumber));

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
