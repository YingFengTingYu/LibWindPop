namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.DirectX
{
    public interface IDirectXTexture
    {
        static virtual DXGI_FORMAT DirectXFormat { get => DXGI_FORMAT.DXGI_FORMAT_UNKNOWN; }
    }
}
