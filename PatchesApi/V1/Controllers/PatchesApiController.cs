using PatchesApi.V1.Boundary.Response;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PatchesApi.V1.UseCase.Interfaces;
using PatchesApi.V1.Boundary.Request;
using System.Threading.Tasks;
using PatchesApi.V1.Factories;
using System.Net.Http.Headers;
using PatchesApi.V1.Infrastructure;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Hackney.Core.Middleware;
using HeaderConstants = PatchesApi.V1.Infrastructure.HeaderConstants;
using PatchesApi.V1.Infrastructure.Exceptions;

namespace PatchesApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/patch")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class PatchesApiController : BaseController
    {
        private readonly IGetPatchByIdUseCase _getByIdUseCase;
        private readonly IUpdatePatchResponsibilitiesUseCase _updatePatchResponsibilities;
        private readonly IHttpContextWrapper _contextWrapper;

        public PatchesApiController(IGetPatchByIdUseCase getByIdUseCase, IUpdatePatchResponsibilitiesUseCase updatePatchResponsibilities,
            IHttpContextWrapper contextWrapper)
        {
            _getByIdUseCase = getByIdUseCase;
            _updatePatchResponsibilities = updatePatchResponsibilities;
            _contextWrapper = contextWrapper;
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

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch]
        [Route("{id}/responsibleEntity/{responsibileEntityId}")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> UpdatePatchForResponsibility([FromRoute] UpdatePatchesResponsibilityRequest query, [FromBody] UpdatePatchesResponsibilitiesRequestObject requestObject)
        {
            var contextHeaders = _contextWrapper.GetContextRequestHeaders(HttpContext);
            var ifMatch = GetIfMatchFromHeader();

            try
            {
                // We use a request object AND the raw request body text because the incoming request will only contain the fields that changed
                // whereas the request object has all possible updateable fields defined.
                // The implementation will use the raw body text to identify which fields to update and the request object is specified here so that its
                // associated validation will be executed by the MVC pipeline before we even get to this point.
                var patch = await _updatePatchResponsibilities.ExecuteAsync(query, requestObject, ifMatch)
                                                                .ConfigureAwait(false);
                if (patch == null) return NotFound(query.Id);
                return NoContent();
            }
            catch (VersionNumberConflictException vncErr)
            {
                return Conflict(vncErr.Message);
            }
            catch(ResponsibilityEntityException reErr)
            {
                return Conflict(reErr.Message);
            }
        }

        private int? GetIfMatchFromHeader()
        {
            var header = HttpContext.Request.Headers.GetHeaderValue(HeaderConstants.IfMatch);

            if (header == null)
                return null;

            _ = EntityTagHeaderValue.TryParse(header, out var entityTagHeaderValue);

            if (entityTagHeaderValue == null)
                return null;

            var version = entityTagHeaderValue.Tag.Replace("\"", string.Empty);

            if (int.TryParse(version, out var numericValue))
                return numericValue;

            return null;
        }
    }
}
