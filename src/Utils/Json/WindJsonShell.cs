namespace LibWindPop.Utils.Json
{
    internal struct WindJsonShell<T>
        where T : class, IJsonVersionCheckable
    {
        public string? Source;

        public string? Author;

        public uint? Version;

        public T? Content;
    }
}
