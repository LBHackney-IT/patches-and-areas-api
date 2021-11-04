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
    public class DeleteResponsibilityFromPatchRequestValidatorTests
    {
        private readonly DeleteResponsibilityFromPatchRequestValidator _classUnderTest;

        public DeleteResponsibilityFromPatchRequestValidatorTests()
        {
            _classUnderTest = new DeleteResponsibilityFromPatchRequestValidator();
        }

        [Fact]
        public void RequestShouldErrorWithNullId()
        {
            var query = new DeleteResponsibilityFromPatchRequest();
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void RequestShouldErrorWithEmptyId()
        {
            var query = new DeleteResponsibilityFromPatchRequest() { Id = Guid.Empty };
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void RequestShouldErrorWithNullResponsibilityId()
        {
            var query = new DeleteResponsibilityFromPatchRequest();
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.ResponsibileEntityId);
        }

        [Fact]
        public void RequestShouldErrorWithEmptyResponsbilityId()
        {
            var query = new DeleteResponsibilityFromPatchRequest() { ResponsibileEntityId = Guid.Empty };
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.ResponsibileEntityId);
        }
    }
}
