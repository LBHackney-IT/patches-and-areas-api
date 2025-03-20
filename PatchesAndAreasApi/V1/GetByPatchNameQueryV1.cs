using Microsoft.AspNetCore.Mvc;

namespace PatchesAndAreasApi.V1
{
    public class GetByPatchNameQueryV1
    {
        [FromRoute(Name = "patchName")]
        public string PatchName { get; set; }
    }
}
