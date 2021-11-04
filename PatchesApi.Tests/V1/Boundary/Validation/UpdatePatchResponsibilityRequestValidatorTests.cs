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
    public class UpdatePatchResponsibilityRequestValidatorTests
    {
        private readonly UpdatePatchResponsibilityRequestValidator _classUnderTest;

        public UpdatePatchResponsibilityRequestValidatorTests()
        {
            _classUnderTest = new UpdatePatchResponsibilityRequestValidator();
        }

        private const string StringWithTags = "Some string with <tag> in it.";

        [Fact]
        public void RequestShouldErrorWithTagsInName()
        {
            var model = new UpdatePatchesResponsibilitiesRequestObject() { Name = StringWithTags };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void RequestShouldNotErrorWithValidName()
        {
            string name = "name12345";
            var model = new UpdatePatchesResponsibilitiesRequestObject() { Name = name };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
    }
}
