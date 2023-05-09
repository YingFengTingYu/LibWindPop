using System.Text;

namespace LibWindPop.Utils.Extension
{
    internal static class EncodingExtension
    {
        public static Encoding GetEncoding(this EncodingType encodingType)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                return Encoding.GetEncoding((int)encodingType);
            }
            catch
            {
                return Encoding.ASCII;
            }
        }
    }
}
