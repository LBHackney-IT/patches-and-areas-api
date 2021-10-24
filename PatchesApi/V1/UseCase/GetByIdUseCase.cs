using Hackney.Core.Logging;
using PatchesApi.V1.Boundary.Request;
using PatchesApi.V1.Domain;
using PatchesApi.V1.Gateways;
using PatchesApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.UseCase
{
    public class GetByIdUseCase : IGetByIdUseCase
    {
        private IPatchesGateway _gateway;
        public GetByIdUseCase(IPatchesGateway gateway)
        {
            _gateway = gateway;
        }
        [LogCall]
        //TODO: rename id to the name of the identifier that will be used for this API, the type may also need to change
        public Task<Entity> Execute(PatchesQueryObject query)
        {
            return _gateway.GetEntityById(query);
        }
    }
}
