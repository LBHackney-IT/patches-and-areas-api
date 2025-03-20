using FluentAssertions;
using Hackney.Shared.PatchesAndAreas.Boundary.Response;
using Hackney.Shared.PatchesAndAreas.Infrastructure;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace PatchesAndAreasApi.Tests.V1.E2ETests.Steps
{
    public class GetByPatchNameStep : BaseSteps
    {
        public GetByPatchNameStep(HttpClient httpClient) : base(httpClient)
        { }

        public async Task WhenPatchDetailsAreRequested(string patchName)
        {
            var uri = new Uri($"api/v1/patch/patchName/{patchName}", UriKind.Relative);
            _lastResponse = await _httpClient.GetAsync(uri).ConfigureAwait(false);
        }

        public async Task ThenThePatchDetailsAreReturned(PatchesDb patchesDb)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiPatch = JsonSerializer.Deserialize<PatchesResponseObject>(responseContent, CreateJsonOptions());

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
