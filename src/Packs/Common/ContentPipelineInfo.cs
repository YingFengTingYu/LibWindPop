using LibWindPop.Utils.Json;
using System.Collections.Generic;

namespace LibWindPop.Packs.Common
{
    public class ContentPipelineInfo : IJsonVersionCheckable
    {
        public static uint JsonVersion => 0u;

        public List<string>? Pipelines;
    }
}
