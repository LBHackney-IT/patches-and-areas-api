using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.Boundary.Request.Validation
{
    public class UpdatePatchResponsibilityRequestValidator : AbstractValidator<UpdatePatchesResponsibilitiesRequestObject>
    {
        public UpdatePatchResponsibilityRequestValidator()
        {
            RuleFor(x => x.Name).NotXssString()
                         .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
