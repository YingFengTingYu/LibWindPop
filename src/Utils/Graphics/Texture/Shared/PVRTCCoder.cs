using System;
using System.Buffers;

namespace LibWindPop.Utils.Graphics.Texture.Shared
{
    internal static unsafe class PVRTCCoder
    {
        public static void EncodeTexture_RGBA_PVRTCII_4BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCEncoder.CompressPVRTC(texPtr, color, width, height, 4, true, true);
        }

        public static void EncodeTexture_RGBA_PVRTCII_2BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCEncoder.CompressPVRTC(texPtr, color, width, height, 2, true, true);
        }

        public static void EncodeTexture_RGBA_PVRTCI_4BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCEncoder.CompressPVRTC(texPtr, color, width, height, 4, false, true);
        }

        public static void EncodeTexture_RGB_PVRTCI_4BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCEncoder.CompressPVRTC(texPtr, color, width, height, 4, false, false);
        }

        public static void EncodeTexture_RGBA_PVRTCI_2BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCEncoder.CompressPVRTC(texPtr, color, width, height, 2, false, true);
        }

        public static void EncodeTexture_RGB_PVRTCI_2BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCEncoder.CompressPVRTC(texPtr, color, width, height, 2, false, false);
        }

        public static void DecodeTexture_RGBA_PVRTCII_4BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCDecoder.DecompressPVRTC(texPtr, color, width, height, 4, true, true);
        }

        public static void DecodeTexture_RGBA_PVRTCII_2BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCDecoder.DecompressPVRTC(texPtr, color, width, height, 2, true, true);
        }

        public static void DecodeTexture_RGBA_PVRTCI_4BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCDecoder.DecompressPVRTC(texPtr, color, width, height, 4, false, true);
        }

        public static void DecodeTexture_RGB_PVRTCI_4BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCDecoder.DecompressPVRTC(texPtr, color, width, height, 4, false, false);
        }

        public static void DecodeTexture_RGBA_PVRTCI_2BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCDecoder.DecompressPVRTC(texPtr, color, width, height, 2, false, true);
        }

        public static void DecodeTexture_RGB_PVRTCI_2BPP(byte* texPtr, YFColor* color, uint width, uint height)
        {
            PVRTCDecoder.DecompressPVRTC(texPtr, color, width, height, 2, false, false);
        }

        /// <summary>
        /// PowerVR SDK provide PVRTCI decoding function
        /// I modify it to support PVRTCII decoding
        /// Source file:
        /// https://github.com/powervr-graphics/Native_SDK/blob/master/framework/PVRCore/texture/PVRTDecompress.cpp
        /// Source project:
        /// https://github.com/powervr-graphics/Native_SDK
        /// </summary>
        private static class PVRTCDecoder
        {
            /// <summary>
            /// a 128-bit color struct
            /// </summary>
            private struct Pixel128S
            {
                public int red, green, blue, alpha;
            };

            /// <summary>
            /// a pvrtc word
            /// </summary>
            private struct PVRTCWord
            {
                /// <summary>
                /// index data
                /// </summary>
                public uint ModulationData;
                /// <summary>
                /// color data
                /// </summary>
                public uint ColorData;
            };

            /// <summary>
            /// a struct to save four pvrtc block position
            /// </summary>
            private struct PVRTCWordIndices
            {
                public fixed int P[2], Q[2], R[2], S[2];
            };

            /// <summary>
            /// decode colorA as RGBA5554 from ColorData
            /// </summary>
            /// <param name="ColorData">PVRTCWord ColorData</param>
            /// <param name="versionII">if the PVRTCWord is PVRTCII</param>
            /// <returns>colorA as RGBA5554</returns>
            private static YFColor GetColorA(uint ColorData, bool versionII)
            {
                YFColor color;
                uint mask = versionII ? 0x80000000 : 0x8000;
                if ((ColorData & mask) != 0)
                {
                    color.Red = (byte)((ColorData & 0x7c00) >> 10);
                    color.Green = (byte)((ColorData & 0x3e0) >> 5);
                    color.Blue = (byte)((ColorData & 0x1e) | ((ColorData & 0x1e) >> 4));
                    color.Alpha = (byte)0xf;
                }
                else
                {
                    color.Red = (byte)(((ColorData & 0xf00) >> 7) | ((ColorData & 0xf00) >> 11));
                    color.Green = (byte)(((ColorData & 0xf0) >> 3) | ((ColorData & 0xf0) >> 7));
                    color.Blue = (byte)(((ColorData & 0xe) << 1) | ((ColorData & 0xe) >> 2));
                    color.Alpha = (byte)((ColorData & 0x7000) >> 11);
                }
                return color;
            }

            /// <summary>
            /// decode colorB as RGBA5554 from ColorData
            /// </summary>
            /// <param name="ColorData">PVRTCWord ColorData</param>
            /// <param name="versionII">if the PVRTCWord is PVRTCII</param>
            /// <returns>colorB as RGBA5554</returns>
            private static YFColor GetColorB(uint ColorData, bool versionII)
            {
                YFColor color;
                if ((ColorData & 0x80000000) != 0)
                {
                    color.Red = (byte)((ColorData & 0x7c000000) >> 26);
                    color.Green = (byte)((ColorData & 0x3e00000) >> 21);
                    color.Blue = (byte)((ColorData & 0x1f0000) >> 16);
                    color.Alpha = (byte)0xf;
                }
                else
                {
                    uint adds = versionII ? 1U : 0U;
                    color.Red = (byte)(((ColorData & 0xf000000) >> 23) | ((ColorData & 0xf000000) >> 27));
                    color.Green = (byte)(((ColorData & 0xf00000) >> 19) | ((ColorData & 0xf00000) >> 23));
                    color.Blue = (byte)(((ColorData & 0xf0000) >> 15) | ((ColorData & 0xf0000) >> 19));
                    color.Alpha = (byte)(((ColorData & 0x70000000) >> 27) | adds);
                }
                return color;
            }

            /// <summary>
            /// Expand RGBA5554 as RGBA8888
            /// </summary>
            /// <param name="color5554">RGBA5554 color</param>
            /// <returns>RGBA8888 color</returns>
            private static Pixel128S Color5554To8888(YFColor color5554)
            {
                return new Pixel128S
                {
                    red = (color5554.Red << 3) | (color5554.Red >> 2),
                    green = (color5554.Green << 3) | (color5554.Green >> 2),
                    blue = (color5554.Blue << 3) | (color5554.Blue >> 2),
                    alpha = (color5554.Alpha << 4) | color5554.Alpha,
                };
            }

            /// <summary>
            /// interpolate color as 4x4/8x4 block from 4 color
            /// </summary>
            /// <param name="P">P(Left Top Point) Color</param>
            /// <param name="Q">Q(Right Top Point) Color</param>
            /// <param name="R">R(Left Bottom Point) Color</param>
            /// <param name="S">S(Right Bottom Point) Color</param>
            /// <param name="pPixel">buffer to save interpolated color</param>
            /// <param name="bpp">color bits per pixel</param>
            private static void InterpolateColors(YFColor P, YFColor Q, YFColor R, YFColor S, Pixel128S* pPixel, byte bpp)
            {
                int WordWidth = bpp == 2 ? 8 : 4;
                int WordHeight = 4;
                Pixel128S hP = new Pixel128S
                {
                    red = P.Red,
                    green = P.Green,
                    blue = P.Blue,
                    alpha = P.Alpha
                };
                Pixel128S hQ = new Pixel128S
                {
                    red = Q.Red,
                    green = Q.Green,
                    blue = Q.Blue,
                    alpha = Q.Alpha
                };
                Pixel128S hR = new Pixel128S
                {
                    red = R.Red,
                    green = R.Green,
                    blue = R.Blue,
                    alpha = R.Alpha
                };
                Pixel128S hS = new Pixel128S
                {
                    red = S.Red,
                    green = S.Green,
                    blue = S.Blue,
                    alpha = S.Alpha
                };
                Pixel128S QminusP = new Pixel128S
                {
                    red = hQ.red - hP.red,
                    green = hQ.green - hP.green,
                    blue = hQ.blue - hP.blue,
                    alpha = hQ.alpha - hP.alpha
                };
                Pixel128S SminusR = new Pixel128S
                {
                    red = hS.red - hR.red,
                    green = hS.green - hR.green,
                    blue = hS.blue - hR.blue,
                    alpha = hS.alpha - hR.alpha
                };
                hP.red *= WordWidth;
                hP.green *= WordWidth;
                hP.blue *= WordWidth;
                hP.alpha *= WordWidth;
                hR.red *= WordWidth;
                hR.green *= WordWidth;
                hR.blue *= WordWidth;
                hR.alpha *= WordWidth;
                if (bpp == 2)
                {
                    for (int x = 0; x < WordWidth; x++)
                    {
                        Pixel128S result = new Pixel128S
                        {
                            red = 4 * hP.red,
                            green = 4 * hP.green,
                            blue = 4 * hP.blue,
                            alpha = 4 * hP.alpha
                        };
                        Pixel128S dY = new Pixel128S
                        {
                            red = hR.red - hP.red,
                            green = hR.green - hP.green,
                            blue = hR.blue - hP.blue,
                            alpha = hR.alpha - hP.alpha
                        };
                        for (int y = 0; y < WordHeight; y++)
                        {
                            pPixel[y * WordWidth + x].red = (result.red >> 7) + (result.red >> 2);
                            pPixel[y * WordWidth + x].green = (result.green >> 7) + (result.green >> 2);
                            pPixel[y * WordWidth + x].blue = (result.blue >> 7) + (result.blue >> 2);
                            pPixel[y * WordWidth + x].alpha = (result.alpha >> 5) + (result.alpha >> 1);
                            result.red += dY.red;
                            result.green += dY.green;
                            result.blue += dY.blue;
                            result.alpha += dY.alpha;
                        }
                        hP.red += QminusP.red;
                        hP.green += QminusP.green;
                        hP.blue += QminusP.blue;
                        hP.alpha += QminusP.alpha;
                        hR.red += SminusR.red;
                        hR.green += SminusR.green;
                        hR.blue += SminusR.blue;
                        hR.alpha += SminusR.alpha;
                    }
                }
                else
                {
                    for (int y = 0; y < WordHeight; y++)
                    {
                        Pixel128S result = new Pixel128S
                        {
                            red = 4 * hP.red,
                            green = 4 * hP.green,
                            blue = 4 * hP.blue,
                            alpha = 4 * hP.alpha
                        };
                        Pixel128S dY = new Pixel128S
                        {
                            red = hR.red - hP.red,
                            green = hR.green - hP.green,
                            blue = hR.blue - hP.blue,
                            alpha = hR.alpha - hP.alpha
                        };
                        for (int x = 0; x < WordWidth; x++)
                        {
                            pPixel[y * WordWidth + x].red = (result.red >> 6) + (result.red >> 1);
                            pPixel[y * WordWidth + x].green = (result.green >> 6) + (result.green >> 1);
                            pPixel[y * WordWidth + x].blue = (result.blue >> 6) + (result.blue >> 1);
                            pPixel[y * WordWidth + x].alpha = (result.alpha >> 4) + result.alpha;
                            result.red += dY.red;
                            result.green += dY.green;
                            result.blue += dY.blue;
                            result.alpha += dY.alpha;
                        }
                        hP.red += QminusP.red;
                        hP.green += QminusP.green;
                        hP.blue += QminusP.blue;
                        hP.alpha += QminusP.alpha;
                        hR.red += SminusR.red;
                        hR.green += SminusR.green;
                        hR.blue += SminusR.blue;
                        hR.alpha += SminusR.alpha;
                    }
                }
            }

            /// <summary>
            /// 从PVRTCWord中获取一个四分之一PVRTC块的索引调制值和索引值
            /// </summary>
            /// <param name="word">PVRTCWord</param>
            /// <param name="offsetX">offset X</param>
            /// <param name="offsetY">offset Y</param>
            /// <param name="ModulationValues">index pointer</param>
            /// <param name="ModulationModes">modes pointer</param>
            /// <param name="bpp">color bits per pixel</param>
            private static void UnpackModulations(PVRTCWord word, int offsetX, int offsetY, int** ModulationValues, int** ModulationModes, byte bpp)
            {
                uint WordModMode = word.ColorData & 0x1;
                uint ModulationBits = word.ModulationData;
                if (bpp == 2)
                {
                    if (WordModMode != 0)
                    {
                        if ((ModulationBits & 0x1) != 0)
                        {
                            if ((ModulationBits & (0x1 << 20)) != 0)
                            {
                                WordModMode = 3;
                            }
                            else
                            {
                                WordModMode = 2;
                            }

                            if ((ModulationBits & (0x1 << 21)) != 0)
                            {
                                ModulationBits |= (0x1 << 20);
                            }
                            else
                            {
                                ModulationBits &= ~((uint)0x1 << 20);
                            }
                        }

                        if ((ModulationBits & 0x2) != 0)
                        {
                            ModulationBits |= 0x1;
                        }
                        else
                        {
                            ModulationBits &= ~(uint)0x1;
                        }

                        for (int y = 0; y < 4; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                ModulationModes[x + offsetX][y + offsetY] = (int)WordModMode;

                                if (((x ^ y) & 1) == 0)
                                {
                                    ModulationValues[x + offsetX][y + offsetY] = (int)(ModulationBits & 3);
                                    ModulationBits >>= 2;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                ModulationModes[x + offsetX][y + offsetY] = (int)WordModMode;

                                if ((ModulationBits & 1) != 0)
                                {
                                    ModulationValues[x + offsetX][y + offsetY] = 0x3;
                                }
                                else
                                {
                                    ModulationValues[x + offsetX][y + offsetY] = 0x0;
                                }
                                ModulationBits >>= 1;
                            }
                        }
                    }
                }
                else
                {
                    if (WordModMode != 0)
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                ModulationModes[y + offsetY][x + offsetX] = (int)WordModMode;
                                ModulationValues[y + offsetY][x + offsetX] = (int)(ModulationBits & 3);
                                if (ModulationValues[y + offsetY][x + offsetX] == 1)
                                {
                                    ModulationValues[y + offsetY][x + offsetX] = 4;
                                }
                                else if (ModulationValues[y + offsetY][x + offsetX] == 2)
                                {
                                    ModulationValues[y + offsetY][x + offsetX] = 14;
                                }
                                else if (ModulationValues[y + offsetY][x + offsetX] == 3)
                                {
                                    ModulationValues[y + offsetY][x + offsetX] = 8;
                                }
                                ModulationBits >>= 2;
                            }
                        }
                    }
                    else
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                ModulationModes[y + offsetY][x + offsetX] = (int)WordModMode;
                                ModulationValues[y + offsetY][x + offsetX] = (int)(ModulationBits & 3);
                                ModulationValues[y + offsetY][x + offsetX] *= 3;
                                if (ModulationValues[y + offsetY][x + offsetX] > 3) { ModulationValues[y + offsetY][x + offsetX] -= 1; }
                                ModulationBits >>= 2;
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// array for interpolating colorA/B
            /// </summary>
            private static readonly int[] RepVals0 = { 0, 3, 5, 8 };

            /// <summary>
            /// 获取用于线性插值的颜色的插值比例（主要给2BPP用的，4BPP直接返回对应位置插值比例）
            /// </summary>
            /// <param name="ModulationValues">2BPP索引/4BPP插值比例指针</param>
            /// <param name="ModulationModes">2BPP调制模式指针</param>
            /// <param name="xPos">x坐标</param>
            /// <param name="yPos">y坐标</param>
            /// <param name="bpp">color bits per pixel</param>
            /// <returns>插值比例</returns>
            private static int GetModulationValues(int** ModulationValues, int** ModulationModes, uint xPos, uint yPos, byte bpp)
            {
                if (bpp == 2)
                {
                    if (ModulationModes[xPos][yPos] == 0)
                    {
                        return RepVals0[ModulationValues[xPos][yPos]];
                    }
                    else
                    {
                        if (((xPos ^ yPos) & 1) == 0)
                        {
                            return RepVals0[ModulationValues[xPos][yPos]];
                        }
                        else if (ModulationModes[xPos][yPos] == 1)
                        {
                            return (RepVals0[ModulationValues[xPos][yPos - 1]] + RepVals0[ModulationValues[xPos][yPos + 1]] + RepVals0[ModulationValues[xPos - 1][yPos]] + RepVals0[ModulationValues[xPos + 1][yPos]] + 2) >> 2;
                        }
                        else if (ModulationModes[xPos][yPos] == 2)
                        {
                            return (RepVals0[ModulationValues[xPos - 1][yPos]] + RepVals0[ModulationValues[xPos + 1][yPos]] + 1) >> 1;
                        }
                        else
                        {
                            return (RepVals0[ModulationValues[xPos][yPos - 1]] + RepVals0[ModulationValues[xPos][yPos + 1]] + 1) >> 1;
                        }
                    }
                }
                else
                {
                    return ModulationValues[xPos][yPos];
                }
            }

            /// <summary>
            /// 获取本地调色板中指定坐标和索引的颜色
            /// 这是利用下面链接中的本地调色板表格写的函数：
            /// https://www.khronos.org/registry/DataFormat/specs/1.3/dataformat.1.3.html
            /// 情况比较复杂，不在这里介绍了
            /// </summary>
            /// <param name="Pa">左上角P点的颜色A（RGBA8888）</param>
            /// <param name="Pb">左上角P点的颜色B（RGBA8888）</param>
            /// <param name="Qa">右上角Q点的颜色A（RGBA8888）</param>
            /// <param name="Qb">右上角Q点的颜色B（RGBA8888）</param>
            /// <param name="Ra">左下角R点的颜色A（RGBA8888）</param>
            /// <param name="Rb">左下角R点的颜色B（RGBA8888）</param>
            /// <param name="Sa">右下角S点的颜色A（RGBA8888）</param>
            /// <param name="Sb">右下角S点的颜色B（RGBA8888）</param>
            /// <param name="x">在中间子块中的x坐标</param>
            /// <param name="y">在中间子块中的y坐标</param>
            /// <param name="modvalue">索引但不完全是索引，而是调制比</param>
            /// <returns>对应位置的颜色</returns>
            private static Pixel128S GetPaletteColor(Pixel128S Pa, Pixel128S Pb, Pixel128S Qa, Pixel128S Qb, Pixel128S Ra, Pixel128S Rb, Pixel128S Sa, Pixel128S Sb, uint x, uint y, int modvalue)
            {
                if (y == 0)
                {
                    if (x == 0)
                    {
                        if (modvalue == 0)
                        {
                            return Pa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return new Pixel128S
                            {
                                red = (Pa.red * 5 + Pb.red * 3) >> 3,
                                green = (Pa.green * 5 + Pb.green * 3) >> 3,
                                blue = (Pa.blue * 5 + Pb.blue * 3) >> 3,
                                alpha = (Pa.alpha * 5 + Pb.alpha * 3) >> 3,
                            };
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return new Pixel128S
                            {
                                red = (Pa.red * 3 + Pb.red * 5) >> 3,
                                green = (Pa.green * 3 + Pb.green * 5) >> 3,
                                blue = (Pa.blue * 3 + Pb.blue * 5) >> 3,
                                alpha = (Pa.alpha * 3 + Pb.alpha * 5) >> 3,
                            };
                        }
                        else if (modvalue == 8)
                        {
                            return Pb;
                        }
                    }
                    else if (x == 1 || x == 2 || x == 3)
                    {
                        if (modvalue == 0)
                        {
                            return Pa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Pb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Qa;
                        }
                        else if (modvalue == 8)
                        {
                            return Qb;
                        }
                    }
                }
                else if (y == 1)
                {
                    if (x == 0)
                    {
                        if (modvalue == 0)
                        {
                            return Pa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Pb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Ra;
                        }
                        else if (modvalue == 8)
                        {
                            return Rb;
                        }
                    }
                    else if (x == 1)
                    {
                        if (modvalue == 0)
                        {
                            return Pa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Pb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Qa;
                        }
                        else if (modvalue == 8)
                        {
                            return Rb;
                        }
                    }
                    else if (x == 2)
                    {
                        if (modvalue == 0)
                        {
                            return Pa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Pb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Qa;
                        }
                        else if (modvalue == 8)
                        {
                            return Qb;
                        }
                    }
                    else if (x == 3)
                    {
                        if (modvalue == 0)
                        {
                            return Sa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Pb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Qa;
                        }
                        else if (modvalue == 8)
                        {
                            return Qb;
                        }
                    }
                }
                else if (y == 2)
                {
                    if (x == 0 || x == 1)
                    {
                        if (modvalue == 0)
                        {
                            return Pa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Pb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Ra;
                        }
                        else if (modvalue == 8)
                        {
                            return Rb;
                        }
                    }
                    else if (x == 2)
                    {
                        if (modvalue == 0)
                        {
                            return Pa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Sb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Ra;
                        }
                        else if (modvalue == 8)
                        {
                            return Qb;
                        }
                    }
                    else if (x == 3)
                    {
                        if (modvalue == 0)
                        {
                            return Sa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Sb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Qa;
                        }
                        else if (modvalue == 8)
                        {
                            return Qb;
                        }
                    }
                }
                else if (y == 3)
                {
                    if (x == 0)
                    {
                        if (modvalue == 0)
                        {
                            return Pa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Pb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Ra;
                        }
                        else if (modvalue == 8)
                        {
                            return Rb;
                        }
                    }
                    else if (x == 1)
                    {
                        if (modvalue == 0)
                        {
                            return Pa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Sb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Ra;
                        }
                        else if (modvalue == 8)
                        {
                            return Rb;
                        }
                    }
                    else if (x == 2)
                    {
                        if (modvalue == 0)
                        {
                            return Sa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Sb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Ra;
                        }
                        else if (modvalue == 8)
                        {
                            return Rb;
                        }
                    }
                    else if (x == 3)
                    {
                        if (modvalue == 0)
                        {
                            return Sa;
                        }
                        else if (modvalue == 4 || modvalue == 3)
                        {
                            return Sb;
                        }
                        else if (modvalue > 10 || modvalue == 5)
                        {
                            return Ra;
                        }
                        else if (modvalue == 8)
                        {
                            return Qb;
                        }
                    }
                }
                return default(Pixel128S);
            }

            /// <summary>
            /// 从4个PVRTCWord中解码出4*4/8*4颜色块
            /// </summary>
            /// <param name="P">左上角P点PVRTCWord</param>
            /// <param name="Q">右上角Q点PVRTCWord</param>
            /// <param name="R">左下角R点PVRTCWord</param>
            /// <param name="S">右下角S点PVRTCWord</param>
            /// <param name="pColorData">颜色块指针</param>
            /// <param name="bpp">color bits per pixel</param>
            /// <param name="ModulationValues">索引</param>
            /// <param name="ModulationModes">2BPP调制模式</param>
            /// <param name="upscaledColorA">解码的颜色A指针</param>
            /// <param name="upscaledColorB">解码的颜色B指针</param>
            /// <param name="versionII">是否为PVRTCII纹理</param>
            static void PvrtcGetDecompressedPixels(PVRTCWord P, PVRTCWord Q, PVRTCWord R, PVRTCWord S, YFColor* pColorData, byte bpp, int** ModulationValues, int** ModulationModes, Pixel128S* upscaledColorA, Pixel128S* upscaledColorB, bool versionII)
            {
                uint WordWidth = bpp == 2 ? 8U : 4U;
                uint WordHeight = 4;
                uint HalfWordWidth = WordWidth >> 1;
                uint HalfWordHeight = WordHeight >> 1;
                UnpackModulations(P, 0, 0, ModulationValues, ModulationModes, bpp);
                UnpackModulations(Q, (int)WordWidth, 0, ModulationValues, ModulationModes, bpp);
                UnpackModulations(R, 0, (int)WordHeight, ModulationValues, ModulationModes, bpp);
                UnpackModulations(S, (int)WordWidth, (int)WordHeight, ModulationValues, ModulationModes, bpp);
                YFColor Pa = GetColorA(P.ColorData, versionII);
                YFColor Pb = GetColorB(P.ColorData, versionII);
                YFColor Qa = GetColorA(Q.ColorData, versionII);
                YFColor Qb = GetColorB(Q.ColorData, versionII);
                YFColor Ra = GetColorA(R.ColorData, versionII);
                YFColor Rb = GetColorB(R.ColorData, versionII);
                YFColor Sa = GetColorA(S.ColorData, versionII);
                YFColor Sb = GetColorB(S.ColorData, versionII);
                if (versionII && ((P.ColorData & 0x8000) != 0)) // HardTransition，只受P影响
                {
                    Pixel128S PaD = Color5554To8888(Pa);
                    Pixel128S PbD = Color5554To8888(Pb);
                    Pixel128S QaD = Color5554To8888(Qa);
                    Pixel128S QbD = Color5554To8888(Qb);
                    Pixel128S RaD = Color5554To8888(Ra);
                    Pixel128S RbD = Color5554To8888(Rb);
                    Pixel128S SaD = Color5554To8888(Sa);
                    Pixel128S SbD = Color5554To8888(Sb);
                    for (uint y = 0; y < WordHeight; y++)
                    {
                        for (uint x = 0; x < WordWidth; x++)
                        {
                            int mod = GetModulationValues(ModulationValues, ModulationModes, x + HalfWordWidth, y + HalfWordHeight, bpp);
                            Pixel128S result;
                            if ((bpp != 2) && (ModulationModes[x + HalfWordWidth][y + HalfWordHeight] == 1))
                            {
                                result = GetPaletteColor(PaD, PbD, QaD, QbD, RaD, RbD, SaD, SbD, y, x, mod);
                            }
                            else
                            {
                                uint rX, rY;
                                if (bpp == 2)
                                {
                                    rX = x;
                                    rY = y;
                                }
                                else // 4BPP一切都是反着的
                                {
                                    rX = y;
                                    rY = x;
                                }
                                Pixel128S BaseColorA, BaseColorB;
                                if (rX < HalfWordWidth)
                                {
                                    // P or R
                                    if (rY < HalfWordHeight)
                                    {
                                        BaseColorA = PaD;
                                        BaseColorB = PbD;
                                    }
                                    else
                                    {
                                        BaseColorA = RaD;
                                        BaseColorB = RbD;
                                    }
                                }
                                else
                                {
                                    // Q or S
                                    if (rY < HalfWordHeight)
                                    {
                                        BaseColorA = QaD;
                                        BaseColorB = QbD;
                                    }
                                    else
                                    {
                                        BaseColorA = SaD;
                                        BaseColorB = SbD;
                                    }
                                }
                                result.red = (BaseColorA.red * (8 - mod) + BaseColorB.red * mod) / 8;
                                result.green = (BaseColorA.green * (8 - mod) + BaseColorB.green * mod) / 8;
                                result.blue = (BaseColorA.blue * (8 - mod) + BaseColorB.blue * mod) / 8;
                                result.alpha = (BaseColorA.alpha * (8 - mod) + BaseColorB.alpha * mod) / 8;
                            }
                            if (bpp == 2)
                            {
                                uint index = y * WordWidth + x;
                                pColorData[index].Red = (byte)result.red;
                                pColorData[index].Green = (byte)result.green;
                                pColorData[index].Blue = (byte)result.blue;
                                pColorData[index].Alpha = (byte)result.alpha;
                            }
                            else
                            {
                                uint index = y + x * WordHeight;
                                pColorData[index].Red = (byte)result.red;
                                pColorData[index].Green = (byte)result.green;
                                pColorData[index].Blue = (byte)result.blue;
                                pColorData[index].Alpha = (byte)result.alpha;
                            }
                        }
                    }
                }
                else
                {
                    InterpolateColors(Pa, Qa, Ra, Sa, upscaledColorA, bpp);
                    InterpolateColors(Pb, Qb, Rb, Sb, upscaledColorB, bpp);
                    for (uint y = 0; y < WordHeight; y++)
                    {
                        for (uint x = 0; x < WordWidth; x++)
                        {
                            int mod = GetModulationValues(ModulationValues, ModulationModes, x + HalfWordWidth, y + HalfWordHeight, bpp);
                            Pixel128S result;
                            uint yWordWidthx = y * WordWidth + x;
                            if (mod > 10)
                            {
                                result.red = 0;
                                result.green = 0;
                                result.blue = 0;
                                result.alpha = 0;
                            }
                            else
                            {
                                result.red = (upscaledColorA[yWordWidthx].red * (8 - mod) + upscaledColorB[yWordWidthx].red * mod) / 8;
                                result.green = (upscaledColorA[yWordWidthx].green * (8 - mod) + upscaledColorB[yWordWidthx].green * mod) / 8;
                                result.blue = (upscaledColorA[yWordWidthx].blue * (8 - mod) + upscaledColorB[yWordWidthx].blue * mod) / 8;
                                result.alpha = (upscaledColorA[yWordWidthx].alpha * (8 - mod) + upscaledColorB[yWordWidthx].alpha * mod) / 8;
                            }
                            if (bpp == 2)
                            {
                                pColorData[yWordWidthx].Red = (byte)result.red;
                                pColorData[yWordWidthx].Green = (byte)result.green;
                                pColorData[yWordWidthx].Blue = (byte)result.blue;
                                pColorData[yWordWidthx].Alpha = (byte)result.alpha;
                            }
                            else
                            {
                                uint index = y + x * WordHeight;
                                pColorData[index].Red = (byte)result.red;
                                pColorData[index].Green = (byte)result.green;
                                pColorData[index].Blue = (byte)result.blue;
                                pColorData[index].Alpha = (byte)result.alpha;
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Get PVRTCWord real index
            /// </summary>
            /// <param name="numWords">word count</param>
            /// <param name="word">word index</param>
            /// <returns>real index</returns>
            private static uint WrapWordIndex(uint numWords, int word)
            {
                return (uint)((word + numWords) % numWords);
            }

            /// <summary>
            /// Get word index from size and position
            /// </summary>
            /// <param name="XSize">imageWidth / blockWidth</param>
            /// <param name="YSize">imageHeight / blockHeight</param>
            /// <param name="XPos">x in word</param>
            /// <param name="YPos">y in word</param>
            /// <param name="versionII">是否为PVRTCII纹理</param>
            /// <returns>index</returns>
            private static uint GetIndex(uint XSize, uint YSize, uint XPos, uint YPos, bool versionII)
            {
                if (versionII)
                {
                    return YPos * XSize + XPos;
                }
                else
                {
                    uint MinDimension = XSize;
                    uint MaxValue = YPos;
                    uint Twiddled = 0;
                    uint SrcBitPos = 1;
                    uint DstBitPos = 1;
                    int ShiftCount = 0;
                    if (YSize < XSize)
                    {
                        MinDimension = YSize;
                        MaxValue = XPos;
                    }
                    while (SrcBitPos < MinDimension)
                    {
                        if ((YPos & SrcBitPos) != 0)
                        {
                            Twiddled |= DstBitPos;
                        }
                        if ((XPos & SrcBitPos) != 0)
                        {
                            Twiddled |= (DstBitPos << 1);
                        }
                        SrcBitPos <<= 1;
                        DstBitPos <<= 2;
                        ShiftCount++;
                    }
                    MaxValue >>= ShiftCount;
                    Twiddled |= MaxValue << (ShiftCount << 1);
                    return Twiddled;
                }
            }

            /// <summary>
            /// Copy decoded data to image
            /// </summary>
            /// <param name="pOutput">image data pointer</param>
            /// <param name="width">image width</param>
            /// <param name="pWord">decoded data pointer</param>
            /// <param name="words">Word position</param>
            /// <param name="bpp">color bits per pixel</param>
            /// <param name="hasA">if needs to decode alpha</param>
            private static void MapDecompressedData(YFColor* pOutput, int width, YFColor* pWord, PVRTCWordIndices words, byte bpp, bool hasA)
            {
                uint WordWidth = bpp == 2 ? 8U : 4U;
                uint WordHeight = 4;
                uint dw = WordWidth >> 1;
                uint dh = WordHeight >> 1;
                for (uint y = 0; y < dh; y++)
                {
                    for (uint x = 0; x < dw; x++)
                    {
                        long leftIndex = ((words.P[1] * WordHeight) + y + dh) * width + words.P[0] * WordWidth + x + dw;
                        long rightIndex = y * WordWidth + x;
                        pOutput[leftIndex].Red = pWord[rightIndex].Red;
                        pOutput[leftIndex].Green = pWord[rightIndex].Green;
                        pOutput[leftIndex].Blue = pWord[rightIndex].Blue;
                        pOutput[leftIndex].Alpha = hasA ? pWord[rightIndex].Alpha : (byte)0xFF;
                        leftIndex = ((words.Q[1] * WordHeight) + y + dh) * width + words.Q[0] * WordWidth + x;
                        rightIndex = y * WordWidth + x + dw;
                        pOutput[leftIndex].Red = pWord[rightIndex].Red;
                        pOutput[leftIndex].Green = pWord[rightIndex].Green;
                        pOutput[leftIndex].Blue = pWord[rightIndex].Blue;
                        pOutput[leftIndex].Alpha = hasA ? pWord[rightIndex].Alpha : (byte)0xFF;
                        leftIndex = ((words.R[1] * WordHeight) + y) * width + words.R[0] * WordWidth + x + dw;
                        rightIndex = (y + dh) * WordWidth + x;
                        pOutput[leftIndex].Red = pWord[rightIndex].Red;
                        pOutput[leftIndex].Green = pWord[rightIndex].Green;
                        pOutput[leftIndex].Blue = pWord[rightIndex].Blue;
                        pOutput[leftIndex].Alpha = hasA ? pWord[rightIndex].Alpha : (byte)0xFF;
                        leftIndex = ((words.S[1] * WordHeight) + y) * width + words.S[0] * WordWidth + x;
                        rightIndex = (y + dh) * WordWidth + x + dw;
                        pOutput[leftIndex].Red = pWord[rightIndex].Red;
                        pOutput[leftIndex].Green = pWord[rightIndex].Green;
                        pOutput[leftIndex].Blue = pWord[rightIndex].Blue;
                        pOutput[leftIndex].Alpha = hasA ? pWord[rightIndex].Alpha : (byte)0xFF;
                    }
                }
            }

            /// <summary>
            /// 解码PVRTC数据
            /// </summary>
            /// <param name="pCompressedData">pvrtc word pointer</param>
            /// <param name="pDecompressedData">image data pointer</param>
            /// <param name="Width">image width</param>
            /// <param name="Height">image height</param>
            /// <param name="bpp">color bits per pixel</param>
            /// <param name="versionII">是否为PVRTCII纹理</param>
            /// <param name="hasA">if needs to decode alpha</param>
            public static void DecompressPVRTC(byte* pCompressedData, YFColor* pDecompressedData, uint Width, uint Height, byte bpp, bool versionII, bool hasA)
            {
                uint WordWidth = bpp == 2 ? 8U : 4U;
                uint WordHeight = 4;
                uint* pWordMembers = (uint*)pCompressedData;
                int NumXWords = (int)(Width / WordWidth);
                int NumYWords = (int)(Height / WordHeight);
                PVRTCWordIndices indices;
                int alloc_mem =
                    (int)(WordWidth * WordHeight) * sizeof(YFColor)
                    + 4 * sizeof(uint)
                    + 32 * sizeof(Pixel128S)
                    + 32 * sizeof(Pixel128S)
                    + 256 * sizeof(int)
                    + 16 * sizeof(int*)
                    + 16 * sizeof(int*);
                using (IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(alloc_mem))
                {
                    Span<byte> owner_span = owner.Memory.Span[..alloc_mem];
                    fixed (byte* owner_ptr = owner_span)
                    {
                        nuint owner_ptr_num = (nuint)owner_ptr;
                        YFColor* pPixels = (YFColor*)owner_ptr_num; //stackalloc YFColor[(int)(WordWidth * WordHeight)];
                        owner_ptr_num += (uint)((int)(WordWidth * WordHeight) * sizeof(YFColor));
                        uint* WordOffsets = (uint*)owner_ptr_num; // stackalloc uint[4];
                        owner_ptr_num += 4 * sizeof(uint);
                        Pixel128S* upscaledColorA = (Pixel128S*)owner_ptr_num; // stackalloc Pixel128S[32];
                        owner_ptr_num += (uint)(32 * sizeof(Pixel128S));
                        Pixel128S* upscaledColorB = (Pixel128S*)owner_ptr_num; // stackalloc Pixel128S[32];
                        owner_ptr_num += (uint)(32 * sizeof(Pixel128S));
                        int* int_buffer = (int*)owner_ptr_num; // stackalloc int[16 * 8 + 16 * 8];
                        owner_ptr_num += 256 * sizeof(int);
                        int** ModulationValues = (int**)owner_ptr_num; // stackalloc int*[16]; // [16][8]
                        owner_ptr_num += (uint)(16 * sizeof(int*));
                        int** ModulationModes = (int**)owner_ptr_num; // stackalloc int*[16]; // [16][8]
                        owner_ptr_num += (uint)(16 * sizeof(int*));
                        for (int i = 0; i < 16; i++)
                        {
                            ModulationValues[i] = int_buffer;
                            int_buffer += 8;
                        }
                        for (int i = 0; i < 16; i++)
                        {
                            ModulationModes[i] = int_buffer;
                            int_buffer += 8;
                        }
                        //Decompress
                        for (int wordY = -1; wordY < NumYWords - 1; wordY++)
                        {
                            for (int wordX = -1; wordX < NumXWords - 1; wordX++)
                            {
                                indices.P[0] = (int)WrapWordIndex((uint)NumXWords, wordX);
                                indices.P[1] = (int)WrapWordIndex((uint)NumYWords, wordY);
                                indices.Q[0] = (int)WrapWordIndex((uint)NumXWords, wordX + 1);
                                indices.Q[1] = (int)WrapWordIndex((uint)NumYWords, wordY);
                                indices.R[0] = (int)WrapWordIndex((uint)NumXWords, wordX);
                                indices.R[1] = (int)WrapWordIndex((uint)NumYWords, wordY + 1);
                                indices.S[0] = (int)WrapWordIndex((uint)NumXWords, wordX + 1);
                                indices.S[1] = (int)WrapWordIndex((uint)NumYWords, wordY + 1);
                                WordOffsets[0] = GetIndex((uint)NumXWords, (uint)NumYWords, (uint)indices.P[0], (uint)indices.P[1], versionII) << 1;
                                WordOffsets[1] = GetIndex((uint)NumXWords, (uint)NumYWords, (uint)indices.Q[0], (uint)indices.Q[1], versionII) << 1;
                                WordOffsets[2] = GetIndex((uint)NumXWords, (uint)NumYWords, (uint)indices.R[0], (uint)indices.R[1], versionII) << 1;
                                WordOffsets[3] = GetIndex((uint)NumXWords, (uint)NumYWords, (uint)indices.S[0], (uint)indices.S[1], versionII) << 1;
                                PVRTCWord P, Q, R, S;
                                P.ColorData = pWordMembers[WordOffsets[0] + 1];
                                P.ModulationData = pWordMembers[WordOffsets[0]];
                                Q.ColorData = pWordMembers[WordOffsets[1] + 1];
                                Q.ModulationData = pWordMembers[WordOffsets[1]];
                                R.ColorData = pWordMembers[WordOffsets[2] + 1];
                                R.ModulationData = pWordMembers[WordOffsets[2]];
                                S.ColorData = pWordMembers[WordOffsets[3] + 1];
                                S.ModulationData = pWordMembers[WordOffsets[3]];
                                PvrtcGetDecompressedPixels(P, Q, R, S, pPixels, bpp, ModulationValues, ModulationModes, upscaledColorA, upscaledColorB, versionII);
                                MapDecompressedData(pDecompressedData, (int)Width, pPixels, indices, bpp, hasA);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// google image_compression provide RGBA_PVRTCI_2BPP encoding function
        ///  I modify it to support RGB_PVRTCI_4/2BPP, RGBA_PVRTCI_4BPP and RGBA_PVRTCII_4/2BPP encoding
        /// Source file:
        /// https://github.com/google/image-compression/blob/master/image_compression/internal/pvrtc_compressor.cc
        /// Source project:
        /// https://github.com/google/image-compression
        /// </summary>
        private static class PVRTCEncoder
        {
            /// <summary>
            /// 4BPP格式PVRTCWord采用的模式
            /// </summary>
            private enum ModulationMode_4BPP
            {
                /// <summary>
                /// 对高频低频图中的两个颜色进行双线性插值，索引和颜色比例如下：
                /// 0: color0
                /// 1: 5 / 8 * color0 + 3 / 8 * color1
                /// 2: 3 / 8 * color0 + 5 / 8 * color1
                /// 3: color1
                /// </summary>
                StandardBilinear,
                /// <summary>
                /// （本编码器不支持）对高频低频图中的两个颜色进行线性插值，对
                /// 索引为2的图像清空其Alpha通道，索引和颜色比例如下：
                /// 0: color0
                /// 1: 1 / 2 * color0 + 1 / 2 * color1
                /// 2: 1 / 2 * color0 + 1 / 2 * color1, Alpha = 0
                /// 3: color1
                /// </summary>
                PunchThrough
            }

            /// <summary>
            /// 2BPP格式PVRTCWord采用的混合模式
            /// </summary>
            private enum ModulationMode_2BPP
            {
                // 1 bit per pixel for each pixel.
                /// <summary>
                /// 每个点使用1比特直接存储其索引
                /// </summary>
                Direct1BPP,
                // The following three modes have 2 bits per pixel for every other pixel in
                // the block, in a checkerboard pattern. The mode specifies how to infer color
                // for the intervening pixels.
                /// <summary>
                /// 存储时每个点使用2比特的方式存储其在高频低频图的线性插值调色板中的索引，
                /// 然后对未存储索引的点取其水平和垂直方向相邻的四个点的索引值的平均值并四
                /// 舍五入到最近的整数，对于边缘的点，如果当前PVRTCWord中不包含相邻点，则
                /// 获取周围的PVRTCWord中对应点的索引，如果周围连PVRTCWord都没有，就去图像
                /// 的另一侧去找PVRTCWord然后获取对应点的索引
                /// </summary>
                Interpolated2BPP,  // Average the 4 orthoganally connected neighbors.
                /// <summary>
                /// 存储时每个点使用2比特的方式存储其在高频低频图的线性插值调色板中的索引，
                /// 然后对未存储索引的点取其垂直方向相邻的两个点的索引值的平均值并四舍五入
                /// 到最近的整数，对于边缘的点，如果当前PVRTCWord中不包含相邻点，则获取周
                /// 围的PVRTCWord中对应点的索引，如果周围连PVRTCWord都没有，就去图像的另一
                /// 侧去找PVRTCWord然后获取对应点的索引
                /// </summary>
                VerticallyInterpolated2BPP,  // Average the 2 vertical neighbors.
                /// <summary>
                /// 存储时每个点使用2比特的方式存储其在高频低频图的线性插值调色板中的索引，
                /// 然后对未存储索引的点取其水平方向相邻的两个点的索引值的平均值并四舍五入
                /// 到最近的整数，对于边缘的点，如果当前PVRTCWord中不包含相邻点，则获取周
                /// 围的PVRTCWord中对应点的索引，如果周围连PVRTCWord都没有，就去图像的另一
                /// 侧去找PVRTCWord然后获取对应点的索引
                /// </summary>
                HorizontallyInterpolated2BPP  // Average the 2 horizontal neighbors.
            };

            // Block width and height as for 2BPP PVRTC.
            /// <summary>
            /// 2BPP块宽度8是2的3次方，所以这个值是3
            /// </summary>
            const uint kLog2BlockWidth_2BPP = 3;

            /// <summary>
            /// 2BPP块高度4是2的2次方，所以这个值是2
            /// </summary>
            const uint kLog2BlockHeight_2BPP = 2;

            /// <summary>
            /// 2BPP块宽度是8
            /// </summary>
            const uint kBlockWidth_2BPP = 8;

            /// <summary>
            /// 2BPP块高度是4
            /// </summary>
            const uint kBlockHeight_2BPP = 4;

            // 我加个4BPP支持
            /// <summary>
            /// 4BPP块宽度4是2的2次方，所以这个值是2
            /// </summary>
            const uint kLog2BlockWidth_4BPP = 2;

            /// <summary>
            /// 4BPP块高度4是2的2次方，所以这个值是2
            /// </summary>
            const uint kLog2BlockHeight_4BPP = 2;

            /// <summary>
            /// 4BPP块宽度是4
            /// </summary>
            const uint kBlockWidth_4BPP = 4;

            /// <summary>
            /// 4BPP块高度是4
            /// </summary>
            const uint kBlockHeight_4BPP = 4;

            //-----------------------------------------------------------------------------

            //
            // General helper functions.
            //

            // Little-endian write.
            /// <summary>
            /// 将32位无符号整数写入给定指针位置，并返回增加位置后的指针
            /// </summary>
            /// <param name="value">被写入的32位无符号整数值</param>
            /// <param name="output">被写入的位置指针</param>
            /// <returns>增加位置后的指针</returns>
            static byte* Append32(uint value, byte* output)
            {
                *output++ = (byte)(value & 0xFF);
                *output++ = (byte)((value >> 8) & 0xFF);
                *output++ = (byte)((value >> 16) & 0xFF);
                *output++ = (byte)(value >> 24);
                return output;
            }

            /// <summary>
            /// 返回32位有符号整数的绝对值
            /// </summary>
            /// <param name="v">被计算绝对值的32位有符号整数</param>
            /// <returns>计算出的绝对值</returns>
            static uint abs(int v)
            {
                if (v <= 0) return (uint)-v;
                return (uint)v;
            }

            // A quick measure of how different are two colors for the human eye.
            // The bigger the return value, the more different.
            /// <summary>
            /// 快速计算两个颜色的差异值，返回值越大则颜色差异越大
            /// 原理是分别计算R，G，B，A通道的差值的绝对值，并求和
            /// </summary>
            /// <param name="color0">第一个颜色</param>
            /// <param name="color1">第二个颜色</param>
            /// <returns>差异值（R，G，B，A通道的差值的绝对值的和）</returns>
            static uint ColorDiff(YFColor color0, YFColor color1, bool hasA)
            {
                uint rgb_delta = abs(color0.Red - color1.Red) + abs(color0.Green - color1.Green) + abs(color0.Blue - color1.Blue);
                if (hasA)
                {
                    rgb_delta += abs(color0.Alpha - color1.Alpha);
                }
                return rgb_delta;
            }

            // Calculates *|x| and *|y| for the Z-order curve value |z|.
            /// <summary>
            /// 通过反莫顿编码下的颜色索引计算对应的坐标
            /// </summary>
            /// <param name="z">反莫顿编码的颜色索引</param>
            /// <param name="x">对应x坐标指针</param>
            /// <param name="y">对应y坐标指针</param>
            static void FromZOrder(uint z, uint* x, uint* y, uint width, uint height, bool versionII)
            {
                if (versionII)
                {
                    *y = z / width;
                    *x = z % width;
                }
                else
                {
                    uint minB = width;
                    if (height < width) minB = height;
                    uint AddTimes = z / (minB * minB);
                    uint ReadZ = z & (minB * minB - 1);
                    *x = *y = 0;
                    for (int j = 0; j < 16; j++)
                    {
                        *x |= ((ReadZ >> (j * 2 + 1)) & 1) << j;
                        *y |= ((ReadZ >> (j * 2 + 0)) & 1) << j;
                    }
                    if (width > height)
                    {
                        *x += AddTimes * minB;
                    }
                    else if (width < height)
                    {
                        *y += AddTimes * minB;
                    }
                }
            }

            /// <summary>
            /// 获取用于“与运算”取指定数量位数据的值
            /// </summary>
            /// <param name="num_ones">取的数据的位数</param>
            /// <returns>用于“与运算”取指定数量位数据的值</returns>
            static uint GetMask(int num_ones)
            {
                return (uint)((1 << num_ones) - 1);
            }

            // Returns the result of encoding the 8 bits of input down as |bit_depth| bits
            // and then decoding back up to 8 bits.
            // Encoding will simply preserve only the top |bit_depth| bits.
            // Decoding will bitwise-or these bits repeatedly into the result, so that the
            // output values range as smoothly as possible from 0 to 255.
            /// <summary>
            /// 返回将输入的8位整数降为指定位数的整数，然后解码回8位的结果
            /// </summary>
            /// <param name="input">输入的8位整数</param>
            /// <param name="bit_depth">位数</param>
            /// <returns>降级然后解码后的8位整数</returns>
            static byte ApplyBitDepthReduction(byte input, int bit_depth)
            {
                byte encoding_mask = (byte)((GetMask(bit_depth) << (8 - bit_depth)) & 0xFF);
                byte encoded_bits = (byte)(input & encoding_mask);
                byte result = (byte)(encoded_bits | (encoded_bits >> bit_depth));
                if (bit_depth <= 3)
                {
                    // The encoded bits will have to be repeated again for the least significant
                    // output bits.
                    result |= (byte)(encoded_bits >> (bit_depth << 1));
                }
                return result;
            }

            //-----------------------------------------------------------------------------

            //
            // PVRTC-specific helper functions operating on pixels and blocks of pixels.
            //


            /// <summary>
            /// 对高频低频图中的两个颜色进行线性插值生成调色板，并返回指定索引位置的颜色
            /// 4BPP插值方式依赖于索引和ModulationFlags
            /// 如果ModulationFlags是0，那么对不同索引有如下四种插值方式：
            ///    0 = color0
            ///    1 = 5/8ths color0, 3/8ths color1
            ///    2 = 3/8ths color0, 5/8ths color1
            ///    3 = color1
            /// 如果ModulationFlags是1，那么对不同索引有如下四种插值方式：
            ///    0 = color0
            ///    1 = 4/8ths color0, 4/8ths color1
            ///    2 = 4/8ths color0, 4/8ths color1, Alpha = 0
            ///    3 = color1
            /// </summary>
            /// <param name="color0">高频信号颜色</param>
            /// <param name="color1">低频信号颜色</param>
            /// <param name="mod">颜色索引</param>
            /// <param name="flags">调制模式</param>
            /// <returns>索引对应颜色</returns>
            static YFColor ApplyModulation_4BPP(YFColor color0, YFColor color1, uint mod, ModulationMode_4BPP flags)
            {
                YFColor result = default(YFColor);
                switch (flags)
                {
                    case ModulationMode_4BPP.StandardBilinear:
                        switch (mod)
                        {
                            case 0:
                                result = color0;
                                break;
                            case 1:
                                result.Red = (byte)((5 * color0.Red + 3 * color1.Red) >> 3);
                                result.Green = (byte)((5 * color0.Green + 3 * color1.Green) >> 3);
                                result.Blue = (byte)((5 * color0.Blue + 3 * color1.Blue) >> 3);
                                result.Alpha = (byte)((5 * color0.Alpha + 3 * color1.Alpha) >> 3);
                                break;
                            case 2:
                                result.Red = (byte)((3 * color0.Red + 5 * color1.Red) >> 3);
                                result.Green = (byte)((3 * color0.Green + 5 * color1.Green) >> 3);
                                result.Blue = (byte)((3 * color0.Blue + 5 * color1.Blue) >> 3);
                                result.Alpha = (byte)((3 * color0.Alpha + 5 * color1.Alpha) >> 3);
                                break;
                            case 3:
                                result = color1;
                                break;
                        }
                        break;
                    case ModulationMode_4BPP.PunchThrough:
                        switch (mod)
                        {
                            case 0:
                                result = color0;
                                break;
                            case 1:
                                result.Red = (byte)((color0.Red + color1.Red) >> 1);
                                result.Green = (byte)((color0.Green + color1.Green) >> 1);
                                result.Blue = (byte)((color0.Blue + color1.Blue) >> 1);
                                result.Alpha = (byte)((color0.Alpha + color1.Alpha) >> 1);
                                break;
                            case 2:
                                result.Red = (byte)((color0.Red + color1.Red) >> 1);
                                result.Green = (byte)((color0.Green + color1.Green) >> 1);
                                result.Blue = (byte)((color0.Blue + color1.Blue) >> 1);
                                result.Alpha = 0;
                                break;
                            case 3:
                                result = color1;
                                break;
                        }
                        break;
                }
                return result;
            }

            // Returns the color interpolated between |color0| and |color1| as specified by
            // |mod| which can range from 0 to 3:
            //   0 = color0
            //   1 = 5/8ths color0, 3/8ths color1
            //   2 = 3/8ths color0, 5/8ths color1
            //   3 = color1
            /// <summary>
            /// 对高频低频图中的两个颜色进行线性插值生成调色板，并返回指定索引位置的颜色
            /// 2BPP插值方式依赖于索引，对不同索引有如下四种插值方式：
            ///    0 = color0
            ///    1 = 5/8ths color0, 3/8ths color1
            ///    2 = 3/8ths color0, 5/8ths color1
            ///    3 = color1
            /// </summary>
            /// <param name="color0">高频信号颜色</param>
            /// <param name="color1">低频信号颜色</param>
            /// <param name="mod">颜色索引</param>
            /// <returns>索引对应颜色</returns>
            static YFColor ApplyModulation_2BPP(YFColor color0, YFColor color1, uint mod)
            {
                YFColor result;// = color0;
                switch (mod)
                {
                    case 0:
                        result = color0;
                        break;
                    case 1:
                        result.Red = (byte)((5 * color0.Red + 3 * color1.Red) >> 3);
                        result.Green = (byte)((5 * color0.Green + 3 * color1.Green) >> 3);
                        result.Blue = (byte)((5 * color0.Blue + 3 * color1.Blue) >> 3);
                        result.Alpha = (byte)((5 * color0.Alpha + 3 * color1.Alpha) >> 3);
                        break;
                    case 2:
                        result.Red = (byte)((3 * color0.Red + 5 * color1.Red) >> 3);
                        result.Green = (byte)((3 * color0.Green + 5 * color1.Green) >> 3);
                        result.Blue = (byte)((3 * color0.Blue + 5 * color1.Blue) >> 3);
                        result.Alpha = (byte)((3 * color0.Alpha + 5 * color1.Alpha) >> 3);
                        break;
                    case 3:
                        result = color1;
                        break;
                    default:
                        throw new Exception($"PVRTCCoder: ApplyModulation_2BPP with unknow mod {mod}");
                }
                return result;
            }

            // 返回最能表示color的索引
            /// <summary>
            /// 在高频低频颜色生成的调色版中返回最能表示指定颜色的索引
            /// 函数在生成调色版后，对给定颜色依次计算每个颜色的差异值
            /// 然后选择差异值最小的索引，返回该索引值
            /// </summary>
            /// <param name="color">给定颜色</param>
            /// <param name="color0">高频信号颜色</param>
            /// <param name="color1">低频信号颜色</param>
            /// <param name="flags">调制模式</param>
            /// <returns>最适合颜色的索引</returns>
            static uint BestModulation_4BPP(YFColor color, YFColor color0, YFColor color1, ModulationMode_4BPP flags, bool hasA)
            {
                uint diff = ColorDiff(color, color0, hasA);
                uint best_diff = diff;
                uint best_mod = 0;

                for (uint current_mod = 1; current_mod < 4; ++current_mod)
                {
                    YFColor current_color = ApplyModulation_4BPP(color0, color1, current_mod, flags);
                    diff = ColorDiff(color, current_color, hasA);
                    if (diff < best_diff)
                    {
                        best_diff = diff;
                        best_mod = current_mod;
                    }
                }

                return best_mod;
            }

            // Returns which modulation (from 0 through 3) best represents |color| given
            // the color palette |color0| and |color1|.
            // 返回最能表示color的索引
            /// <summary>
            /// 在高频低频颜色生成的调色版中返回最能表示指定颜色的索引
            /// 函数在生成调色版后，对给定颜色依次计算每个颜色的差异值
            /// 然后选择差异值最小的索引，返回该索引值
            /// </summary>
            /// <param name="color">给定颜色</param>
            /// <param name="color0">高频信号颜色</param>
            /// <param name="color1">低频信号颜色</param>
            /// <returns>最适合颜色的索引</returns>
            static uint BestModulation_2BPP(YFColor color, YFColor color0, YFColor color1, bool hasA)
            {
                uint diff = ColorDiff(color, color0, hasA);
                uint best_diff = diff;
                uint best_mod = 0;

                for (uint current_mod = 1; current_mod < 4; ++current_mod)
                {
                    YFColor current_color = ApplyModulation_2BPP(color0, color1, current_mod);
                    diff = ColorDiff(color, current_color, hasA);
                    if (diff < best_diff)
                    {
                        best_diff = diff;
                        best_mod = current_mod;
                    }
                    else
                    {
                        // If it's not getting better here, it won't get better later.
                        return best_mod;
                    }
                }

                return best_mod;
            }

            /// <summary>
            /// 返回指定位置在四种颜色之间进行插值所得颜色（我写的位置可能有误）
            /// </summary>
            /// <param name="color00">左上角颜色</param>
            /// <param name="color01">右上角颜色</param>
            /// <param name="color10">左下角颜色</param>
            /// <param name="color11">右下角颜色</param>
            /// <param name="px">在四个颜色块中的x坐标</param>
            /// <param name="py">在四个颜色块中的y坐标</param>
            /// <returns>插值所得颜色</returns>
            static YFColor Interpolate4_4BPP(YFColor color00, YFColor color01, YFColor color10, YFColor color11, uint px, uint py)
            {
                // Calculate the weights that should be applied to the four input colors.
                uint a = (kBlockHeight_4BPP - py) * (kBlockWidth_4BPP - px);
                uint b = (kBlockHeight_4BPP - py) * px;
                uint c = py * (kBlockWidth_4BPP - px);
                uint d = py * px;
                // Apply these weights.
                uint downscale = kBlockWidth_4BPP * kBlockHeight_4BPP;
                return new YFColor(
                    (byte)((a * color00.Red + b * color01.Red + c * color10.Red + d * color11.Red) /
                        downscale),
                    (byte)((a * color00.Green + b * color01.Green + c * color10.Green + d * color11.Green) /
                        downscale),
                    (byte)((a * color00.Blue + b * color01.Blue + c * color10.Blue + d * color11.Blue) /
                        downscale),
                    (byte)((a * color00.Alpha + b * color01.Alpha + c * color10.Alpha + d * color11.Alpha) /
                        downscale));
            }

            // Returns a color bilinearly interpolated between the four input colors.
            // |px| ranges from 0 (pure |color00| or |color01|) to
            //      kBlockWidth (pure |color10| or color11|).
            // |py| ranges from 0 (pure |color00| or |color10|) to
            //      kBlockHeight (pure |color01| or |color11|).
            /// <summary>
            /// 返回指定位置在四种颜色之间进行插值所得颜色（我写的位置可能有误）
            /// </summary>
            /// <param name="color00">左上角颜色</param>
            /// <param name="color01">右上角颜色</param>
            /// <param name="color10">左下角颜色</param>
            /// <param name="color11">右下角颜色</param>
            /// <param name="px">在四个颜色块中的x坐标</param>
            /// <param name="py">在四个颜色块中的y坐标</param>
            /// <returns>插值所得颜色</returns>
            static YFColor Interpolate4_2BPP(YFColor color00, YFColor color01, YFColor color10, YFColor color11, uint px, uint py)
            {
                // Calculate the weights that should be applied to the four input colors.
                uint a = (kBlockHeight_2BPP - py) * (kBlockWidth_2BPP - px);
                uint b = (kBlockHeight_2BPP - py) * px;
                uint c = py * (kBlockWidth_2BPP - px);
                uint d = py * px;
                // Apply these weights.
                uint downscale = kBlockWidth_2BPP * kBlockHeight_2BPP;
                return new YFColor(
                    (byte)((a * color00.Red + b * color01.Red + c * color10.Red + d * color11.Red) /
                        downscale),
                    (byte)((a * color00.Green + b * color01.Green + c * color10.Green + d * color11.Green) /
                        downscale),
                    (byte)((a * color00.Blue + b * color01.Blue + c * color10.Blue + d * color11.Blue) /
                        downscale),
                    (byte)((a * color00.Alpha + b * color01.Alpha + c * color10.Alpha + d * color11.Alpha) /
                        downscale));
            }

            /// <summary>
            /// 对于整个图像和给定点的坐标，返回此点在输入图像的双线性放大版本中
            /// 像素的颜色
            /// 输入图像在宽度上放大了kBlockWidth，在高度上放大了kBlockHeight
            /// 插值覆盖图像的所有四条边
            /// 对于放大图像中大小为kBlockWidth * kBlockHeight的每个像素块，以左
            /// 上角为坐标原点，在(kBlockWidth / 2，kBlockHeight / 2)位置处的像
            /// 素将使用不相关的颜色，其余将进行插值。
            /// 根据
            /// https://www.khronos.org/registry/gles/extensions/IMG/IMG_texture_compression_pvrtc.txt
            /// 宽度和高度必须是2的幂
            /// </summary>
            /// <param name="source">图像指针</param>
            /// <param name="width">图像宽度，2的幂</param>
            /// <param name="height">图像高度，2的幂</param>
            /// <param name="x">给定点的x坐标</param>
            /// <param name="y">给定点的y坐标</param>
            /// <returns>插值后的颜色</returns>
            static YFColor GetInterpolatedColor4BPP(YFColor* source, uint width, uint height, uint x, uint y)
            {
                // The left, top, right and bottom edges of the 2x2 pixel block in the source
                // image that will be used to interpolate. Note that through wrapping (for
                // example) source_left may be to the right of source_right.
                // width and height are not power-of-two, so we can not use '&' instead of '%'.
                uint source_left =
                    ((x - kBlockWidth_4BPP / 2) % width) >> (int)kLog2BlockWidth_4BPP;
                uint source_top =
                    ((y - kBlockHeight_4BPP / 2) % height) >> (int)kLog2BlockHeight_4BPP;
                uint source_right =
                   (source_left + 1) % (width >> (int)kLog2BlockWidth_4BPP);
                uint source_bottom =
                   (source_top + 1) % (height >> (int)kLog2BlockHeight_4BPP);

                // The bilinear weights to be used for interpolation.
                uint x_weight = (x + kBlockWidth_4BPP / 2) % kBlockWidth_4BPP;
                uint y_weight = (y + kBlockHeight_4BPP / 2) % kBlockHeight_4BPP;

                uint source_width = width / kBlockWidth_4BPP;
                YFColor color00 = source[source_top * source_width + source_left];
                YFColor color01 = source[source_top * source_width + source_right];
                YFColor color10 = source[source_bottom * source_width + source_left];
                YFColor color11 = source[source_bottom * source_width + source_right];

                return Interpolate4_4BPP(color00, color01, color10, color11, x_weight, y_weight);
            }

            // Returns the color for a pixel in a bilinearly upscaled version of an input
            // image. The input image is upscaled kBlockWidth in width and kBlockHeight in
            // height. The bilinear interpolation wraps on all four edges of the image.
            // For every block of pixels of size (kBlockWidth * kBlockHeight) in the
            // upscaled image, where the top left is (0,0), the pixel at position
            // (kBlockWidth / 2, kBlockHeight / 2) will use the uninterpolated
            // low-frequency image colors, and the rest will be interpolated.
            // |source| the raw pixel data for the input image.
            // |width| width of the upscaled image.
            // |height| height of the upscaled image.
            // |x| and |y| the position of the pixel in the upscaled image.
            // According to:
            //  https://www.khronos.org/registry/gles/extensions/IMG/IMG_texture_compression_pvrtc.txt
            // width and height must be power-of-two.
            /// <summary>
            /// 对于整个图像和给定点的坐标，返回此点在输入图像的双线性放大版本中
            /// 像素的颜色
            /// 输入图像在宽度上放大了kBlockWidth，在高度上放大了kBlockHeight
            /// 插值覆盖图像的所有四条边
            /// 对于放大图像中大小为kBlockWidth * kBlockHeight的每个像素块，以左
            /// 上角为坐标原点，在(kBlockWidth / 2，kBlockHeight / 2)位置处的像
            /// 素将使用不相关的颜色，其余将进行插值。
            /// 根据
            /// https://www.khronos.org/registry/gles/extensions/IMG/IMG_texture_compression_pvrtc.txt
            /// 宽度和高度必须是2的幂
            /// </summary>
            /// <param name="source">图像指针</param>
            /// <param name="width">图像宽度，2的幂</param>
            /// <param name="height">图像高度，2的幂</param>
            /// <param name="x">给定点的x坐标</param>
            /// <param name="y">给定点的y坐标</param>
            /// <returns>插值后的颜色</returns>
            static YFColor GetInterpolatedColor2BPP(YFColor* source, uint width, uint height, uint x, uint y)
            {
                // The left, top, right and bottom edges of the 2x2 pixel block in the source
                // image that will be used to interpolate. Note that through wrapping (for
                // example) source_left may be to the right of source_right.
                // width and height are not power-of-two, so we can not use '&' instead of '%'.
                uint source_left =
                    ((x - kBlockWidth_2BPP / 2) % width) >> (int)kLog2BlockWidth_2BPP;
                uint source_top =
                    ((y - kBlockHeight_2BPP / 2) % height) >> (int)kLog2BlockHeight_2BPP;
                uint source_right =
                   (source_left + 1) % (width >> (int)kLog2BlockWidth_2BPP);
                uint source_bottom =
                   (source_top + 1) % (height >> (int)kLog2BlockHeight_2BPP);

                // The bilinear weights to be used for interpolation.
                uint x_weight = (x + kBlockWidth_2BPP / 2) % kBlockWidth_2BPP;
                uint y_weight = (y + kBlockHeight_2BPP / 2) % kBlockHeight_2BPP;

                uint source_width = width / kBlockWidth_2BPP;
                YFColor color00 = source[source_top * source_width + source_left];
                YFColor color01 = source[source_top * source_width + source_right];
                YFColor color10 = source[source_bottom * source_width + source_left];
                YFColor color11 = source[source_bottom * source_width + source_right];

                return Interpolate4_2BPP(color00, color01, color10, color11, x_weight, y_weight);
            }

            // An ordering for colors roughly based on brightness.
            /// <summary>
            /// 大致基于亮度的颜色排序值，用于确定颜色相对是低频还是高频
            /// 原理是求R，G，B，A通道的和
            /// </summary>
            /// <param name="color">给定颜色</param>
            /// <returns>颜色排序值（R，G，B，A通道的和）</returns>
            static uint ColorBrightnessOrder(YFColor color)
            {
                return (uint)color.Red + color.Green + color.Blue + color.Alpha;
            }

            static byte GetChannel(YFColor color, int i, bool hasA)
            {
                return i switch
                {
                    0 => color.Red,
                    1 => color.Green,
                    2 => color.Blue,
                    3 => hasA ? color.Alpha : (byte)0xFF,
                    _ => 0
                };
            }

            // 获取块中颜色的极值
            /// <summary>
            /// 获取一个PVRTC块中的高频低频信号颜色
            /// </summary>
            /// <param name="image">图像指针</param>
            /// <param name="width">图像宽度，2的幂</param>
            /// <param name="height">图像高度，2的幂</param>
            /// <param name="x0">PVRTC块x坐标</param>
            /// <param name="y0">PVRTC块y坐标</param>
            /// <param name="out_index_0">高频信号颜色指针</param>
            /// <param name="out_index_1">低频信号颜色指针</param>
            static void GetExtremesFast_4BPP(YFColor* image, uint width, uint height, uint x0, uint y0, uint* out_index_0, uint* out_index_1, bool hasA)
            {
                // Consider 5 different pairs; lightness, then R, G, B, A axes.
                uint* UINT_BUFFER = stackalloc uint[20];
                uint** best_fitness = stackalloc uint*[5]; //uint best_fitness[5][10]
                uint** best_index = stackalloc uint*[5]; //uint best_index[5][10]
                for (int i = 0; i < 5; i++)
                {
                    best_fitness[i] = UINT_BUFFER;
                    UINT_BUFFER += 2;
                }
                for (int i = 0; i < 5; i++)
                {
                    best_index[i] = UINT_BUFFER;
                    UINT_BUFFER += 2;
                }
                for (uint i = 0; i < 5; i++)
                {
                    // For each pair of colors, the first must have the lowest possible value
                    // for the tested fitness, the second the highest possible; hence
                    // initialize "best" with extreme high and low values.
                    best_fitness[i][0] = 0xFFFFFFFFU;
                    best_fitness[i][1] = 0;
                    best_index[i][0] = 0;
                    best_index[i][1] = 0;
                }

                for (uint y = y0; y < y0 + kBlockHeight_4BPP; y++)
                {
                    for (uint x = x0; x < x0 + kBlockWidth_4BPP; x++)
                    {
                        uint x_wrapped = (x + width) % width;
                        uint y_wrapped = (y + height) % height;
                        uint index = y_wrapped * width + x_wrapped;
                        YFColor color = image[index];

                        // For the first pair, use the lightness.
                        uint lightness = (uint)((77 * color.Red + 150 * color.Green + 28 * color.Blue) / 256);
                        if (lightness < best_fitness[0][0])
                        {
                            best_fitness[0][0] = lightness;
                            best_index[0][0] = index;
                        }
                        if (lightness > best_fitness[0][1])
                        {
                            best_fitness[0][1] = lightness;
                            best_index[0][1] = index;
                        }

                        // For the next 4 axes, use the R, G, B or A axis.
                        for (int component = 0; component < 4; component++)
                        {
                            int output_pair = component + 1;
                            byte c = GetChannel(color, component, hasA);
                            if (c < best_fitness[output_pair][0])
                            {
                                best_fitness[output_pair][0] = c;
                                best_index[output_pair][0] = index;
                            }
                            if (c > best_fitness[output_pair][1])
                            {
                                best_fitness[output_pair][1] = c;
                                best_index[output_pair][1] = index;
                            }
                        }
                    }
                }

                // Choose the pair for which the color difference is biggest. This makes the
                // algorithm somewhat principal component-ish.
                uint best_pair_diff = 0;
                uint best_pair = 0;
                for (uint i = 0; i < 5; i++)
                {
                    uint diff = ColorDiff(image[best_index[i][0]], image[best_index[i][1]], hasA);
                    if (diff > best_pair_diff)
                    {
                        best_pair = i;
                        best_pair_diff = diff;
                    }
                }

                *out_index_0 = best_index[best_pair][0];
                *out_index_1 = best_index[best_pair][1];

                // *out_index_0 should be darker than *out_index_1 for consistency; swap if
                // not.
                if (ColorBrightnessOrder(image[*out_index_1]) < ColorBrightnessOrder(image[*out_index_0]))
                {
                    uint temp = *out_index_0;
                    *out_index_0 = *out_index_1;
                    *out_index_1 = temp;
                }
            }

            // Gets two colors that represent extremes of the range of colors within a block
            // in a source image. A fast alternative to principal component analysis.
            // This function also takes care of the wrapping of the coordinates, i.e. |x0|
            // and |y0| can be outside the bounds of the image.
            // |image| the source image pixel data.
            // |width| source image width (must be a power of two).
            // |height| source image height (must be a power of two).
            // |x0| left edge of the block to be considered in pixels.
            // |y0| top edge of the block to be considered in pixels.
            // |out_index_0|, |out_index_1| output colors as indices into |image|.
            /// <summary>
            /// 获取一个PVRTC块中的高频低频信号颜色
            /// </summary>
            /// <param name="image">图像指针</param>
            /// <param name="width">图像宽度，2的幂</param>
            /// <param name="height">图像高度，2的幂</param>
            /// <param name="x0">PVRTC块x坐标</param>
            /// <param name="y0">PVRTC块y坐标</param>
            /// <param name="out_index_0">高频信号颜色指针</param>
            /// <param name="out_index_1">低频信号颜色指针</param>
            static void GetExtremesFast_2BPP(YFColor* image, uint width, uint height, uint x0, uint y0, uint* out_index_0, uint* out_index_1, bool hasA)
            {
                // Consider 5 different pairs; lightness, then R, G, B, A axes.
                uint* UINT_BUFFER = stackalloc uint[20];
                uint** best_fitness = stackalloc uint*[5]; //uint best_fitness[5][10]
                uint** best_index = stackalloc uint*[5]; //uint best_index[5][10]
                for (int i = 0; i < 5; i++)
                {
                    best_fitness[i] = UINT_BUFFER;
                    UINT_BUFFER += 2;
                }
                for (int i = 0; i < 5; i++)
                {
                    best_index[i] = UINT_BUFFER;
                    UINT_BUFFER += 2;
                }
                for (uint i = 0; i < 5; i++)
                {
                    // For each pair of colors, the first must have the lowest possible value
                    // for the tested fitness, the second the highest possible; hence
                    // initialize "best" with extreme high and low values.
                    best_fitness[i][0] = 0xFFFFFFFFU;
                    best_fitness[i][1] = 0;
                    best_index[i][0] = 0;
                    best_index[i][1] = 0;
                }

                for (uint y = y0; y < y0 + kBlockHeight_2BPP; y++)
                {
                    for (uint x = x0; x < x0 + kBlockWidth_2BPP; x++)
                    {
                        uint x_wrapped = (x + width) % width;
                        uint y_wrapped = (y + height) % height;
                        uint index = y_wrapped * width + x_wrapped;
                        YFColor color = image[index];

                        // For the first pair, use the lightness.
                        uint lightness = (uint)((77 * color.Red + 150 * color.Green + 28 * color.Blue) / 256);
                        if (lightness < best_fitness[0][0])
                        {
                            best_fitness[0][0] = lightness;
                            best_index[0][0] = index;
                        }
                        if (lightness > best_fitness[0][1])
                        {
                            best_fitness[0][1] = lightness;
                            best_index[0][1] = index;
                        }

                        // For the next 4 axes, use the R, G, B or A axis.
                        for (int component = 0; component < 4; component++)
                        {
                            int output_pair = component + 1;
                            byte c = GetChannel(color, component, hasA);
                            if (c < best_fitness[output_pair][0])
                            {
                                best_fitness[output_pair][0] = c;
                                best_index[output_pair][0] = index;
                            }
                            if (c > best_fitness[output_pair][1])
                            {
                                best_fitness[output_pair][1] = c;
                                best_index[output_pair][1] = index;
                            }
                        }
                    }
                }

                // Choose the pair for which the color difference is biggest. This makes the
                // algorithm somewhat principal component-ish.
                uint best_pair_diff = 0;
                uint best_pair = 0;
                for (uint i = 0; i < 5; i++)
                {
                    uint diff = ColorDiff(image[best_index[i][0]], image[best_index[i][1]], hasA);
                    if (diff > best_pair_diff)
                    {
                        best_pair = i;
                        best_pair_diff = diff;
                    }
                }

                *out_index_0 = best_index[best_pair][0];
                *out_index_1 = best_index[best_pair][1];

                // *out_index_0 should be darker than *out_index_1 for consistency; swap if
                // not.
                if (ColorBrightnessOrder(image[*out_index_1]) <
                        ColorBrightnessOrder(image[*out_index_0]))
                {
                    uint temp = *out_index_0;
                    *out_index_0 = *out_index_1;
                    *out_index_1 = temp;
                }
            }

            // Returns the color that the input color will become after encoding as an "A"
            // or "B" color in a PVRTC compressed image (where they are converted to
            // 16-bit), and then decoding back to 32-bit.
            // This helps the compressor choose correct modulation values once the "A" and
            // "B" colors are chosen.
            // |is_b| is true if this is the "B" color; "A" and "B" are encoded differently.
            /// <summary>
            /// 将高频或低频颜色降级为PVRTCWord中存储的位数后解码为8位，并返回解码后的值
            /// </summary>
            /// <param name="color">高频或低频颜色</param>
            /// <param name="is_b">若为低频颜色则值为true，否则为false</param>
            /// <returns>降级后解码所得颜色</returns>
            static YFColor ApplyColorChannelReduction(YFColor color, bool is_b, bool hasA)
            {
                if ((!hasA) || color.Alpha == 255)
                {
                    color.Red = ApplyBitDepthReduction(color.Red, 5);
                    color.Green = ApplyBitDepthReduction(color.Green, 5);
                    color.Blue = ApplyBitDepthReduction(color.Blue, is_b ? 5 : 4);
                }
                else
                {
                    color.Red = ApplyBitDepthReduction(color.Red, 4);
                    color.Green = ApplyBitDepthReduction(color.Green, 4);
                    color.Blue = ApplyBitDepthReduction(color.Blue, is_b ? 4 : 3);
                    color.Alpha = ApplyBitDepthReduction(color.Alpha, 3);
                }
                return color;
            }

            // 将两种颜色和一个调制位编码为32位无符号整数
            /// <summary>
            /// 将高频低频信号颜色和调制位编码为32位无符号整数
            /// </summary>
            /// <param name="colora">高频颜色信号</param>
            /// <param name="colorb">低频颜色信号</param>
            /// <param name="mode">调制模式</param>
            /// <returns>编码的32位无符号整数</returns>
            static uint EncodeColors_4BPP(YFColor colora, YFColor colorb, ModulationMode_4BPP mode, bool versionII, bool hasA)
            {
                uint value = 0;

                if (versionII)
                {
                    if ((!hasA) || (colora.Alpha == 255 && colorb.Alpha == 255))
                    {
                        SetBits(31, 1, 1, &value);
                        SetBits(16, 5, colorb.Blue >> 3, &value);
                        SetBits(21, 5, colorb.Green >> 3, &value);
                        SetBits(26, 5, colorb.Red >> 3, &value);
                        SetBits(15, 1, 0, &value); // Hard
                        SetBits(1, 4, colora.Blue >> 4, &value);
                        SetBits(5, 5, colora.Green >> 3, &value);
                        SetBits(10, 5, colora.Red >> 3, &value);
                    }
                    else
                    {
                        SetBits(31, 1, 0, &value);
                        SetBits(16, 4, colorb.Blue >> 4, &value);
                        SetBits(20, 4, colorb.Green >> 4, &value);
                        SetBits(24, 4, colorb.Red >> 4, &value);
                        SetBits(28, 3, colorb.Alpha >> 5, &value);
                        SetBits(15, 1, 0, &value); // Hard
                        SetBits(1, 3, colora.Blue >> 5, &value);
                        SetBits(4, 4, colora.Green >> 4, &value);
                        SetBits(8, 4, colora.Red >> 4, &value);
                        SetBits(12, 3, colora.Alpha >> 5, &value);
                    }
                }
                else
                {
                    if ((!hasA) || colora.Alpha == 255)
                    {
                        SetBits(15, 1, 1, &value);
                        SetBits(1, 4, colora.Blue >> 4, &value);
                        SetBits(5, 5, colora.Green >> 3, &value);
                        SetBits(10, 5, colora.Red >> 3, &value);
                    }
                    else
                    {
                        SetBits(15, 1, 0, &value);
                        SetBits(1, 3, colora.Blue >> 5, &value);
                        SetBits(4, 4, colora.Green >> 4, &value);
                        SetBits(8, 4, colora.Red >> 4, &value);
                        SetBits(12, 3, colora.Alpha >> 5, &value);
                    }
                    if ((!hasA) || colorb.Alpha == 255)
                    {
                        SetBits(31, 1, 1, &value);
                        SetBits(16, 5, colorb.Blue >> 3, &value);
                        SetBits(21, 5, colorb.Green >> 3, &value);
                        SetBits(26, 5, colorb.Red >> 3, &value);
                    }
                    else
                    {
                        SetBits(31, 1, 0, &value);
                        SetBits(16, 4, colorb.Blue >> 4, &value);
                        SetBits(20, 4, colorb.Green >> 4, &value);
                        SetBits(24, 4, colorb.Red >> 4, &value);
                        SetBits(28, 3, colorb.Alpha >> 5, &value);
                    }
                }
                SetBits(0, 1, mode == ModulationMode_4BPP.StandardBilinear ? 0 : 1, &value);
                return value;
            }

            // Encode two colors and a modulation mode into an unsigned int.
            // The encoding is as follows, in the direction from MSB to LSB:
            // 16 bit |colora|, 15 bit |colorb|, 1 bit |mod_mode|.
            // Opaque colors are: 1 bit 1, 5 bit R, 5 bit G, 4/5 bit B.
            // Translucent colors are: 1 bit 0, 3 bit A, 4 bit R, 4 bit G, 3/4 bit B.
            /// <summary>
            /// 将高频低频信号颜色和调制位编码为32位无符号整数
            /// </summary>
            /// <param name="colora">高频颜色信号</param>
            /// <param name="colorb">低频颜色信号</param>
            /// <param name="mode">调制模式</param>
            /// <returns>编码的32位无符号整数</returns>
            static uint EncodeColors_2BPP(YFColor colora, YFColor colorb, ModulationMode_2BPP mode, bool versionII, bool hasA)
            {
                uint value = 0;
                if (versionII)
                {
                    if ((!hasA) || (colora.Alpha == 255 && colorb.Alpha == 255))
                    {
                        SetBits(31, 1, 1, &value);
                        SetBits(16, 5, colorb.Blue >> 3, &value);
                        SetBits(21, 5, colorb.Green >> 3, &value);
                        SetBits(26, 5, colorb.Red >> 3, &value);
                        SetBits(15, 1, 0, &value);
                        SetBits(1, 4, colora.Blue >> 4, &value);
                        SetBits(5, 5, colora.Green >> 3, &value);
                        SetBits(10, 5, colora.Red >> 3, &value);
                    }
                    else
                    {
                        SetBits(31, 1, 0, &value);
                        SetBits(16, 4, colorb.Blue >> 4, &value);
                        SetBits(20, 4, colorb.Green >> 4, &value);
                        SetBits(24, 4, colorb.Red >> 4, &value);
                        SetBits(28, 3, colorb.Alpha >> 5, &value);
                        SetBits(15, 1, 0, &value);
                        SetBits(1, 3, colora.Blue >> 5, &value);
                        SetBits(4, 4, colora.Green >> 4, &value);
                        SetBits(8, 4, colora.Red >> 4, &value);
                        SetBits(12, 3, colora.Alpha >> 5, &value);
                    }
                }
                else
                {
                    if ((!hasA) || colora.Alpha == 255)
                    {
                        SetBits(15, 1, 1, &value);
                        SetBits(1, 4, colora.Blue >> 4, &value);
                        SetBits(5, 5, colora.Green >> 3, &value);
                        SetBits(10, 5, colora.Red >> 3, &value);
                    }
                    else
                    {
                        SetBits(15, 1, 0, &value);
                        SetBits(1, 3, colora.Blue >> 5, &value);
                        SetBits(4, 4, colora.Green >> 4, &value);
                        SetBits(8, 4, colora.Red >> 4, &value);
                        SetBits(12, 3, colora.Alpha >> 5, &value);
                    }
                    if ((!hasA) || colorb.Alpha == 255)
                    {
                        SetBits(31, 1, 1, &value);
                        SetBits(16, 5, colorb.Blue >> 3, &value);
                        SetBits(21, 5, colorb.Green >> 3, &value);
                        SetBits(26, 5, colorb.Red >> 3, &value);
                    }
                    else
                    {
                        SetBits(31, 1, 0, &value);
                        SetBits(16, 4, colorb.Blue >> 4, &value);
                        SetBits(20, 4, colorb.Green >> 4, &value);
                        SetBits(24, 4, colorb.Red >> 4, &value);
                        SetBits(28, 3, colorb.Alpha >> 5, &value);
                    }
                }
                SetBits(0, 1, mode == ModulationMode_2BPP.Direct1BPP ? 0 : 1, &value);
                return value;
            }

            /// <summary>
            /// 设置给定32位无符号整数中的指定位为指定值
            /// </summary>
            /// <param name="start_bit">起始位</param>
            /// <param name="num_bits">位长度</param>
            /// <param name="value">设置的值</param>
            /// <param name="bits">给定32位无符号整数的指针</param>
            static void SetBits(int start_bit, int num_bits, int value, uint* bits)
            {
                // Negative numbers must be converted to unsigned so they do not
                // sign-extend into the rest of the target value. This cast takes
                // care of both positive and negative cases.
                uint mask = GetMask(num_bits);
                uint unsigned_value = (uint)value & mask;

                // Clear any bits that are set, then set the new bits.
                *bits = (*bits & ~(mask << start_bit)) | (unsigned_value << start_bit);
            }

            // 计算给定块使用的调制模式
            /// <summary>
            /// 计算PVRTC块应该使用哪种调制模式（4BPP只支持StandardBilinear，不会计算，直接返回）
            /// </summary>
            /// <param name="image_mod">图像指针</param>
            /// <param name="width">图像宽度，2的幂</param>
            /// <param name="height">图像高度，2的幂</param>
            /// <param name="block_x">PVRTC块起始x坐标</param>
            /// <param name="block_y">PVRTC块起始y坐标</param>
            /// <returns>调制模式</returns>
            static ModulationMode_4BPP CalculateBlockModulationMode_4BPP(byte* image_mod, uint width, uint height, uint block_x, uint block_y)
            {
                return ModulationMode_4BPP.StandardBilinear;
            }

            // Works out which modulation mode to use for a given block in an image.
            // |image_mod| the modulation information for the image.
            // |width| and |height| image_mod pixel dimensions (must be a power of two).
            // |block_x| block x coordinate, i.e. ranging from 0 to |width| / kBlockWidth.
            // |block_y| block y coordinate, i.e. ranging from 0 to |height| / kBlockHeight.
            /// <summary>
            /// 计算PVRTC块应该使用哪种调制模式
            /// </summary>
            /// <param name="image_mod">图像指针</param>
            /// <param name="width">图像宽度，2的幂</param>
            /// <param name="height">图像高度，2的幂</param>
            /// <param name="block_x">PVRTC块起始x坐标</param>
            /// <param name="block_y">PVRTC块起始y坐标</param>
            /// <returns>调制模式</returns>
            static ModulationMode_2BPP CalculateBlockModulationMode_2BPP(byte* image_mod, uint width, uint height, uint block_x, uint block_y)
            {
                // A count of how many pixels are best served by modulation values 2 or 3,
                // i.e. intermediate between the extremes of one color or the other.
                uint intermediate_value_count = 0;

                // A measure of how much variation between pixels there is horizontally.
                uint horizontal_count = 0;

                // A measure of how much variation between pixels there is vertically.
                uint vertical_count = 0;

                for (uint y = 0; y < kBlockHeight_2BPP; y++)
                {
                    for (uint x = 0; x < kBlockWidth_2BPP; x++)
                    {
                        uint index = (block_y * kBlockHeight_2BPP + y) * width + (block_x * kBlockWidth_2BPP + x);

                        if (image_mod[index] == 1 || image_mod[index] == 2)
                        {
                            intermediate_value_count++;
                        }

                        // Index of adjacent horizontal pixel in |image_mod|.
                        uint index_adjacent_horizontal = (block_y * kBlockHeight_2BPP + y) * width + ((block_x * kBlockWidth_2BPP + x + 1) / width);

                        // Index of adjacent vertical pixel in |image_mod|.
                        uint index_adjacent_vertical = ((block_y * kBlockHeight_2BPP + y + 1) / height) * width + block_x * kBlockWidth_2BPP + x;

                        horizontal_count += abs(image_mod[index] - image_mod[index_adjacent_vertical]);
                        vertical_count += abs(image_mod[index] - image_mod[index_adjacent_horizontal]);
                    }
                }

                if (intermediate_value_count <= 4)
                {
                    return ModulationMode_2BPP.Direct1BPP;
                }

                const uint absolute_threshold = 10;
                const uint ratio_threshold = 2;

                if (vertical_count > absolute_threshold && vertical_count > horizontal_count * ratio_threshold)
                {
                    return ModulationMode_2BPP.VerticallyInterpolated2BPP;
                }
                else if (horizontal_count > absolute_threshold && horizontal_count > vertical_count * ratio_threshold)
                {
                    return ModulationMode_2BPP.HorizontallyInterpolated2BPP;
                }

                return ModulationMode_2BPP.Interpolated2BPP;
            }

            // 计算给定块中的32位无符号整数调制信息
            /// <summary>
            /// 将给定块中的颜色索引编码为32位无符号整数
            /// </summary>
            /// <param name="image_mod">颜色索引的指针</param>
            /// <param name="width">图像宽度</param>
            /// <param name="height">图像高度</param>
            /// <param name="block_x">PVRTC块起始x坐标</param>
            /// <param name="block_y">PVRTC块起始y坐标</param>
            /// <returns>编码的32位无符号整数</returns>
            static uint CalculateBlockModulationData_4BPP(byte* image_mod, uint width, uint height, uint block_x, uint block_y)
            {
                uint result = 0;
                int bitpos = 0;
                for (uint y = 0; y < 4; y++)
                {
                    for (uint x = 0; x < 4; x++)
                    {
                        SetBits(bitpos, 2, image_mod[(block_y * 4 + y) * width + block_x * 4 + x], &result);
                        bitpos += 2;
                    }
                }
                return result;
            }

            // Calculates the 32 bits of modulation information to store for a given block
            // in an image.
            // |image_mod| the modulation information for the image.
            // |width| and |height| image_mod pixel dimensions.
            // |block_x| block x coordinate, i.e. ranging from 0 to |width| / kBlockWidth.
            // |block_y| block y coordinate, i.e. ranging from 0 to |height| / kBlockHeight.
            // |mode| which modulation mode to use.
            /// <summary>
            /// 将给定块中的颜色索引编码为32位无符号整数
            /// </summary>
            /// <param name="image_mod">颜色索引的指针</param>
            /// <param name="width">图像宽度</param>
            /// <param name="height">图像高度</param>
            /// <param name="block_x">PVRTC块起始x坐标</param>
            /// <param name="block_y">PVRTC块起始y坐标</param>
            /// <param name="mode">2BPP的PVRTC块调制模式</param>
            /// <returns>编码的32位无符号整数</returns>
            static uint CalculateBlockModulationData_2BPP(byte* image_mod, uint width, uint height, uint block_x, uint block_y, ModulationMode_2BPP mode)
            {
                uint result = 0;
                int bitpos = 0;
                for (uint y = 0; y < 4; y++)
                {
                    for (uint x = 0; x < 8; x++)
                    {
                        uint index = (block_y * 4 + y) * width + (block_x * 8 + x);

                        if (mode == ModulationMode_2BPP.Direct1BPP)
                        {
                            int bit = image_mod[index] / 2;
                            SetBits(bitpos, 1, bit, &result);
                            bitpos++;
                        }
                        else
                        {
                            if (((x ^ y) & 1) != 0) continue;  // checkerboard
                            int bit = image_mod[index];
                            // The bits at position 0 (0,0) and at position 20 (4,2) are the ones
                            // that use only a single bit for the modulation value, and the other
                            // bit for selecting the sub-mode.
                            if (bitpos == 0)
                            {
                                // The saved bit chooses average-of-4 or "other".
                                if (mode == ModulationMode_2BPP.Interpolated2BPP)
                                    bit &= 2;
                                else
                                    bit |= 1;
                            }
                            else if (bitpos == 20)
                            {
                                // The saved bit chooses vertical versus horizontal.
                                if (mode == ModulationMode_2BPP.VerticallyInterpolated2BPP)
                                    bit |= 1;
                                else
                                    bit &= 2;
                            }

                            SetBits(bitpos, 2, bit, &result);
                            bitpos += 2;
                        }
                    }
                }
                return result;
            }

            //-----------------------------------------------------------------------------

            //
            // Helper functions operating on entire images.
            //

            // 将原图像降频为高频和低频图像
            /// <summary>
            /// 将原图像降频为高频和低频图像
            /// </summary>
            /// <param name="image">图像指针</param>
            /// <param name="width">图像宽度，2的幂</param>
            /// <param name="height">图像高度，2的幂</param>
            /// <param name="outa">高频图指针</param>
            /// <param name="outb">低频图指针</param>
            static void Morph_4BPP(YFColor* image, uint width, uint height, YFColor* outa, YFColor* outb, bool hasA)
            {
                for (uint y = 0; y < height; y += kBlockHeight_4BPP)
                {
                    for (uint x = 0; x < width; x += kBlockWidth_4BPP)
                    {
                        uint indexa = 0;
                        uint indexb = 0;
                        GetExtremesFast_4BPP(image, width, height, x, y, &indexa, &indexb, hasA);

                        uint index_out = y / kBlockHeight_4BPP * (width / kBlockWidth_4BPP) + (x / kBlockWidth_4BPP);

                        outa[index_out] = ApplyColorChannelReduction(image[indexa], false, hasA);
                        outb[index_out] = ApplyColorChannelReduction(image[indexb], true, hasA);
                    }
                }
            }

            // Fills in the two low-resolution images representing the "a" and "b" colors in
            // the source |image|.
            /// <summary>
            /// 将原图像降频为高频和低频图像
            /// </summary>
            /// <param name="image">图像指针</param>
            /// <param name="width">图像宽度，2的幂</param>
            /// <param name="height">图像高度，2的幂</param>
            /// <param name="outa">高频图指针</param>
            /// <param name="outb">低频图指针</param>
            static void Morph_2BPP(YFColor* image, uint width, uint height, YFColor* outa, YFColor* outb, bool hasA)
            {
                for (uint y = 0; y < height; y += kBlockHeight_2BPP)
                {
                    for (uint x = 0; x < width; x += kBlockWidth_2BPP)
                    {
                        uint indexa = 0;
                        uint indexb = 0;
                        GetExtremesFast_2BPP(image, width, height, x, y, &indexa, &indexb, hasA);

                        uint index_out = y / kBlockHeight_2BPP * (width / kBlockWidth_2BPP) + (x / kBlockWidth_2BPP);

                        outa[index_out] = ApplyColorChannelReduction(image[indexa], false, hasA);
                        outb[index_out] = ApplyColorChannelReduction(image[indexb], true, hasA);
                    }
                }
            }

            // 获取图像每个点的索引
            /// <summary>
            /// 获取每个点在高频低频颜色线性插值生成调色板中最适合的索引
            /// </summary>
            /// <param name="image">图像指针</param>
            /// <param name="width">图像宽度</param>
            /// <param name="height">图像高度</param>
            /// <param name="imagea">高频图指针</param>
            /// <param name="imageb">低频图指针</param>
            /// <param name="mod">索引指针</param>
            static void Modulate_4BPP(YFColor* image, uint width, uint height, YFColor* imagea, YFColor* imageb, byte* mod, bool hasA)
            {
                for (uint y = 0; y < height; y++)
                {
                    for (uint x = 0; x < width; x++)
                    {
                        YFColor colora = GetInterpolatedColor4BPP(imagea, width, height, x, y);
                        YFColor colorb = GetInterpolatedColor4BPP(imageb, width, height, x, y);
                        *mod++ = (byte)BestModulation_4BPP(*image++, colora, colorb, ModulationMode_4BPP.StandardBilinear, hasA);
                    }
                }
            }

            // Given a source |image| and two low-resolution "a" and "b" images, creates a
            // 2-bits-per-pixel "mod" image, i.e. values between 0 and 3 for each pixel in
            // |image|. Each output pixel is stored in a byte in |mod|, which is assumed
            // to be pre-allocated.
            /// <summary>
            /// 获取每个点在高频低频颜色线性插值生成调色板中最适合的索引
            /// </summary>
            /// <param name="image">图像指针</param>
            /// <param name="width">图像宽度</param>
            /// <param name="height">图像高度</param>
            /// <param name="imagea">高频图指针</param>
            /// <param name="imageb">低频图指针</param>
            /// <param name="mod">索引指针</param>
            static void Modulate_2BPP(YFColor* image, uint width, uint height, YFColor* imagea, YFColor* imageb, byte* mod, bool hasA)
            {
                for (uint y = 0; y < height; y++)
                {
                    for (uint x = 0; x < width; x++)
                    {
                        YFColor colora = GetInterpolatedColor2BPP(imagea, width, height, x, y);
                        YFColor colorb = GetInterpolatedColor2BPP(imageb, width, height, x, y);
                        *mod++ = (byte)BestModulation_2BPP(*image++, colora, colorb, hasA);
                    }
                }
            }

            // 最后一步编码并写入
            /// <summary>
            /// 将所有信息编码并写入PVRTCWord指针
            /// </summary>
            /// <param name="width">图像宽度</param>
            /// <param name="height">图像高度</param>
            /// <param name="imagea">高频信号指针</param>
            /// <param name="imageb">低频信号指针</param>
            /// <param name="image_mod">索引指针</param>
            /// <param name="pvr">PVRTCWord指针</param>
            static void Encode_4BPP(uint width, uint height, YFColor* imagea, YFColor* imageb, byte* image_mod, byte* pvr, bool versionII, bool hasA)
            {
                uint times = width * height / (kBlockWidth_4BPP * kBlockHeight_4BPP);
                // Loop through all output blocks.
                for (uint i = 0; i < times; i++)
                {
                    // The blocks are stored in Z-order; calculate the block x and y.
                    uint block_x = 0;
                    uint block_y = 0;
                    FromZOrder(i, &block_x, &block_y, width / kBlockWidth_4BPP, height / kBlockHeight_4BPP, versionII);

                    // Calculate which kind of encoding is worth doing for this block.
                    ModulationMode_4BPP mode = CalculateBlockModulationMode_4BPP(image_mod, width, height, block_x, block_y);

                    // Given this mode, calculate the 32 bits that represent the block's
                    // modulation information.
                    uint mod_data = CalculateBlockModulationData_4BPP(image_mod, width, height, block_x, block_y);

                    // The 32 bits that represent the 2-color palette for this block and mode.
                    uint color_data = EncodeColors_4BPP(imagea[block_y * (width / kBlockWidth_4BPP) + block_x], imageb[block_y * (width / kBlockWidth_4BPP) + block_x], mode, versionII, hasA);

                    // Write out this information.

                    pvr = Append32(mod_data, pvr);
                    pvr = Append32(color_data, pvr);
                }
            }

            // Takes the calculated "A" and "B" images, and the modulation information, and
            // writes out the data in PVRTC format. Though the input modulation information
            // has 2 bits per pixel of the uncompressed original image, this function will
            // choose on a per-block basis which of the 4 modulation modes to use.
            // |width| and |height| image dimensions.
            // |imagea| the "A" image as described in pvrtc_compressor.h.
            // |imageb| the "B" image as also described.
            // |imagemod| One byte per pixel modulation data, containing 2-bit values.
            // |pvr| output pvrtc data, assumed preallocated.
            /// <summary>
            /// 将所有信息编码并写入PVRTCWord指针
            /// </summary>
            /// <param name="width">图像宽度</param>
            /// <param name="height">图像高度</param>
            /// <param name="imagea">高频信号指针</param>
            /// <param name="imageb">低频信号指针</param>
            /// <param name="image_mod">索引指针</param>
            /// <param name="pvr">PVRTCWord指针</param>
            static void Encode_2BPP(uint width, uint height, YFColor* imagea, YFColor* imageb, byte* image_mod, byte* pvr, bool versionII, bool hasA)
            {
                uint times = width * height / (kBlockWidth_2BPP * kBlockHeight_2BPP);
                // Loop through all output blocks.
                for (uint i = 0; i < times; i++)
                {
                    // The blocks are stored in Z-order; calculate the block x and y.
                    uint block_x = 0;
                    uint block_y = 0;
                    FromZOrder(i, &block_x, &block_y, width / kBlockWidth_2BPP, height / kBlockHeight_2BPP, versionII);

                    // Calculate which kind of encoding is worth doing for this block.
                    ModulationMode_2BPP mode = CalculateBlockModulationMode_2BPP(image_mod, width, height,
                                                                       block_x, block_y);

                    // Given this mode, calculate the 32 bits that represent the block's
                    // modulation information.
                    uint mod_data = CalculateBlockModulationData_2BPP(image_mod, width, height,
                                                                   block_x, block_y, mode);

                    // The 32 bits that represent the 2-color palette for this block and mode.
                    uint color_data = EncodeColors_2BPP(
                        imagea[block_y * (width / kBlockWidth_2BPP) + block_x],
                        imageb[block_y * (width / kBlockWidth_2BPP) + block_x], mode, versionII, hasA);

                    // Write out this information.
                    pvr = Append32(mod_data, pvr);
                    pvr = Append32(color_data, pvr);
                }
            }

            public static void CompressPVRTC(byte* pvr, YFColor* image, uint width, uint height, byte bpp, bool versionII, bool hasA)
            {
                if (bpp == 2)
                {
                    uint low_image_size = (width * height) / (kBlockWidth_2BPP * kBlockHeight_2BPP);
                    int alloc_mem = (int)(2 * low_image_size * sizeof(YFColor) + width * height);
                    using (IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(alloc_mem))
                    {
                        Span<byte> owner_span = owner.Memory.Span[..alloc_mem];
                        fixed (byte* owner_ptr = owner_span)
                        {
                            nuint owner_ptr_num = (nuint)owner_ptr;
                            YFColor* iap = (YFColor*)owner_ptr_num;
                            owner_ptr_num += low_image_size * (uint)sizeof(YFColor);
                            YFColor* ibp = (YFColor*)owner_ptr_num;
                            owner_ptr_num += low_image_size * (uint)sizeof(YFColor);
                            byte* im = (byte*)owner_ptr_num;
                            Morph_2BPP(image, width, height, iap, ibp, hasA);
                            Modulate_2BPP(image, width, height, iap, ibp, im, hasA);
                            Encode_2BPP(width, height, iap, ibp, im, pvr, versionII, hasA);
                        }
                    }
                }
                else if (bpp == 4)
                {
                    uint low_image_size = (width * height) / (kBlockWidth_4BPP * kBlockHeight_4BPP);
                    int alloc_mem = (int)(2 * low_image_size * sizeof(YFColor) + width * height);
                    using (IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(alloc_mem))
                    {
                        Span<byte> owner_span = owner.Memory.Span[..alloc_mem];
                        fixed (byte* owner_ptr = owner_span)
                        {
                            nuint owner_ptr_num = (nuint)owner_ptr;
                            YFColor* iap = (YFColor*)owner_ptr_num;
                            owner_ptr_num += low_image_size * (uint)sizeof(YFColor);
                            YFColor* ibp = (YFColor*)owner_ptr_num;
                            owner_ptr_num += low_image_size * (uint)sizeof(YFColor);
                            byte* im = (byte*)owner_ptr_num;
                            Morph_4BPP(image, width, height, iap, ibp, hasA);
                            Modulate_4BPP(image, width, height, iap, ibp, im, hasA);
                            Encode_4BPP(width, height, iap, ibp, im, pvr, versionII, hasA);
                        }
                    }
                }
            }
        }
    }
}
