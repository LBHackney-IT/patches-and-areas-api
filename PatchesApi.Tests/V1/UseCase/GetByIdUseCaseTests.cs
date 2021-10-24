using PatchesApi.V1.Gateways;
using PatchesApi.V1.UseCase;
using Moq;
using Xunit;

namespace PatchesApi.Tests.V1.UseCase
{
    [Collection("LogCall Collection")]
    public class GetByIdUseCaseTests
    {
        private Mock<IPatchesGateway> _mockGateway;
        private GetByIdUseCase _classUnderTest;

        public GetByIdUseCaseTests()
        {
            _mockGateway = new Mock<IPatchesGateway>();
            _classUnderTest = new GetByIdUseCase(_mockGateway.Object);
        }

        //TODO: test to check that the use case retrieves the correct record from the database.
        //Guidance on unit testing and example of mocking can be found here https://github.com/LBHackney-IT/lbh-patches-api/wiki/Writing-Unit-Tests
    }
}
