#include "PtxHandlerBestQuality.h"

int Transcode(void* inData, void* outData, PVRTuint32 width, PVRTuint32 height, PVRTuint64 inFormat, PVRTexLibColourSpace inColorSpace, PVRTuint64 outFormat, PVRTexLibColourSpace outColorSpace, PVRTexLibCompressorQuality quality, bool dither)
{
    PVRTextureHeader header(inFormat, width, height, 1, 1, 1, 1, inColorSpace, PVRTexLibVariableType::PVRTLVT_UnsignedByteNorm, 0);
    PVRTexture* tex = new PVRTexture(header, inData);
    if (!tex->GetTextureDataSize())
    {
        return 1;
    }
    if (!tex->Transcode(outFormat, PVRTexLibVariableType::PVRTLVT_UnsignedByteNorm, outColorSpace, quality, dither))
    {
        return 2;
    }
    memcpy(outData, tex->GetTextureDataPointer(0), tex->GetTextureDataSize(0));
    PVRTexLib_DestroyTexture(tex);
    return 0;
}

void DecodeBGRA8888Word(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    bitmapDataPtrPtr[0] = wordDataPtrPtr[0]; // B
    bitmapDataPtrPtr[1] = wordDataPtrPtr[1]; // G
    bitmapDataPtrPtr[2] = wordDataPtrPtr[2]; // R
    bitmapDataPtrPtr[3] = wordDataPtrPtr[3]; // A
}

void DecodeRGBA8888Word(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    bitmapDataPtrPtr[0] = wordDataPtrPtr[2]; // B
    bitmapDataPtrPtr[1] = wordDataPtrPtr[1]; // G
    bitmapDataPtrPtr[2] = wordDataPtrPtr[0]; // R
    bitmapDataPtrPtr[3] = wordDataPtrPtr[3]; // A
}

void DecodeRGBA4444Word(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    unsigned short word = wordDataPtrPtr[0] | (wordDataPtrPtr[1] << 8); // RRRRGGGGBBBBAAAA(LE)
    unsigned char temp;
    temp = ((word >> 4) & 0xF);
    bitmapDataPtrPtr[0] = temp | (temp << 4); // B
    temp = ((word >> 8) & 0xF);
    bitmapDataPtrPtr[1] = temp | (temp << 4); // G
    temp = ((word >> 12) & 0xF);
    bitmapDataPtrPtr[2] = temp | (temp << 4); // R
    temp = ((word >> 0) & 0xF);
    bitmapDataPtrPtr[3] = temp | (temp << 4); // A
}

void DecodeRGB565Word(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    unsigned short word = wordDataPtrPtr[0] | (wordDataPtrPtr[1] << 8); // RRRRRGGGGGGBBBBB(LE)
    unsigned char temp;
    temp = ((word >> 0) & 0x1F);
    bitmapDataPtrPtr[0] = (temp >> 2) | (temp << 3); // B
    temp = ((word >> 5) & 0x3F);
    bitmapDataPtrPtr[1] = (temp >> 4) | (temp << 2); // G
    temp = ((word >> 11) & 0x1F);
    bitmapDataPtrPtr[2] = (temp >> 2) | (temp << 3); // R
    bitmapDataPtrPtr[3] = 0xFF; // A
}

void DecodeRGBA5551Word(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    unsigned short word = wordDataPtrPtr[0] | (wordDataPtrPtr[1] << 8); // RRRRRGGGGGBBBBBA(LE)
    unsigned char temp;
    temp = ((word >> 1) & 0x1F);
    bitmapDataPtrPtr[0] = (temp >> 2) | (temp << 3); // B
    temp = ((word >> 6) & 0x1F);
    bitmapDataPtrPtr[1] = (temp >> 2) | (temp << 3); // G
    temp = ((word >> 11) & 0x1F);
    bitmapDataPtrPtr[2] = (temp >> 2) | (temp << 3); // R
    bitmapDataPtrPtr[3] = (word & 0x1) ? 0xFF : 0x0; // A
}

void EncodeBGRA8888Word(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    wordDataPtrPtr[0] = bitmapDataPtrPtr[0]; // B
    wordDataPtrPtr[1] = bitmapDataPtrPtr[1]; // G
    wordDataPtrPtr[2] = bitmapDataPtrPtr[2]; // R
    wordDataPtrPtr[3] = bitmapDataPtrPtr[3]; // A
}

void EncodeRGBA8888Word(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    wordDataPtrPtr[0] = bitmapDataPtrPtr[2]; // R
    wordDataPtrPtr[1] = bitmapDataPtrPtr[1]; // G
    wordDataPtrPtr[2] = bitmapDataPtrPtr[0]; // B
    wordDataPtrPtr[3] = bitmapDataPtrPtr[3]; // A
}

void EncodeRGBA4444Word(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    unsigned short word = 0;
    word |= (bitmapDataPtrPtr[2] >> 4) << 12; // R
    word |= (bitmapDataPtrPtr[1] >> 4) << 8; // G
    word |= (bitmapDataPtrPtr[0] >> 4) << 4; // B
    word |= (bitmapDataPtrPtr[3] >> 4) << 0; // A
    wordDataPtrPtr[0] = (unsigned char)word;
    wordDataPtrPtr[1] = (unsigned char)(word >> 8);
}

void EncodeRGB565Word(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    unsigned short word = 0;
    word |= (bitmapDataPtrPtr[2] >> 3) << 11; // R
    word |= (bitmapDataPtrPtr[1] >> 2) << 5; // G
    word |= (bitmapDataPtrPtr[0] >> 3) << 0; // B
    wordDataPtrPtr[0] = (unsigned char)word;
    wordDataPtrPtr[1] = (unsigned char)(word >> 8);
}

void EncodeRGBA5551Word(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    unsigned short word = 0;
    word |= (bitmapDataPtrPtr[2] >> 3) << 11; // R
    word |= (bitmapDataPtrPtr[1] >> 3) << 6; // G
    word |= (bitmapDataPtrPtr[0] >> 3) << 1; // B
    word |= (bitmapDataPtrPtr[3] >> 7) << 0; // A
    wordDataPtrPtr[0] = (unsigned char)word;
    wordDataPtrPtr[1] = (unsigned char)(word >> 8);
}

extern "C" {
    int WIND_API PtxHandlerAndroidV3H_GetPtxSize(int width, int height, int pitch, int format, int alphaSize) {
        int tex0Size = PtxHandlerAndroidV3H_GetPtxSizeWithoutAlpha(width, height, pitch, format);
        if (format >= 147 && format <= 149) {
            return tex0Size + width * height;
        }
        return tex0Size;
    }

    int WIND_API PtxHandlerAndroidV3H_GetPtxSizeWithoutAlpha(int width, int height, int pitch, int format) {
        switch (format)
        {
        case 21:
        case 22:
        case 23:
            return pitch * Align(height, 32);
        case 30:
        case 148:
            return width * height / 2;
        case 31:
            return width * height / 4;
        case 32:
        case 35:
        case 36:
        case 147:
            return Align(width, 4) * Align(height, 4) / 2;
        case 37:
            return Align(width, 4) * Align(height, 4);
        case 149:
            return width * height * 4;
        default:
            return pitch * height;
        }
    }

    void WIND_API PtxHandlerAndroidV3H_DecodePtx(void* ptxDataPtr, int ptxDataSize, void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize) {
        PVRTuint64 inFormat;
        int codeType = 0;
        void(*decFuncPtr)(unsigned char* wordDataPtr, unsigned char* bitmapDataPtr) = NULL;
        int byteCount = 0;
        switch (ptxFormat) {
        case 0:
            decFuncPtr = &DecodeRGBA8888Word;
            byteCount = 4;
            codeType = 1;
            break;
        case 1:
        default:
            decFuncPtr = &DecodeRGBA4444Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 2:
            decFuncPtr = &DecodeRGB565Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 3:
            decFuncPtr = &DecodeRGBA5551Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 21:
            decFuncPtr = &DecodeRGBA4444Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 22:
            decFuncPtr = &DecodeRGB565Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 23:
            decFuncPtr = &DecodeRGBA5551Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 30:
        case 148:
            inFormat = PVRTLPF_PVRTCI_4bpp_RGBA;
            codeType = 3;
            break;
        case 31:
            inFormat = PVRTLPF_PVRTCI_2bpp_RGBA;
            codeType = 3;
            break;
        case 32:
        case 147:
            inFormat = PVRTLPF_ETC1;
            codeType = 3;
            break;
        case 35:
            inFormat = PVRTLPF_ETC2_RGB;
            codeType = 3;
            break;
        case 36:
            inFormat = PVRTLPF_ETC2_RGB_A1;
            codeType = 3;
            break;
        case 37:
            inFormat = PVRTLPF_ETC2_RGBA;
            codeType = 3;
            break;
        case 149:
            inFormat = PVRTGENPIXELID4('r', 'g', 'b', 'a', 8, 8, 8, 8);
            codeType = 3;
            break;
        }
        if (codeType) {
            if (codeType == 1) {
                // Line by line
                unsigned char* ptxPtrC = (unsigned char*)ptxDataPtr;
                unsigned char* bitmapPtrC = (unsigned char*)bitmapDataPtr;
                for (int y = 0; y < bitmapHeight; y++) {
                    unsigned char* ptxLinePtrC = ptxPtrC;
                    ptxPtrC += ptxPitch;
                    for (int x = 0; x < bitmapWidth; x++) {
                        decFuncPtr(ptxLinePtrC, bitmapPtrC);
                        ptxLinePtrC += byteCount;
                        bitmapPtrC += 4;
                    }
                }
            }
            else if (codeType == 2) {
                // tile by tile
                // maybe cannot use pitch?
                unsigned char* ptxPtrC = (unsigned char*)ptxDataPtr;
                unsigned char* bitmapPtrC = (unsigned char*)bitmapDataPtr;
                for (int yTile = 0; yTile < bitmapHeight; yTile += 32) {
                    for (int xTile = 0; xTile < bitmapWidth; xTile += 32) {
                        for (int y = 0; y < 32; y++) {
                            for (int x = 0; x < 32; x++) {
                                int thisX = xTile + x;
                                int thisY = yTile + y;
                                if (thisX < bitmapWidth && thisY < bitmapHeight) {
                                    decFuncPtr(ptxPtrC, bitmapPtrC + 4 * (thisY * bitmapWidth + thisX));
                                }
                                ptxPtrC += byteCount;
                            }
                        }
                    }
                }
            }
            else if (codeType == 3) {
                // texture by texture
                Transcode(ptxDataPtr, bitmapDataPtr, bitmapWidth, bitmapHeight, inFormat, PVRTLCS_Linear, PVRTGENPIXELID4('b', 'g', 'r', 'a', 8, 8, 8, 8), PVRTLCS_Linear, PVRTLCQ_PVRTCFastest, false);
            }
        }
        if (ptxFormat >= 147 && ptxFormat <= 149) {
            // alpha with no pitch
            unsigned char* alphaDataPtr = (unsigned char*)ptxDataPtr + PtxHandlerAndroidV3H_GetPtxSizeWithoutAlpha(ptxWidth, ptxHeight, ptxPitch, ptxFormat);
            unsigned char* bitmapPtr = (unsigned char*)bitmapDataPtr;
            int area = bitmapWidth * bitmapHeight;
            for (int i = 0; i < area; i++) {
                bitmapPtr[3] = *alphaDataPtr;
                alphaDataPtr++;
                bitmapPtr += 4;
            }
        }
    }

    void WIND_API PtxHandlerAndroidV3H_EncodePtx(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, void* ptxDataPtr, int ptxDataSize, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize) {
        PVRTuint64 outFormat;
        PVRTexLibCompressorQuality quality = PVRTLCQ_PVRTCFastest;
        int codeType = 0;
        void(*encFuncPtr)(unsigned char* wordDataPtr, unsigned char* bitmapDataPtr) = NULL;
        int byteCount = 0;
        switch (ptxFormat) {
        case 0:
            encFuncPtr = &EncodeRGBA8888Word;
            byteCount = 4;
            codeType = 1;
            break;
        case 1:
        default:
            encFuncPtr = &EncodeRGBA4444Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 2:
            encFuncPtr = &EncodeRGB565Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 3:
            encFuncPtr = &EncodeRGBA5551Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 21:
            encFuncPtr = &EncodeRGBA4444Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 22:
            encFuncPtr = &EncodeRGB565Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 23:
            encFuncPtr = &EncodeRGBA5551Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 30:
        case 148:
            outFormat = PVRTLPF_PVRTCI_4bpp_RGBA;
            quality = PVRTLCQ_PVRTCBest;
            codeType = 3;
            break;
        case 31:
            outFormat = PVRTLPF_PVRTCI_2bpp_RGBA;
            quality = PVRTLCQ_PVRTCBest;
            codeType = 3;
            break;
        case 32:
        case 147:
            outFormat = PVRTLPF_ETC1;
            quality = PVRTLCQ_ETCSlow;
            codeType = 3;
            break;
        case 35:
            outFormat = PVRTLPF_ETC2_RGB;
            quality = PVRTLCQ_ETCSlow;
            codeType = 3;
            break;
        case 36:
            outFormat = PVRTLPF_ETC2_RGB_A1;
            quality = PVRTLCQ_ETCSlow;
            codeType = 3;
            break;
        case 37:
            outFormat = PVRTLPF_ETC2_RGBA;
            quality = PVRTLCQ_ETCSlow;
            codeType = 3;
            break;
        case 149:
            outFormat = PVRTGENPIXELID4('r', 'g', 'b', 'a', 8, 8, 8, 8);
            quality = PVRTLCQ_PVRTCFastest;
            codeType = 3;
            break;
        }
        if (codeType) {
            if (codeType == 1) {
                // Line by line
                unsigned char* ptxPtrC = (unsigned char*)ptxDataPtr;
                unsigned char* bitmapPtrC = (unsigned char*)bitmapDataPtr;
                for (int y = 0; y < bitmapHeight; y++) {
                    unsigned char* ptxLinePtrC = ptxPtrC;
                    ptxPtrC += ptxPitch;
                    for (int x = 0; x < bitmapWidth; x++) {
                        encFuncPtr(ptxLinePtrC, bitmapPtrC);
                        ptxLinePtrC += byteCount;
                        bitmapPtrC += 4;
                    }
                }
            }
            else if (codeType == 2) {
                // tile by tile
                // maybe cannot use pitch?
                unsigned char* ptxPtrC = (unsigned char*)ptxDataPtr;
                unsigned char* bitmapPtrC = (unsigned char*)bitmapDataPtr;
                for (int yTile = 0; yTile < bitmapHeight; yTile += 32) {
                    for (int xTile = 0; xTile < bitmapWidth; xTile += 32) {
                        for (int y = 0; y < 32; y++) {
                            for (int x = 0; x < 32; x++) {
                                int thisX = xTile + x;
                                int thisY = yTile + y;
                                if (thisX < bitmapWidth && thisY < bitmapHeight) {
                                    encFuncPtr(ptxPtrC, bitmapPtrC + 4 * (thisY * bitmapWidth + thisX));
                                }
                                ptxPtrC += byteCount;
                            }
                        }
                    }
                }
            }
            else if (codeType == 3) {
                // texture by texture
                Transcode(bitmapDataPtr, ptxDataPtr, bitmapWidth, bitmapHeight, PVRTGENPIXELID4('b', 'g', 'r', 'a', 8, 8, 8, 8), PVRTLCS_Linear, outFormat, PVRTLCS_Linear, quality, false);
            }
        }
        if (ptxFormat >= 147 && ptxFormat <= 149) {
            // alpha with no pitch
            unsigned char* alphaDataPtr = (unsigned char*)ptxDataPtr + PtxHandlerAndroidV3H_GetPtxSizeWithoutAlpha(ptxWidth, ptxHeight, ptxPitch, ptxFormat);
            unsigned char* bitmapPtr = (unsigned char*)bitmapDataPtr;
            int area = bitmapWidth * bitmapHeight;
            for (int i = 0; i < area; i++) {
                *alphaDataPtr = bitmapPtr[3];
                alphaDataPtr++;
                bitmapPtr += 4;
            }
        }
    }

    EResult WIND_API PtxHandlerAndroidV3H_PeekEncodedPtxInfo(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxFormat, int* ptxWidth, int* ptxHeight, int* ptxPitch, int* ptxAlphaSize) {
        switch (ptxFormat)
        {
        case 21:
        case 22:
        case 23:
            *ptxWidth = bitmapWidth;
            *ptxHeight = bitmapHeight;
            *ptxPitch = Align(bitmapWidth, 32) * 2;
            *ptxAlphaSize = 0;
            return Result_OK;
        case 30u:
        case 31u:
        case 148u:
            if (IsPowerOfTwo(bitmapWidth) && IsPowerOfTwo(bitmapHeight))
            {
                *ptxWidth = bitmapWidth;
                *ptxHeight = bitmapHeight;
                switch (ptxFormat)
                {
                case 30:
                    *ptxPitch = bitmapWidth / 2;
                    break;
                case 31:
                    *ptxPitch = bitmapWidth / 4;
                    break;
                default:
                    *ptxPitch = bitmapWidth * 4;
                    break;
                }
                *ptxAlphaSize = 0u;
                return Result_OK;
            }
            break;
        case 32:
        case 35:
        case 36:
        case 37:
        case 38:
        case 39:
        case 147:
            *ptxWidth = bitmapWidth;
            *ptxHeight = bitmapHeight;
            switch (ptxFormat)
            {
            case 32:
            case 35:
            case 38:
                *ptxPitch = bitmapWidth / 2;
                break;
            case 36:
            case 37:
            case 39:
                *ptxPitch = bitmapWidth;
                break;
            default:
                *ptxPitch = bitmapWidth * 4;
                break;
            }
            *ptxAlphaSize = 0u;
            return Result_OK;
        default:
            *ptxWidth = bitmapWidth;
            *ptxHeight = bitmapHeight;
            switch (ptxFormat)
            {
            case 0:
            case 149:
                *ptxPitch = bitmapWidth * 4;
                break;
            default:
                *ptxPitch = bitmapWidth * 2;
                break;
            }
            *ptxAlphaSize = 0;
            return Result_OK;
        }
        *ptxWidth = 0;
        *ptxHeight = 0;
        *ptxPitch = 0;
        *ptxAlphaSize = 0;
        return Result_Error;
    }

    int WIND_API PtxHandleriOSV5H_GetPtxSize(int width, int height, int pitch, int format, int alphaSize) {
        int tex0Size = PtxHandlerAndroidV3H_GetPtxSizeWithoutAlpha(width, height, pitch, format);
        if (format >= 147 && format <= 149) {
            return tex0Size + width * height;
        }
        return tex0Size;
    }

    int WIND_API PtxHandleriOSV5H_GetPtxSizeWithoutAlpha(int width, int height, int pitch, int format) {
        switch (format)
        {
        case 21:
        case 22:
        case 23:
            return pitch * Align(height, 32);
        case 30:
        case 32:
        case 35:
        case 147:
        case 148:
            return width * height / 2;
        case 31:
            return width * height / 4;
        case 36:
        case 37:
        case 39:
            return width * height;
        case 38:
            return width * height * 3 / 4;
        case 149:
            return width * height * 4;
        default:
            return pitch * height;
        }
    }

    void WIND_API PtxHandleriOSV5H_DecodePtx(void* ptxDataPtr, int ptxDataSize, void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize) {
        PVRTuint64 inFormat;
        int codeType = 0;
        void(*decFuncPtr)(unsigned char* wordDataPtr, unsigned char* bitmapDataPtr) = NULL;
        int byteCount = 0;
        switch (ptxFormat) {
        case 0:
            decFuncPtr = &DecodeBGRA8888Word;
            byteCount = 4;
            codeType = 1;
            break;
        case 1:
        default:
            decFuncPtr = &DecodeRGBA4444Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 2:
            decFuncPtr = &DecodeRGB565Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 3:
            decFuncPtr = &DecodeRGBA5551Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 21:
            decFuncPtr = &DecodeRGBA4444Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 22:
            decFuncPtr = &DecodeRGB565Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 23:
            decFuncPtr = &DecodeRGBA5551Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 30:
            inFormat = PVRTLPF_PVRTCI_4bpp_RGBA;
            codeType = 3;
            break;
        case 31:
            inFormat = PVRTLPF_PVRTCI_2bpp_RGBA;
            codeType = 3;
            break;
        case 148:
            inFormat = PVRTLPF_PVRTCI_4bpp_RGB;
            codeType = 3;
            break;
        case 149:
            inFormat = PVRTGENPIXELID4('b', 'g', 'r', 'a', 8, 8, 8, 8);
            codeType = 3;
            break;
        }
        if (codeType) {
            if (codeType == 1) {
                // Line by line
                unsigned char* ptxPtrC = (unsigned char*)ptxDataPtr;
                unsigned char* bitmapPtrC = (unsigned char*)bitmapDataPtr;
                for (int y = 0; y < bitmapHeight; y++) {
                    unsigned char* ptxLinePtrC = ptxPtrC;
                    ptxPtrC += ptxPitch;
                    for (int x = 0; x < bitmapWidth; x++) {
                        decFuncPtr(ptxLinePtrC, bitmapPtrC);
                        ptxLinePtrC += byteCount;
                        bitmapPtrC += 4;
                    }
                }
            }
            else if (codeType == 2) {
                // tile by tile
                // maybe cannot use pitch?
                unsigned char* ptxPtrC = (unsigned char*)ptxDataPtr;
                unsigned char* bitmapPtrC = (unsigned char*)bitmapDataPtr;
                for (int yTile = 0; yTile < bitmapHeight; yTile += 32) {
                    for (int xTile = 0; xTile < bitmapWidth; xTile += 32) {
                        for (int y = 0; y < 32; y++) {
                            for (int x = 0; x < 32; x++) {
                                int thisX = xTile + x;
                                int thisY = yTile + y;
                                if (thisX < bitmapWidth && thisY < bitmapHeight) {
                                    decFuncPtr(ptxPtrC, bitmapPtrC + 4 * (thisY * bitmapWidth + thisX));
                                }
                                ptxPtrC += byteCount;
                            }
                        }
                    }
                }
            }
            else if (codeType == 3) {
                // texture by texture
                Transcode(ptxDataPtr, bitmapDataPtr, bitmapWidth, bitmapHeight, inFormat, PVRTLCS_Linear, PVRTGENPIXELID4('b', 'g', 'r', 'a', 8, 8, 8, 8), PVRTLCS_Linear, PVRTLCQ_PVRTCFastest, false);
            }
        }
        if (ptxFormat >= 147 && ptxFormat <= 149) {
            // alpha with no pitch
            unsigned char* alphaDataPtr = (unsigned char*)ptxDataPtr + PtxHandleriOSV5H_GetPtxSizeWithoutAlpha(ptxWidth, ptxHeight, ptxPitch, ptxFormat);
            unsigned char* bitmapPtr = (unsigned char*)bitmapDataPtr;
            int area = bitmapWidth * bitmapHeight;
            for (int i = 0; i < area; i++) {
                bitmapPtr[3] = *alphaDataPtr;
                alphaDataPtr++;
                bitmapPtr += 4;
            }
        }
    }

    void WIND_API PtxHandleriOSV5H_EncodePtx(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, void* ptxDataPtr, int ptxDataSize, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize) {
        PVRTuint64 outFormat;
        PVRTexLibCompressorQuality quality = PVRTLCQ_PVRTCFastest;
        int codeType = 0;
        void(*encFuncPtr)(unsigned char* wordDataPtr, unsigned char* bitmapDataPtr) = NULL;
        int byteCount = 0;
        switch (ptxFormat) {
        case 0:
            encFuncPtr = &EncodeBGRA8888Word;
            byteCount = 4;
            codeType = 1;
            break;
        case 1:
        default:
            encFuncPtr = &EncodeRGBA4444Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 2:
            encFuncPtr = &EncodeRGB565Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 3:
            encFuncPtr = &EncodeRGBA5551Word;
            byteCount = 2;
            codeType = 1;
            break;
        case 21:
            encFuncPtr = &EncodeRGBA4444Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 22:
            encFuncPtr = &EncodeRGB565Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 23:
            encFuncPtr = &EncodeRGBA5551Word;
            byteCount = 2;
            codeType = 2;
            break;
        case 30:
            outFormat = PVRTLPF_PVRTCI_4bpp_RGBA;
            quality = PVRTLCQ_PVRTCBest;
            codeType = 3;
            break;
        case 31:
            outFormat = PVRTLPF_PVRTCI_2bpp_RGBA;
            quality = PVRTLCQ_PVRTCBest;
            codeType = 3;
            break;
        case 148:
            outFormat = PVRTLPF_PVRTCI_4bpp_RGB;
            quality = PVRTLCQ_PVRTCBest;
            codeType = 3;
            break;
        case 149:
            outFormat = PVRTGENPIXELID4('b', 'g', 'r', 'a', 8, 8, 8, 8);
            quality = PVRTLCQ_PVRTCFastest;
            codeType = 3;
            break;
        }
        if (codeType) {
            if (codeType == 1) {
                // Line by line
                unsigned char* ptxPtrC = (unsigned char*)ptxDataPtr;
                unsigned char* bitmapPtrC = (unsigned char*)bitmapDataPtr;
                for (int y = 0; y < bitmapHeight; y++) {
                    unsigned char* ptxLinePtrC = ptxPtrC;
                    ptxPtrC += ptxPitch;
                    for (int x = 0; x < bitmapWidth; x++) {
                        encFuncPtr(ptxLinePtrC, bitmapPtrC);
                        ptxLinePtrC += byteCount;
                        bitmapPtrC += 4;
                    }
                }
            }
            else if (codeType == 2) {
                // tile by tile
                // maybe cannot use pitch?
                unsigned char* ptxPtrC = (unsigned char*)ptxDataPtr;
                unsigned char* bitmapPtrC = (unsigned char*)bitmapDataPtr;
                for (int yTile = 0; yTile < bitmapHeight; yTile += 32) {
                    for (int xTile = 0; xTile < bitmapWidth; xTile += 32) {
                        for (int y = 0; y < 32; y++) {
                            for (int x = 0; x < 32; x++) {
                                int thisX = xTile + x;
                                int thisY = yTile + y;
                                if (thisX < bitmapWidth && thisY < bitmapHeight) {
                                    encFuncPtr(ptxPtrC, bitmapPtrC + 4 * (thisY * bitmapWidth + thisX));
                                }
                                ptxPtrC += byteCount;
                            }
                        }
                    }
                }
            }
            else if (codeType == 3) {
                // texture by texture
                Transcode(bitmapDataPtr, ptxDataPtr, bitmapWidth, bitmapHeight, PVRTGENPIXELID4('b', 'g', 'r', 'a', 8, 8, 8, 8), PVRTLCS_Linear, outFormat, PVRTLCS_Linear, quality, false);
            }
        }
        if (ptxFormat >= 147 && ptxFormat <= 149) {
            // alpha with no pitch
            unsigned char* alphaDataPtr = (unsigned char*)ptxDataPtr + PtxHandleriOSV5H_GetPtxSizeWithoutAlpha(ptxWidth, ptxHeight, ptxPitch, ptxFormat);
            unsigned char* bitmapPtr = (unsigned char*)bitmapDataPtr;
            int area = bitmapWidth * bitmapHeight;
            for (int i = 0; i < area; i++) {
                *alphaDataPtr = bitmapPtr[3];
                alphaDataPtr++;
                bitmapPtr += 4;
            }
        }
    }

    EResult WIND_API PtxHandleriOSV5H_PeekEncodedPtxInfo(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxFormat, int* ptxWidth, int* ptxHeight, int* ptxPitch, int* ptxAlphaSize) {
        switch (ptxFormat)
        {
        case 21:
        case 22:
        case 23:
            *ptxWidth = bitmapWidth;
            *ptxHeight = bitmapHeight;
            *ptxPitch = Align(bitmapWidth, 32) * 2;
            *ptxAlphaSize = 0;
            return Result_OK;
        case 30:
        case 31:
        case 148:
            if (IsPowerOfTwo(bitmapWidth) && IsPowerOfTwo(bitmapHeight))
            {
                *ptxWidth = bitmapWidth;
                *ptxHeight = bitmapHeight;
                switch (ptxFormat)
                {
                case 30:
                    *ptxPitch = bitmapWidth / 2;
                    break;
                case 31:
                    *ptxPitch = bitmapWidth / 4;
                    break;
                default:
                    *ptxPitch = bitmapWidth * 4;
                    break;
                }
                *ptxAlphaSize = 0u;
                return Result_OK;
            }
            break;
        default:
            *ptxWidth = bitmapWidth;
            *ptxHeight = bitmapHeight;
            switch (ptxFormat)
            {
            case 0:
            case 149:
                *ptxPitch = bitmapWidth * 4;
                break;
            default:
                *ptxPitch = bitmapWidth * 2;
                break;
            }
            *ptxAlphaSize = 0;
            return Result_OK;
        }
        *ptxWidth = 0;
        *ptxHeight = 0;
        *ptxPitch = 0;
        *ptxAlphaSize = 0;
        return Result_Error;
    }
}
