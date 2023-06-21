#include "PtxHandlerForPVZFree.h"

inline void GetAstcTileLength(const int format, int* width, int* height)
{
    switch (format)
    {
    case 46:
        *width = 4;
        *height = 4;
        break;
    case 47:
        *width = 5;
        *height = 4;
        break;
    case 48:
        *width = 5;
        *height = 5;
        break;
    case 49:
        *width = 6;
        *height = 5;
        break;
    case 50:
        *width = 6;
        *height = 6;
        break;
    case 51:
        *width = 8;
        *height = 5;
        break;
    case 52:
        *width = 8;
        *height = 6;
        break;
    case 53:
        *width = 8;
        *height = 8;
        break;
    case 54:
        *width = 10;
        *height = 5;
        break;
    case 55:
        *width = 10;
        *height = 6;
        break;
    case 56:
        *width = 10;
        *height = 8;
        break;
    case 57:
        *width = 10;
        *height = 10;
        break;
    case 58:
        *width = 12;
        *height = 10;
        break;
    case 59:
        *width = 12;
        *height = 12;
        break;
    }
}

inline int ComputeAstcSize(int width, int height, int xSize, int ySize)
{
    return ((width + xSize - 1) / xSize) * ((height + ySize - 1) / ySize) * 16;
}

inline int ComputeAstcSize(int width, int height, int format) {
    int xSize = 0, ySize = 0;
    GetAstcTileLength(format, &xSize, &ySize);
    return ComputeAstcSize(width, height, xSize, ySize);
}

inline PVRTuint64 GetAstcPVRTexLibFormat(const int format) {
    switch (format)
    {
    case 46:
        return PVRTLPF_ASTC_4x4;
    case 47:
        return PVRTLPF_ASTC_5x4;
    case 48:
        return PVRTLPF_ASTC_5x5;
    case 49:
        return PVRTLPF_ASTC_6x5;
    case 50:
        return PVRTLPF_ASTC_6x6;
    case 51:
        return PVRTLPF_ASTC_8x5;
    case 52:
        return PVRTLPF_ASTC_8x6;
    case 53:
        return PVRTLPF_ASTC_8x8;
    case 54:
        return PVRTLPF_ASTC_10x5;
    case 55:
        return PVRTLPF_ASTC_10x6;
    case 56:
        return PVRTLPF_ASTC_10x8;
    case 57:
        return PVRTLPF_ASTC_10x10;
    case 58:
        return PVRTLPF_ASTC_12x10;
    case 59:
        return PVRTLPF_ASTC_12x12;
    }
    return 0;
}

int Transcode2(void* inData, void* outData, PVRTuint32 width, PVRTuint32 height, PVRTuint64 inFormat, PVRTexLibColourSpace inColorSpace, PVRTuint64 outFormat, PVRTexLibColourSpace outColorSpace, PVRTexLibCompressorQuality quality, bool dither) {
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

void DecodeBGRA8888Word2(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    bitmapDataPtrPtr[0] = wordDataPtrPtr[0]; // B
    bitmapDataPtrPtr[1] = wordDataPtrPtr[1]; // G
    bitmapDataPtrPtr[2] = wordDataPtrPtr[2]; // R
    bitmapDataPtrPtr[3] = wordDataPtrPtr[3]; // A
}

void DecodeRGBA8888Word2(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    bitmapDataPtrPtr[0] = wordDataPtrPtr[2]; // B
    bitmapDataPtrPtr[1] = wordDataPtrPtr[1]; // G
    bitmapDataPtrPtr[2] = wordDataPtrPtr[0]; // R
    bitmapDataPtrPtr[3] = wordDataPtrPtr[3]; // A
}

void DecodeRGBA4444Word2(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
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

void DecodeRGB565Word2(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
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

void DecodeRGBA5551Word2(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
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

void EncodeBGRA8888Word2(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    wordDataPtrPtr[0] = bitmapDataPtrPtr[0]; // B
    wordDataPtrPtr[1] = bitmapDataPtrPtr[1]; // G
    wordDataPtrPtr[2] = bitmapDataPtrPtr[2]; // R
    wordDataPtrPtr[3] = bitmapDataPtrPtr[3]; // A
}

void EncodeRGBA8888Word2(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    wordDataPtrPtr[0] = bitmapDataPtrPtr[2]; // R
    wordDataPtrPtr[1] = bitmapDataPtrPtr[1]; // G
    wordDataPtrPtr[2] = bitmapDataPtrPtr[0]; // B
    wordDataPtrPtr[3] = bitmapDataPtrPtr[3]; // A
}

void EncodeRGBA4444Word2(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    unsigned short word = 0;
    word |= (bitmapDataPtrPtr[2] >> 4) << 12; // R
    word |= (bitmapDataPtrPtr[1] >> 4) << 8; // G
    word |= (bitmapDataPtrPtr[0] >> 4) << 4; // B
    word |= (bitmapDataPtrPtr[3] >> 4) << 0; // A
    wordDataPtrPtr[0] = (unsigned char)word;
    wordDataPtrPtr[1] = (unsigned char)(word >> 8);
}

void EncodeRGB565Word2(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    unsigned short word = 0;
    word |= (bitmapDataPtrPtr[2] >> 3) << 11; // R
    word |= (bitmapDataPtrPtr[1] >> 2) << 5; // G
    word |= (bitmapDataPtrPtr[0] >> 3) << 0; // B
    wordDataPtrPtr[0] = (unsigned char)word;
    wordDataPtrPtr[1] = (unsigned char)(word >> 8);
}

void EncodeRGBA5551Word2(unsigned char* wordDataPtrPtr, unsigned char* bitmapDataPtrPtr) {
    unsigned short word = 0;
    word |= (bitmapDataPtrPtr[2] >> 3) << 11; // R
    word |= (bitmapDataPtrPtr[1] >> 3) << 6; // G
    word |= (bitmapDataPtrPtr[0] >> 3) << 1; // B
    word |= (bitmapDataPtrPtr[3] >> 7) << 0; // A
    wordDataPtrPtr[0] = (unsigned char)word;
    wordDataPtrPtr[1] = (unsigned char)(word >> 8);
}

extern "C" {
    int WIND_API PtxHandlerForPVZFree_GetPtxSize(int width, int height, int pitch, int format, int alphaSize) {
        return PtxHandlerForPVZFree_GetPtxSizeWithoutAlpha(width, height, pitch, format);
    }

    int WIND_API PtxHandlerForPVZFree_GetPtxSizeWithoutAlpha(int width, int height, int pitch, int format) {
        switch (format)
        {
        case 20:
        case 21:
        case 22:
        case 23:
            return pitch * Align(height, 32);
        case 40:
        case 41:
        case 42:
        case 44:
            return Align(width, 4) * Align(height, 4) / 2;
        case 43:
        case 45:
            return Align(width, 4) * Align(height, 4);
        case 46:
        case 47:
        case 48:
        case 49:
        case 50:
        case 51:
        case 52:
        case 53:
        case 54:
        case 55:
        case 56:
        case 57:
        case 58:
        case 59:
            return ComputeAstcSize(width, height, format);
        }
        return height * pitch;
    }

    void WIND_API PtxHandlerForPVZFree_DecodePtx(void* ptxDataPtr, int ptxDataSize, void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize) {
        PVRTuint64 inFormat;
        int codeType = 0;
        void(*decFuncPtr)(unsigned char* wordDataPtr, unsigned char* bitmapDataPtr) = NULL;
        int byteCount = 0;
        switch (ptxFormat) {
        case 0:
            decFuncPtr = &DecodeRGBA8888Word2;
            byteCount = 4;
            codeType = 1;
            break;
        case 1:
        default:
            decFuncPtr = &DecodeRGBA4444Word2;
            byteCount = 2;
            codeType = 1;
            break;
        case 2:
            decFuncPtr = &DecodeRGB565Word2;
            byteCount = 2;
            codeType = 1;
            break;
        case 3:
            decFuncPtr = &DecodeRGBA5551Word2;
            byteCount = 2;
            codeType = 1;
            break;
        case 20:
            decFuncPtr = &DecodeRGBA8888Word2;
            byteCount = 4;
            codeType = 2;
            break;
        case 21:
            decFuncPtr = &DecodeRGBA4444Word2;
            byteCount = 2;
            codeType = 2;
            break;
        case 22:
            decFuncPtr = &DecodeRGB565Word2;
            byteCount = 2;
            codeType = 2;
            break;
        case 23:
            decFuncPtr = &DecodeRGBA5551Word2;
            byteCount = 2;
            codeType = 2;
            break;
        case 40:
            inFormat = PVRTLPF_ETC1;
            codeType = 3;
            break;
        case 41:
            inFormat = PVRTLPF_ETC2_RGB;
            codeType = 3;
            break;
        case 42:
            inFormat = PVRTLPF_ETC2_RGB_A1;
            codeType = 3;
            break;
        case 43:
            inFormat = PVRTLPF_ETC2_RGBA;
            codeType = 3;
            break;
        case 44:
            inFormat = PVRTLPF_EAC_R11;
            codeType = 3;
            break;
        case 45:
            inFormat = PVRTLPF_EAC_RG11;
            codeType = 3;
            break;
        case 46:
        case 47:
        case 48:
        case 49:
        case 50:
        case 51:
        case 52:
        case 53:
        case 54:
        case 55:
        case 56:
        case 57:
        case 58:
        case 59:
            inFormat = GetAstcPVRTexLibFormat(ptxFormat);
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
                Transcode2(ptxDataPtr, bitmapDataPtr, bitmapWidth, bitmapHeight, inFormat, PVRTLCS_Linear, PVRTGENPIXELID4('b', 'g', 'r', 'a', 8, 8, 8, 8), PVRTLCS_Linear, PVRTLCQ_PVRTCFastest, false);
            }
        }
    }

    void WIND_API PtxHandlerForPVZFree_EncodePtx(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, void* ptxDataPtr, int ptxDataSize, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize) {
        PVRTuint64 outFormat;
        PVRTexLibCompressorQuality quality = PVRTLCQ_PVRTCFastest;
        int codeType = 0;
        void(*encFuncPtr)(unsigned char* wordDataPtr, unsigned char* bitmapDataPtr) = NULL;
        int byteCount = 0;
        switch (ptxFormat) {
        case 0:
            encFuncPtr = &EncodeRGBA8888Word2;
            byteCount = 4;
            codeType = 1;
            break;
        case 1:
        default:
            encFuncPtr = &EncodeRGBA4444Word2;
            byteCount = 2;
            codeType = 1;
            break;
        case 2:
            encFuncPtr = &EncodeRGB565Word2;
            byteCount = 2;
            codeType = 1;
            break;
        case 3:
            encFuncPtr = &EncodeRGBA5551Word2;
            byteCount = 2;
            codeType = 1;
            break;
        case 20:
            encFuncPtr = &EncodeRGBA8888Word2;
            byteCount = 4;
            codeType = 2;
            break;
        case 21:
            encFuncPtr = &EncodeRGBA4444Word2;
            byteCount = 2;
            codeType = 2;
            break;
        case 22:
            encFuncPtr = &EncodeRGB565Word2;
            byteCount = 2;
            codeType = 2;
            break;
        case 23:
            encFuncPtr = &EncodeRGBA5551Word2;
            byteCount = 2;
            codeType = 2;
            break;
        case 40:
            outFormat = PVRTLPF_ETC1;
            quality = PVRTLCQ_ETCFast;
            codeType = 3;
            break;
        case 41:
            outFormat = PVRTLPF_ETC2_RGB;
            quality = PVRTLCQ_ETCFast;
            codeType = 3;
            break;
        case 42:
            outFormat = PVRTLPF_ETC2_RGB_A1;
            quality = PVRTLCQ_ETCFast;
            codeType = 3;
            break;
        case 43:
            outFormat = PVRTLPF_ETC2_RGBA;
            quality = PVRTLCQ_ETCFast;
            codeType = 3;
            break;
        case 44:
            outFormat = PVRTLPF_EAC_R11;
            quality = PVRTLCQ_ETCFast;
            codeType = 3;
            break;
        case 45:
            outFormat = PVRTLPF_EAC_RG11;
            quality = PVRTLCQ_ETCFast;
            codeType = 3;
            break;
        case 46:
        case 47:
        case 48:
        case 49:
        case 50:
        case 51:
        case 52:
        case 53:
        case 54:
        case 55:
        case 56:
        case 57:
        case 58:
        case 59:
            outFormat = GetAstcPVRTexLibFormat(ptxFormat);
            quality = PVRTLCQ_ASTCVeryFast;
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
                Transcode2(bitmapDataPtr, ptxDataPtr, bitmapWidth, bitmapHeight, PVRTGENPIXELID4('b', 'g', 'r', 'a', 8, 8, 8, 8), PVRTLCS_Linear, outFormat, PVRTLCS_Linear, quality, false);
            }
        }
    }

    EResult WIND_API PtxHandlerForPVZFree_PeekEncodedPtxInfo(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxFormat, int* ptxWidth, int* ptxHeight, int* ptxPitch, int* ptxAlphaSize) {
        switch (ptxFormat)
        {
        case 20:
            *ptxWidth = bitmapWidth;
            *ptxHeight = bitmapHeight;
            *ptxPitch = Align(bitmapWidth, 32) * 4;
            *ptxAlphaSize = 0;
            return Result_OK;
        case 21:
        case 22:
        case 23:
            *ptxWidth = bitmapWidth;
            *ptxHeight = bitmapHeight;
            *ptxPitch = Align(bitmapWidth, 32) * 2;
            *ptxAlphaSize = 0;
            return Result_OK;
        default:
            *ptxWidth = bitmapWidth;
            *ptxHeight = bitmapHeight;
            *ptxPitch = bitmapWidth * (ptxFormat ? 2 : 4);
            *ptxAlphaSize = 0;
            return Result_OK;
        }
    }
}
