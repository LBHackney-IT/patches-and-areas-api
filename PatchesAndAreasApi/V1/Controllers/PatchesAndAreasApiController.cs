using PatchesAndAreasApi.V1.Boundary.Response;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PatchesAndAreasApi.V1.UseCase.Interfaces;
using PatchesAndAreasApi.V1.Boundary.Request;
using System.Threading.Tasks;
using PatchesAndAreasApi.V1.Factories;
using System.Net.Http.Headers;
using PatchesAndAreasApi.V1.Infrastructure;

namespace PatchesAndAreasApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/patch")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class PatchesAndAreasApiController : BaseController
    {
        private readonly IGetPatchByIdUseCase _getByIdUseCase;
        public PatchesAndAreasApiController(IGetPatchByIdUseCase getByIdUseCase)
        {
            _getByIdUseCase = getByIdUseCase;
        }


        /// <summary>
        /// Retrives the Patch record corresponding to the supplied id
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid id value supplied</response>
        /// <response code="404">No patch found for the specified id</response>
        /// <response code="500">Something went wrong</response>
        [ProducesResponseType(typeof(PatchesResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        [Route("{id}")]
        public async Task<IActionResult> GetPatchById([FromRoute] PatchesQueryObject query)
        {
            var patch = await _getByIdUseCase.Execute(query).ConfigureAwait(false);
            if (patch == null) return NotFound(query.Id);

            var eTag = string.Empty;
            if (patch.VersionNumber.HasValue)
                eTag = patch.VersionNumber.ToString();

            HttpContext.Response.Headers.Add(HeaderConstants.ETag, EntityTagHeaderValue.Parse($"\"{eTag}\"").Tag);

            return Ok(patch.ToResponse());
        }
    }
}
