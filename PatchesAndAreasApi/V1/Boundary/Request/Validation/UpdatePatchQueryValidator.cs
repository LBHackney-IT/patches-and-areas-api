using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.Boundary.Request.Validation
{

    public class UpdatePatchQueryValidator : AbstractValidator<UpdatePatchesResponsibilityRequest>
    {
        public UpdatePatchQueryValidator()
        {
            RuleFor(x => x.Id).NotNull()
                              .NotEqual(Guid.Empty);
            RuleFor(x => x.ResponsibileEntityId)
                              .NotNull()
                              .NotEqual(Guid.Empty);
        }
    }
}
