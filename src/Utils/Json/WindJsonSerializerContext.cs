using LibWindPop.Packs.Common;
using LibWindPop.Packs.Pak.ContentPipeline;
using LibWindPop.Packs.Pak;
using LibWindPop.Packs.Rsb.ContentPipeline;
using LibWindPop.Packs.Rsb;
using LibWindPop.Packs.Xpr;
using System.Text.Json.Serialization;

namespace LibWindPop.Utils.Json
{
    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        GenerationMode = JsonSourceGenerationMode.Metadata,
        IncludeFields = true,
        WriteIndented = true
        )]
    [JsonSerializable(typeof(WindJsonShell<RsbManifestInfo>))]
    [JsonSerializable(typeof(WindJsonShell<RsbPackInfo>))]
    [JsonSerializable(typeof(WindJsonShell<RsgMetadata>))]
    [JsonSerializable(typeof(WindJsonShell<PtxRsbMetadata>))]
    [JsonSerializable(typeof(WindJsonShell<PtxPS3Metadata>))]
    [JsonSerializable(typeof(WindJsonShell<ContentPipelineInfo>))]
    [JsonSerializable(typeof(WindJsonShell<XprPackInfo>))]
    [JsonSerializable(typeof(WindJsonShell<PakPackInfo>))]
    [JsonSerializable(typeof(WindJsonShell<PakRebuildFileSetting>))]
    internal partial class WindJsonSerializerContext : JsonSerializerContext
    {

    }
}
