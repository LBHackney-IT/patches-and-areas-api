using PatchesApi.V1.Boundary.Response;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PatchesApi.V1.UseCase.Interfaces;
using PatchesApi.V1.Boundary.Request;

namespace PatchesApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/patches")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class PatchesApiController : BaseController
    {
        private readonly IGetByIdUseCase _getByIdUseCase;
        public PatchesApiController(IGetByIdUseCase getByIdUseCase)
        {
            _getByIdUseCase = getByIdUseCase;
        }


        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="404">No ? found for the specified ID</response>
        [ProducesResponseType(typeof(PatchesResponseObject), StatusCodes.Status200OK)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        //TODO: rename to match the identifier that will be used
        [Route("{yourId}")]
        public IActionResult ViewRecord(PatchesQueryObject query)
        {
            return Ok(_getByIdUseCase.Execute(query));
        }
    }
}
