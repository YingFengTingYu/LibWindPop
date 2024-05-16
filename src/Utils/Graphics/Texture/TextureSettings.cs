namespace LibWindPop.Utils.Graphics.Texture
{
    public static class TextureSettings
    {
        public static bool UsePVRTexLib { get; set; } = true;
        public static bool PVRTexLibDoDither { get; set; } = false;
        public static PVRTexLibTextureQuality PVRTexLibQuality { get; set; } = PVRTexLibTextureQuality.Best;

        public enum PVRTexLibTextureQuality
        {
            Fast = 0,
            Normal = 1,
            High = 2,
            Best = 3,
        }
    }
}
