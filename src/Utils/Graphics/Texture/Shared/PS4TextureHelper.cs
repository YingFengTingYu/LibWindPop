using System;

namespace LibWindPop.Utils.Graphics.Texture.Shared
{
    internal static class PS4TextureHelper
    {
        public static int GetMicroTexelIndexFromPosition(int xPos, int yPos)
        {
            int Twiddled = 0;
            int SrcBitPos = 1;
            int DstBitPos = 1;
            while (SrcBitPos < 8)
            {
                if ((xPos & SrcBitPos) != 0)
                {
                    Twiddled |= DstBitPos;
                }
                if ((yPos & SrcBitPos) != 0)
                {
                    Twiddled |= DstBitPos << 1;
                }
                SrcBitPos <<= 1;
                DstBitPos <<= 2;
            }
            return Twiddled;
        }

        public static (int alignWidth, int alignHeight) GetAlignSizeFromTexelSize(int width, int height)
        {
            if (width < 8 && height < 8)
            {
                if (!BitHelper.IsPowerOfTwo(width))
                {
                    width = BitHelper.GetClosestPowerOfTwoAbove(width);
                }
                if (!BitHelper.IsPowerOfTwo(height))
                {
                    height = BitHelper.GetClosestPowerOfTwoAbove(height);
                }
                width = height = Math.Max(width, height);
            }
            else
            {
                if ((width & 0x7) != 0)
                {
                    width |= 0x7;
                    width++;
                }
                if ((height & 0x7) != 0)
                {
                    height |= 0x7;
                    height++;
                }
            }
            return (width, height);
        }
    }
}
