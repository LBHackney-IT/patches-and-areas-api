using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.Boundary.Request.Validation
{
    public class GetByIdRequestValidator : AbstractValidator<PatchesQueryObject>
    {
        public GetByIdRequestValidator()
        {
            RuleFor(x => x.Id).NotNull()
                              .NotEqual(Guid.Empty);
        }
    }
}
