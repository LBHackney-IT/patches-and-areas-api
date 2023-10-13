using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PatchesAndAreasApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Hackney.Core.Http;
using System.Collections.Generic;
using Hackney.Shared.PatchesAndAreas.Boundary.Response;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Infrastructure.Exceptions;
using Hackney.Shared.PatchesAndAreas.Factories;
using Hackney.Shared.PatchesAndAreas.Infrastructure.Constants;
using Hackney.Core.Middleware;
using HeaderConstants = Hackney.Shared.PatchesAndAreas.Infrastructure.Constants.HeaderConstants;
using Hackney.Shared.PatchesAndAreas.Domain;

namespace PatchesAndAreasApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/patch")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class PatchesAndAreasApiController : BaseController
    {
        private readonly IGetPatchByIdUseCase _getByIdUseCase;
        private readonly IDeleteResponsibilityFromPatchUseCase _deleteResponsibilityFromPatchUseCase;
        private readonly IUpdatePatchResponsibilitiesUseCase _updatePatchResponsibilities;
        private readonly IReplacePatchResponsibleEntitiesUseCase _replacePatchResponsibleEntities;
        private readonly IGetPatchByParentIdUseCase _getPatchByParentIdUseCase;
        private readonly IGetAllPatchesUseCase _getAllPatchesUseCase;
        private readonly IHttpContextWrapper _contextWrapper;

        public PatchesAndAreasApiController(IGetPatchByIdUseCase getByIdUseCase,
                                            IUpdatePatchResponsibilitiesUseCase updatePatchResponsibilities,
                                            IReplacePatchResponsibleEntitiesUseCase replacePatchResponsibleEntitiesUseCase,
                                            IGetPatchByParentIdUseCase getPatchByParentIdUseCase,
                                            IDeleteResponsibilityFromPatchUseCase deleteResponsibilityFromPatchUseCase,
                                            IGetAllPatchesUseCase getAllPatchesUseCase, IHttpContextWrapper contextWrapper)
        {
            _getByIdUseCase = getByIdUseCase;
            _getPatchByParentIdUseCase = getPatchByParentIdUseCase;
            _updatePatchResponsibilities = updatePatchResponsibilities;
            _replacePatchResponsibleEntities = replacePatchResponsibleEntitiesUseCase;
            _deleteResponsibilityFromPatchUseCase = deleteResponsibilityFromPatchUseCase;
            _getAllPatchesUseCase = getAllPatchesUseCase;
            _contextWrapper = contextWrapper;
        }

        /// <summary>
        /// Retrives all Patch records
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="500">Something went wrong</response>
        [ProducesResponseType(typeof(PatchesResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        [Route("all")]
        public async Task<IActionResult> GetAllPatches()
        {
            var allPatches = await _getAllPatchesUseCase.Execute().ConfigureAwait(false);

            return Ok(allPatches.ToResponse());
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
        [HttpDelete]
        [Route("{id}/responsibleEntity/{responsibileEntityId}")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> DeleteResponsibilityFromPatch([FromRoute] DeleteResponsibilityFromPatchRequest query)
        {
            try
            {
                await _deleteResponsibilityFromPatchUseCase.Execute(query).ConfigureAwait(false);
                return NoContent();
            }
            catch (PatchNotFoundException)
            {
                return NotFound(query.Id);
            }
            catch (ResponsibileIdNotFoundInPatchException)
            {
                return NotFound(query.ResponsibileEntityId);
            }
        }
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

        }

        [HttpPatch]
        [Route("{id}/responsibleEntities")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> ReplacePatchResponsibleEntities([FromRoute] PatchesQueryObject query,
                                                                          [FromBody] List<ResponsibleEntities> responsibleEntitiesRequestObject)
        {
            var contextHeaders = _contextWrapper.GetContextRequestHeaders(HttpContext);
            var ifMatch = GetIfMatchFromHeader();

            try
            {
                // We use a request object AND the raw request body text because the incoming request will only contain the fields that changed
                // whereas the request object has all possible updateable fields defined.
                // The implementation will use the raw body text to identify which fields to update and the request object is specified here so that its
                // associated validation will be executed by the MVC pipeline before we even get to this point.
                var patch = await _replacePatchResponsibleEntities.ExecuteAsync(query, responsibleEntitiesRequestObject, ifMatch)
                                                                .ConfigureAwait(false);
                if (patch == null) return NotFound(query.Id);
                return NoContent();
            }
            catch (VersionNumberConflictException vncErr)
            {
                return Conflict(vncErr.Message);
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
        /// <summary>
        /// Retrieves all patch for the supplied parentId.
        /// </summary>
        /// <response code="200">Returns the list of patches for the supplied parentId.</response>
        /// <response code="400">Invalid Query Parameter.</response>
        /// <response code="404">No notes found for the supplied targetId</response>
        [ProducesResponseType(typeof(List<PatchesResponseObject>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> GetByParentIdAsync([FromQuery] GetPatchByParentIdQuery query)
        {
            var patch = await _getPatchByParentIdUseCase.ExecuteAsync(query).ConfigureAwait(false);
            if (patch == null || patch.Count == 0) return NotFound(query.ParentId);


            return Ok(patch);
        }
    }
}
