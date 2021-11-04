using FluentValidation.TestHelper;
using PatchesApi.V1.Boundary.Request;
using PatchesApi.V1.Boundary.Request.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PatchesApi.Tests.V1.Boundary.Validation
{
    public class GetByIdRequestValidatorTests
    {
        private readonly GetByIdRequestValidator _classUnderTest;

        public GetByIdRequestValidatorTests()
        {
            _classUnderTest = new GetByIdRequestValidator();
        }

        [Fact]
        public void RequestShouldErrorWithNullId()
        {
            var query = new PatchesQueryObject();
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void RequestShouldErrorWithEmptyId()
        {
            var query = new PatchesQueryObject() { Id = Guid.Empty };
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}
