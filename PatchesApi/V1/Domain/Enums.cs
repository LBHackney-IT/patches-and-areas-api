using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PatchesApi.V1.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PatchType
    {
        patch,
        area
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResponsibleType
    {
        HousingOfficer,
        HousingAreaManager
    }
}
