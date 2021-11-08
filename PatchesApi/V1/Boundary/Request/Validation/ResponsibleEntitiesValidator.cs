using FluentValidation;
using Hackney.Core.Validation;
using PatchesApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.Boundary.Request.Validation
{
    public class ResponsibleEntitiesValidator : AbstractValidator<ResponsibleEntities>
    {
        public ResponsibleEntitiesValidator()
        {
            RuleFor(x => x.Name).NotXssString()
                         .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
