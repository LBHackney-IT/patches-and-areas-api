using FluentValidation;
using Hackney.Core.Validation;
using PatchesAndAreasApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.Boundary.Request.Validation
{
    public class PatchValidator : AbstractValidator<PatchEntity>
    {
        public PatchValidator()
        {
            RuleFor(x => x.Id).NotNull()
                .NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotXssString()
                         .WithErrorCode(ErrorCodes.XssCheckFailure);
            RuleFor(x => x.Domain).NotXssString()
                         .WithErrorCode(ErrorCodes.XssCheckFailure);
            RuleForEach(x => x.ResponsibleEntities).SetValidator(new ResponsibleEntitiesValidator());
        }
    }
}
