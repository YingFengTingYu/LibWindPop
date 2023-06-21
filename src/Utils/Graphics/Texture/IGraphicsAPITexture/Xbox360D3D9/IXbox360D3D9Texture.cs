namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9
{
    /// <summary>
    /// HRESULT CreateTexture(UINT Width, UINT Height, UINT Levels, DWORD Usage, D3DFORMAT Format, D3DPOOL UnusedPool, IDirect3DTexture9 **ppTexture, HANDLE *pUnusedSharedHandle)
    /// </summary>
    public interface IXbox360D3D9Texture
    {
        static abstract D3DFORMAT Xbox360D3D9Format { get; }
    }
}
