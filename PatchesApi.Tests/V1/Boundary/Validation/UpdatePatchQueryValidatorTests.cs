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
    public class UpdatePatchQueryValidatorTests
    {
        private readonly UpdatePatchQueryValidator _classUnderTest;

        public UpdatePatchQueryValidatorTests()
        {
            _classUnderTest = new UpdatePatchQueryValidator();
        }

        [Fact]
        public void RequestShouldErrorWithNullId()
        {
            var query = new UpdatePatchesResponsibilityRequest();
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void RequestShouldErrorWithEmptyId()
        {
            var query = new UpdatePatchesResponsibilityRequest() { Id = Guid.Empty };
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void RequestShouldErrorWithNullResponsibilityId()
        {
            var query = new UpdatePatchesResponsibilityRequest();
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.ResponsibileEntityId);
        }

        [Fact]
        public void RequestShouldErrorWithEmptyResponsbilityId()
        {
            var query = new UpdatePatchesResponsibilityRequest() { ResponsibileEntityId = Guid.Empty };
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.ResponsibileEntityId);
        }
    }
}
