using FluentAssertions;
using Hackney.Shared.PatchesAndAreas.Boundary.Response;
using Hackney.Shared.PatchesAndAreas.Infrastructure;
using Hackney.Shared.PatchesAndAreas.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.Tests.V1.E2ETests.Steps
{
    public class GetPatchByIdStep : BaseSteps
    {
        public GetPatchByIdStep(HttpClient httpClient) : base(httpClient)
        { }

        public async Task WhenPatchDetailsAreRequested(string id)
        {
            var uri = new Uri($"api/v1/patch/{id}", UriKind.Relative);
            _lastResponse = await _httpClient.GetAsync(uri).ConfigureAwait(false);
        }

        public async Task ThenThePatchDetailsAreReturned(PatchesDb patchesDb)
        {
            //_lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiPatch = JsonSerializer.Deserialize<PatchesResponseObject>(responseContent, CreateJsonOptions());

            apiPatch.Should().BeEquivalentTo(patchesDb, config => config.Excluding(y => y.VersionNumber));
            var expectedEtagValue = $"\"{patchesDb.VersionNumber}\"";
            _lastResponse.Headers.ETag.Tag.Should().Be(expectedEtagValue);
            var eTagHeaders = _lastResponse.Headers.GetValues(HeaderConstants.ETag);
            eTagHeaders.Count().Should().Be(1);
            eTagHeaders.First().Should().Be(expectedEtagValue);
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
