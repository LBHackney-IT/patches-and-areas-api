using AutoFixture;
using PatchesApi.V1.Controllers;
using PatchesApi.V1.UseCase.Interfaces;
using Moq;
using Xunit;

namespace PatchesApi.Tests.V1.Controllers
{
    [Collection("LogCall Collection")]
    public class PatchesApiControllerTests
    {
        private PatchesApiController _classUnderTest;
        private Mock<IGetByIdUseCase> _mockGetByIdUseCase;

        public PatchesApiControllerTests()
        {
            _mockGetByIdUseCase = new Mock<IGetByIdUseCase>();
            _classUnderTest = new PatchesApiController(_mockGetByIdUseCase.Object);
        }


        //Add Tests Here
    }
}
