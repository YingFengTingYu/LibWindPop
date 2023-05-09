﻿namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Gcm
{
    public enum CellGcmEnum
    {
        //	Enable
        CELL_GCM_FALSE = (0),
        CELL_GCM_TRUE = (1),

        // Location
        CELL_GCM_LOCATION_LOCAL = (0),
        CELL_GCM_LOCATION_MAIN = (1),


        // SetSurface
        CELL_GCM_SURFACE_X1R5G5B5_Z1R5G5B5 = (1),
        CELL_GCM_SURFACE_X1R5G5B5_O1R5G5B5 = (2),
        CELL_GCM_SURFACE_R5G6B5 = (3),
        CELL_GCM_SURFACE_X8R8G8B8_Z8R8G8B8 = (4),
        CELL_GCM_SURFACE_X8R8G8B8_O8R8G8B8 = (5),
        CELL_GCM_SURFACE_A8R8G8B8 = (8),
        CELL_GCM_SURFACE_B8 = (9),
        CELL_GCM_SURFACE_G8B8 = (10),
        CELL_GCM_SURFACE_F_W16Z16Y16X16 = (11),
        CELL_GCM_SURFACE_F_W32Z32Y32X32 = (12),
        CELL_GCM_SURFACE_F_X32 = (13),
        CELL_GCM_SURFACE_X8B8G8R8_Z8B8G8R8 = (14),
        CELL_GCM_SURFACE_X8B8G8R8_O8B8G8R8 = (15),
        CELL_GCM_SURFACE_A8B8G8R8 = (16),

        CELL_GCM_SURFACE_Z16 = (1),
        CELL_GCM_SURFACE_Z24S8 = (2),

        CELL_GCM_SURFACE_PITCH = (1),
        CELL_GCM_SURFACE_SWIZZLE = (2),

        CELL_GCM_SURFACE_CENTER_1 = (0),
        CELL_GCM_SURFACE_DIAGONAL_CENTERED_2 = (3),
        CELL_GCM_SURFACE_SQUARE_CENTERED_4 = (4),
        CELL_GCM_SURFACE_SQUARE_ROTATED_4 = (5),

        CELL_GCM_SURFACE_TARGET_NONE = (0),
        CELL_GCM_SURFACE_TARGET_0 = (1),
        CELL_GCM_SURFACE_TARGET_1 = (2),
        CELL_GCM_SURFACE_TARGET_MRT1 = (0x13),
        CELL_GCM_SURFACE_TARGET_MRT2 = (0x17),
        CELL_GCM_SURFACE_TARGET_MRT3 = (0x1f),

        // SetSurfaceWindow
        CELL_GCM_WINDOW_ORIGIN_TOP = (0),
        CELL_GCM_WINDOW_ORIGIN_BOTTOM = (1),

        CELL_GCM_WINDOW_PIXEL_CENTER_HALF = (0),
        CELL_GCM_WINDOW_PIXEL_CENTER_INTEGER = (1),

        // SetClearSurface
        CELL_GCM_CLEAR_Z = (1 << 0),
        CELL_GCM_CLEAR_S = (1 << 1),
        CELL_GCM_CLEAR_R = (1 << 4),
        CELL_GCM_CLEAR_G = (1 << 5),
        CELL_GCM_CLEAR_B = (1 << 6),
        CELL_GCM_CLEAR_A = (1 << 7),
        CELL_GCM_CLEAR_M = (0xf3),

        // SetVertexDataArray
        CELL_GCM_VERTEX_S1 = (1),
        CELL_GCM_VERTEX_F = (2),
        CELL_GCM_VERTEX_SF = (3),
        CELL_GCM_VERTEX_UB = (4),
        CELL_GCM_VERTEX_S32K = (5),
        CELL_GCM_VERTEX_CMP = (6),
        CELL_GCM_VERTEX_UB256 = (7),

        CELL_GCM_VERTEX_S16_NR = (1),
        CELL_GCM_VERTEX_F32 = (2),
        CELL_GCM_VERTEX_F16 = (3),
        CELL_GCM_VERTEX_U8_NR = (4),
        CELL_GCM_VERTEX_S16_UN = (5),
        CELL_GCM_VERTEX_S11_11_10_NR = (6),
        CELL_GCM_VERTEX_U8_UN = (7),

        // SetTexture
        CELL_GCM_TEXTURE_B8 = (0x81),
        CELL_GCM_TEXTURE_A1R5G5B5 = (0x82),
        CELL_GCM_TEXTURE_A4R4G4B4 = (0x83),
        CELL_GCM_TEXTURE_R5G6B5 = (0x84),
        CELL_GCM_TEXTURE_A8R8G8B8 = (0x85),
        CELL_GCM_TEXTURE_COMPRESSED_DXT1 = (0x86),
        CELL_GCM_TEXTURE_COMPRESSED_DXT23 = (0x87),
        CELL_GCM_TEXTURE_COMPRESSED_DXT45 = (0x88),
        CELL_GCM_TEXTURE_G8B8 = (0x8B),
        CELL_GCM_TEXTURE_R6G5B5 = (0x8F),
        CELL_GCM_TEXTURE_DEPTH24_D8 = (0x90),
        CELL_GCM_TEXTURE_DEPTH24_D8_FLOAT = (0x91),
        CELL_GCM_TEXTURE_DEPTH16 = (0x92),
        CELL_GCM_TEXTURE_DEPTH16_FLOAT = (0x93),
        CELL_GCM_TEXTURE_X16 = (0x94),
        CELL_GCM_TEXTURE_Y16_X16 = (0x95),
        CELL_GCM_TEXTURE_R5G5B5A1 = (0x97),
        CELL_GCM_TEXTURE_COMPRESSED_HILO8 = (0x98),
        CELL_GCM_TEXTURE_COMPRESSED_HILO_S8 = (0x99),
        CELL_GCM_TEXTURE_W16_Z16_Y16_X16_FLOAT = (0x9A),
        CELL_GCM_TEXTURE_W32_Z32_Y32_X32_FLOAT = (0x9B),
        CELL_GCM_TEXTURE_X32_FLOAT = (0x9C),
        CELL_GCM_TEXTURE_D1R5G5B5 = (0x9D),
        CELL_GCM_TEXTURE_D8R8G8B8 = (0x9E),
        CELL_GCM_TEXTURE_Y16_X16_FLOAT = (0x9F),
        CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8 = (0xAD),
        CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8 = (0xAE),

        CELL_GCM_TEXTURE_SZ = (0x00),
        CELL_GCM_TEXTURE_LN = (0x20),
        CELL_GCM_TEXTURE_NR = (0x00),
        CELL_GCM_TEXTURE_UN = (0x40),

        CELL_GCM_TEXTURE_DIMENSION_1 = (1),
        CELL_GCM_TEXTURE_DIMENSION_2 = (2),
        CELL_GCM_TEXTURE_DIMENSION_3 = (3),

        CELL_GCM_TEXTURE_REMAP_ORDER_XYXY = (0),
        CELL_GCM_TEXTURE_REMAP_ORDER_XXXY = (1),
        CELL_GCM_TEXTURE_REMAP_FROM_A = (0),
        CELL_GCM_TEXTURE_REMAP_FROM_R = (1),
        CELL_GCM_TEXTURE_REMAP_FROM_G = (2),
        CELL_GCM_TEXTURE_REMAP_FROM_B = (3),
        CELL_GCM_TEXTURE_REMAP_ZERO = (0),
        CELL_GCM_TEXTURE_REMAP_ONE = (1),
        CELL_GCM_TEXTURE_REMAP_REMAP = (2),

        CELL_GCM_TEXTURE_BORDER_TEXTURE = (0),
        CELL_GCM_TEXTURE_BORDER_COLOR = (1),

        // SetTextureFilter
        CELL_GCM_TEXTURE_NEAREST = (1),
        CELL_GCM_TEXTURE_LINEAR = (2),
        CELL_GCM_TEXTURE_NEAREST_NEAREST = (3),
        CELL_GCM_TEXTURE_LINEAR_NEAREST = (4),
        CELL_GCM_TEXTURE_NEAREST_LINEAR = (5),
        CELL_GCM_TEXTURE_LINEAR_LINEAR = (6),
        CELL_GCM_TEXTURE_CONVOLUTION_MIN = (7),
        CELL_GCM_TEXTURE_CONVOLUTION_MAG = (4),
        CELL_GCM_TEXTURE_CONVOLUTION_QUINCUNX = (1),
        CELL_GCM_TEXTURE_CONVOLUTION_GAUSSIAN = (2),
        CELL_GCM_TEXTURE_CONVOLUTION_QUINCUNX_ALT = (3),

        // SetTextureAddress
        CELL_GCM_TEXTURE_WRAP = (1),
        CELL_GCM_TEXTURE_MIRROR = (2),
        CELL_GCM_TEXTURE_CLAMP_TO_EDGE = (3),
        CELL_GCM_TEXTURE_BORDER = (4),
        CELL_GCM_TEXTURE_CLAMP = (5),
        CELL_GCM_TEXTURE_MIRROR_ONCE_CLAMP_TO_EDGE = (6),
        CELL_GCM_TEXTURE_MIRROR_ONCE_BORDER = (7),
        CELL_GCM_TEXTURE_MIRROR_ONCE_CLAMP = (8),

        CELL_GCM_TEXTURE_UNSIGNED_REMAP_NORMAL = (0),
        CELL_GCM_TEXTURE_UNSIGNED_REMAP_BIASED = (1),

        CELL_GCM_TEXTURE_SIGNED_REMAP_NORMAL = (0x0),
        CELL_GCM_TEXTURE_SIGNED_REMAP_CLAMPED = (0x3),

        CELL_GCM_TEXTURE_ZFUNC_NEVER = (0),
        CELL_GCM_TEXTURE_ZFUNC_LESS = (1),
        CELL_GCM_TEXTURE_ZFUNC_EQUAL = (2),
        CELL_GCM_TEXTURE_ZFUNC_LEQUAL = (3),
        CELL_GCM_TEXTURE_ZFUNC_GREATER = (4),
        CELL_GCM_TEXTURE_ZFUNC_NOTEQUAL = (5),
        CELL_GCM_TEXTURE_ZFUNC_GEQUAL = (6),
        CELL_GCM_TEXTURE_ZFUNC_ALWAYS = (7),

        CELL_GCM_TEXTURE_GAMMA_R = (1 << 0),
        CELL_GCM_TEXTURE_GAMMA_G = (1 << 1),
        CELL_GCM_TEXTURE_GAMMA_B = (1 << 2),
        CELL_GCM_TEXTURE_GAMMA_A = (1 << 3),

        // SetAnisoSpread
        CELL_GCM_TEXTURE_ANISO_SPREAD_0_50_TEXEL = (0x0),
        CELL_GCM_TEXTURE_ANISO_SPREAD_1_00_TEXEL = (0x1),
        CELL_GCM_TEXTURE_ANISO_SPREAD_1_125_TEXEL = (0x2),
        CELL_GCM_TEXTURE_ANISO_SPREAD_1_25_TEXEL = (0x3),
        CELL_GCM_TEXTURE_ANISO_SPREAD_1_375_TEXEL = (0x4),
        CELL_GCM_TEXTURE_ANISO_SPREAD_1_50_TEXEL = (0x5),
        CELL_GCM_TEXTURE_ANISO_SPREAD_1_75_TEXEL = (0x6),
        CELL_GCM_TEXTURE_ANISO_SPREAD_2_00_TEXEL = (0x7),

        // SetCylindricalWrap
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX0_U = (1 << 0),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX0_V = (1 << 1),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX0_P = (1 << 2),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX0_Q = (1 << 3),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX1_U = (1 << 4),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX1_V = (1 << 5),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX1_P = (1 << 6),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX1_Q = (1 << 7),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX2_U = (1 << 8),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX2_V = (1 << 9),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX2_P = (1 << 10),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX2_Q = (1 << 11),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX3_U = (1 << 12),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX3_V = (1 << 13),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX3_P = (1 << 14),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX3_Q = (1 << 15),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX4_U = (1 << 16),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX4_V = (1 << 17),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX4_P = (1 << 18),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX4_Q = (1 << 19),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX5_U = (1 << 20),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX5_V = (1 << 21),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX5_P = (1 << 22),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX5_Q = (1 << 23),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX6_U = (1 << 24),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX6_V = (1 << 25),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX6_P = (1 << 26),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX6_Q = (1 << 27),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX7_U = (1 << 28),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX7_V = (1 << 29),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX7_P = (1 << 30),
        CELL_GCM_TEXTURE_CYLINDRICAL_WRAP_ENABLE_TEX7_Q = (1 << 31),

        // SetTextureControl
        CELL_GCM_TEXTURE_MAX_ANISO_1 = (0),
        CELL_GCM_TEXTURE_MAX_ANISO_2 = (1),
        CELL_GCM_TEXTURE_MAX_ANISO_4 = (2),
        CELL_GCM_TEXTURE_MAX_ANISO_6 = (3),
        CELL_GCM_TEXTURE_MAX_ANISO_8 = (4),
        CELL_GCM_TEXTURE_MAX_ANISO_10 = (5),
        CELL_GCM_TEXTURE_MAX_ANISO_12 = (6),
        CELL_GCM_TEXTURE_MAX_ANISO_16 = (7),

        // SetDrawArrays, SetDrawIndexArray
        CELL_GCM_PRIMITIVE_POINTS = (1),
        CELL_GCM_PRIMITIVE_LINES = (2),
        CELL_GCM_PRIMITIVE_LINE_LOOP = (3),
        CELL_GCM_PRIMITIVE_LINE_STRIP = (4),
        CELL_GCM_PRIMITIVE_TRIANGLES = (5),
        CELL_GCM_PRIMITIVE_TRIANGLE_STRIP = (6),
        CELL_GCM_PRIMITIVE_TRIANGLE_FAN = (7),
        CELL_GCM_PRIMITIVE_QUADS = (8),
        CELL_GCM_PRIMITIVE_QUAD_STRIP = (9),
        CELL_GCM_PRIMITIVE_POLYGON = (10),

        // SetColorMask
        CELL_GCM_COLOR_MASK_B = (1 << 0),
        CELL_GCM_COLOR_MASK_G = (1 << 8),
        CELL_GCM_COLOR_MASK_R = (1 << 16),
        CELL_GCM_COLOR_MASK_A = (1 << 24),

        // SetColorMaskMrt
        CELL_GCM_COLOR_MASK_MRT1_A = (1 << 4),
        CELL_GCM_COLOR_MASK_MRT1_R = (1 << 5),
        CELL_GCM_COLOR_MASK_MRT1_G = (1 << 6),
        CELL_GCM_COLOR_MASK_MRT1_B = (1 << 7),
        CELL_GCM_COLOR_MASK_MRT2_A = (1 << 8),
        CELL_GCM_COLOR_MASK_MRT2_R = (1 << 9),
        CELL_GCM_COLOR_MASK_MRT2_G = (1 << 10),
        CELL_GCM_COLOR_MASK_MRT2_B = (1 << 11),
        CELL_GCM_COLOR_MASK_MRT3_A = (1 << 12),
        CELL_GCM_COLOR_MASK_MRT3_R = (1 << 13),
        CELL_GCM_COLOR_MASK_MRT3_G = (1 << 14),
        CELL_GCM_COLOR_MASK_MRT3_B = (1 << 15),

        // SetAlphaFunc, DepthFunc, StencilFunc
        CELL_GCM_NEVER = (0x0200),
        CELL_GCM_LESS = (0x0201),
        CELL_GCM_EQUAL = (0x0202),
        CELL_GCM_LEQUAL = (0x0203),
        CELL_GCM_GREATER = (0x0204),
        CELL_GCM_NOTEQUAL = (0x0205),
        CELL_GCM_GEQUAL = (0x0206),
        CELL_GCM_ALWAYS = (0x0207),

        // SetBlendFunc
        CELL_GCM_ZERO = (0),
        CELL_GCM_ONE = (1),
        CELL_GCM_SRC_COLOR = (0x0300),
        CELL_GCM_ONE_MINUS_SRC_COLOR = (0x0301),
        CELL_GCM_SRC_ALPHA = (0x0302),
        CELL_GCM_ONE_MINUS_SRC_ALPHA = (0x0303),
        CELL_GCM_DST_ALPHA = (0x0304),
        CELL_GCM_ONE_MINUS_DST_ALPHA = (0x0305),
        CELL_GCM_DST_COLOR = (0x0306),
        CELL_GCM_ONE_MINUS_DST_COLOR = (0x0307),
        CELL_GCM_SRC_ALPHA_SATURATE = (0x0308),
        CELL_GCM_CONSTANT_COLOR = (0x8001),
        CELL_GCM_ONE_MINUS_CONSTANT_COLOR = (0x8002),
        CELL_GCM_CONSTANT_ALPHA = (0x8003),
        CELL_GCM_ONE_MINUS_CONSTANT_ALPHA = (0x8004),

        // SetBlendEquation
        CELL_GCM_FUNC_ADD = (0x8006),
        CELL_GCM_MIN = (0x8007),
        CELL_GCM_MAX = (0x8008),
        CELL_GCM_FUNC_SUBTRACT = (0x800A),
        CELL_GCM_FUNC_REVERSE_SUBTRACT = (0x800B),
        CELL_GCM_FUNC_REVERSE_SUBTRACT_SIGNED = (0x0000F005),
        CELL_GCM_FUNC_ADD_SIGNED = (0x0000F006),
        CELL_GCM_FUNC_REVERSE_ADD_SIGNED = (0x0000F007),

        // SetCullFace
        CELL_GCM_FRONT = (0x0404),
        CELL_GCM_BACK = (0x0405),
        CELL_GCM_FRONT_AND_BACK = (0x0408),

        // SetShadeMode
        CELL_GCM_FLAT = (0x1D00),
        CELL_GCM_SMOOTH = (0x1D01),

        // SetFrontFace
        CELL_GCM_CW = (0x0900),
        CELL_GCM_CCW = (0x0901),

        // SetLogicOp
        CELL_GCM_CLEAR = (0x1500),
        CELL_GCM_AND = (0x1501),
        CELL_GCM_AND_REVERSE = (0x1502),
        CELL_GCM_COPY = (0x1503),
        CELL_GCM_AND_INVERTED = (0x1504),
        CELL_GCM_NOOP = (0x1505),
        CELL_GCM_XOR = (0x1506),
        CELL_GCM_OR = (0x1507),
        CELL_GCM_NOR = (0x1508),
        CELL_GCM_EQUIV = (0x1509),
        CELL_GCM_INVERT = (0x150A),
        CELL_GCM_OR_REVERSE = (0x150B),
        CELL_GCM_COPY_INVERTED = (0x150C),
        CELL_GCM_OR_INVERTED = (0x150D),
        CELL_GCM_NAND = (0x150E),
        CELL_GCM_SET = (0x150F),

        // SetStencilOp
        CELL_GCM_KEEP = (0x1E00),
        CELL_GCM_REPLACE = (0x1E01),
        CELL_GCM_INCR = (0x1E02),
        CELL_GCM_DECR = (0x1E03),
        CELL_GCM_INCR_WRAP = (0x8507),
        CELL_GCM_DECR_WRAP = (0x8508),

        // SetDrawIndexArray
        CELL_GCM_DRAW_INDEX_ARRAY_TYPE_32 = (0),
        CELL_GCM_DRAW_INDEX_ARRAY_TYPE_16 = (1),

        // SetTransfer
        CELL_GCM_TRANSFER_LOCAL_TO_LOCAL = (0),
        CELL_GCM_TRANSFER_MAIN_TO_LOCAL = (1),
        CELL_GCM_TRANSFER_LOCAL_TO_MAIN = (2),
        CELL_GCM_TRANSFER_MAIN_TO_MAIN = (3),

        // SetInvalidateTextureCache
        CELL_GCM_INVALIDATE_TEXTURE = (1),
        CELL_GCM_INVALIDATE_VERTEX_TEXTURE = (2),

        // SetFrequencyDividerOperation
        CELL_GCM_FREQUENCY_MODULO = (1),
        CELL_GCM_FREQUENCY_DIVIDE = (0),

        // SetTile, SetZCull
        CELL_GCM_COMPMODE_DISABLED = (0),
        CELL_GCM_COMPMODE_C32_2X1 = (7),
        CELL_GCM_COMPMODE_C32_2X2 = (8),
        CELL_GCM_COMPMODE_Z32_SEPSTENCIL = (9),
        CELL_GCM_COMPMODE_Z32_SEPSTENCIL_REG = (10),
        CELL_GCM_COMPMODE_Z32_SEPSTENCIL_REGULAR = (10),
        CELL_GCM_COMPMODE_Z32_SEPSTENCIL_DIAGONAL = (11),
        CELL_GCM_COMPMODE_Z32_SEPSTENCIL_ROTATED = (12),

        // SetZcull
        CELL_GCM_ZCULL_Z16 = (1),
        CELL_GCM_ZCULL_Z24S8 = (2),
        CELL_GCM_ZCULL_MSB = (0),
        CELL_GCM_ZCULL_LONES = (1),
        CELL_GCM_ZCULL_LESS = (0),
        CELL_GCM_ZCULL_GREATER = (1),

        CELL_GCM_SCULL_SFUNC_NEVER = (0),
        CELL_GCM_SCULL_SFUNC_LESS = (1),
        CELL_GCM_SCULL_SFUNC_EQUAL = (2),
        CELL_GCM_SCULL_SFUNC_LEQUAL = (3),
        CELL_GCM_SCULL_SFUNC_GREATER = (4),
        CELL_GCM_SCULL_SFUNC_NOTEQUAL = (5),
        CELL_GCM_SCULL_SFUNC_GEQUAL = (6),
        CELL_GCM_SCULL_SFUNC_ALWAYS = (7),

        // flip mode
        CELL_GCM_DISPLAY_HSYNC = (1),
        CELL_GCM_DISPLAY_VSYNC = (2),
        CELL_GCM_DISPLAY_HSYNC_WITH_NOISE = (3),

        // vsync frequency
        CELL_GCM_DISPLAY_FREQUENCY_59_94HZ = (1),
        CELL_GCM_DISPLAY_FREQUENCY_SCANOUT = (2),
        CELL_GCM_DISPLAY_FREQUENCY_DISABLE = (3),


        CELL_GCM_TYPE_B = (1),
        CELL_GCM_TYPE_C = (2),
        CELL_GCM_TYPE_RSX = (3),

        // MRT
        CELL_GCM_MRT_MAXCOUNT = (4),

        // max display id
        CELL_GCM_DISPLAY_MAXID = (8),

        // Debug output level
        CELL_GCM_DEBUG_LEVEL0 = (0),
        CELL_GCM_DEBUG_LEVEL1 = (1),
        CELL_GCM_DEBUG_LEVEL2 = (2),

        // SetRenderEnable
        CELL_GCM_CONDITIONAL = (2),

        // SetClearReport, SetReport, GetReport
        CELL_GCM_ZPASS_PIXEL_CNT = (1),
        CELL_GCM_ZCULL_STATS = (2),
        CELL_GCM_ZCULL_STATS1 = (3),
        CELL_GCM_ZCULL_STATS2 = (4),
        CELL_GCM_ZCULL_STATS3 = (5),

        // SetPointSpriteControl
        CELL_GCM_POINT_SPRITE_RMODE_ZERO = (0),
        CELL_GCM_POINT_SPRITE_RMODE_FROM_R = (1),
        CELL_GCM_POINT_SPRITE_RMODE_FROM_S = (2),

        CELL_GCM_POINT_SPRITE_TEX0 = (1 << 8),
        CELL_GCM_POINT_SPRITE_TEX1 = (1 << 9),
        CELL_GCM_POINT_SPRITE_TEX2 = (1 << 10),
        CELL_GCM_POINT_SPRITE_TEX3 = (1 << 11),
        CELL_GCM_POINT_SPRITE_TEX4 = (1 << 12),
        CELL_GCM_POINT_SPRITE_TEX5 = (1 << 13),
        CELL_GCM_POINT_SPRITE_TEX6 = (1 << 14),
        CELL_GCM_POINT_SPRITE_TEX7 = (1 << 15),
        CELL_GCM_POINT_SPRITE_TEX8 = (1 << 16),
        CELL_GCM_POINT_SPRITE_TEX9 = (1 << 17),

        // SetUserClipPlaneControl
        CELL_GCM_USER_CLIP_PLANE_DISABLE = (0),
        CELL_GCM_USER_CLIP_PLANE_ENABLE_LT = (1),
        CELL_GCM_USER_CLIP_PLANE_ENABLE_GE = (2),

        // SetAttribOutputMask
        CELL_GCM_ATTRIB_OUTPUT_MASK_FRONTDIFFUSE = (1 << 0),
        CELL_GCM_ATTRIB_OUTPUT_MASK_FRONTSPECULAR = (1 << 1),
        CELL_GCM_ATTRIB_OUTPUT_MASK_BACKDIFFUSE = (1 << 2),
        CELL_GCM_ATTRIB_OUTPUT_MASK_BACKSPECULAR = (1 << 3),
        CELL_GCM_ATTRIB_OUTPUT_MASK_FOG = (1 << 4),
        CELL_GCM_ATTRIB_OUTPUT_MASK_POINTSIZE = (1 << 5),
        CELL_GCM_ATTRIB_OUTPUT_MASK_UC0 = (1 << 6),
        CELL_GCM_ATTRIB_OUTPUT_MASK_UC1 = (1 << 7),
        CELL_GCM_ATTRIB_OUTPUT_MASK_UC2 = (1 << 8),
        CELL_GCM_ATTRIB_OUTPUT_MASK_UC3 = (1 << 9),
        CELL_GCM_ATTRIB_OUTPUT_MASK_UC4 = (1 << 10),
        CELL_GCM_ATTRIB_OUTPUT_MASK_UC5 = (1 << 11),
        CELL_GCM_ATTRIB_OUTPUT_MASK_TEX8 = (1 << 12),
        CELL_GCM_ATTRIB_OUTPUT_MASK_TEX9 = (1 << 13),
        CELL_GCM_ATTRIB_OUTPUT_MASK_TEX0 = (1 << 14),
        CELL_GCM_ATTRIB_OUTPUT_MASK_TEX1 = (1 << 15),
        CELL_GCM_ATTRIB_OUTPUT_MASK_TEX2 = (1 << 16),
        CELL_GCM_ATTRIB_OUTPUT_MASK_TEX3 = (1 << 17),
        CELL_GCM_ATTRIB_OUTPUT_MASK_TEX4 = (1 << 18),
        CELL_GCM_ATTRIB_OUTPUT_MASK_TEX5 = (1 << 19),
        CELL_GCM_ATTRIB_OUTPUT_MASK_TEX6 = (1 << 20),
        CELL_GCM_ATTRIB_OUTPUT_MASK_TEX7 = (1 << 21),

        // SetFogMode
        CELL_GCM_FOG_MODE_LINEAR = (0x2601),
        CELL_GCM_FOG_MODE_EXP = (0x0800),
        CELL_GCM_FOG_MODE_EXP2 = (0x0801),
        CELL_GCM_FOG_MODE_EXP_ABS = (0x0802),
        CELL_GCM_FOG_MODE_EXP2_ABS = (0x0803),
        CELL_GCM_FOG_MODE_LINEAR_ABS = (0x0804),

        // SetTextureOptimization
        CELL_GCM_TEXTURE_ISO_LOW = (0),
        CELL_GCM_TEXTURE_ISO_HIGH = (1),
        CELL_GCM_TEXTURE_ANISO_LOW = (0),
        CELL_GCM_TEXTURE_ANISO_HIGH = (1),

        // SetDepthFormat
        CELL_GCM_DEPTH_FORMAT_FIXED = (0),
        CELL_GCM_DEPTH_FORMAT_FLOAT = (1),

        // SetFrontPolygonMode, SetBackPolygonMode
        CELL_GCM_POLYGON_MODE_POINT = (0x1B00),
        CELL_GCM_POLYGON_MODE_LINE = (0x1B01),
        CELL_GCM_POLYGON_MODE_FILL = (0x1B02),

        // CellGcmTransferScale
        CELL_GCM_TRANSFER_SURFACE = (0),
        CELL_GCM_TRANSFER_SWIZZLE = (1),

        CELL_GCM_TRANSFER_CONVERSION_DITHER = (0),
        CELL_GCM_TRANSFER_CONVERSION_TRUNCATE = (1),
        CELL_GCM_TRANSFER_CONVERSION_SUBTRACT_TRUNCATE = (2),

        CELL_GCM_TRANSFER_SCALE_FORMAT_A1R5G5B5 = (1),
        CELL_GCM_TRANSFER_SCALE_FORMAT_X1R5G5B5 = (2),
        CELL_GCM_TRANSFER_SCALE_FORMAT_A8R8G8B8 = (3),
        CELL_GCM_TRANSFER_SCALE_FORMAT_X8R8G8B8 = (4),
        CELL_GCM_TRANSFER_SCALE_FORMAT_CR8YB8CB8YA8 = (5),
        CELL_GCM_TRANSFER_SCALE_FORMAT_YB8CR8YA8CB8 = (6),
        CELL_GCM_TRANSFER_SCALE_FORMAT_R5G6B5 = (7),
        CELL_GCM_TRANSFER_SCALE_FORMAT_Y8 = (8),
        CELL_GCM_TRANSFER_SCALE_FORMAT_AY8 = (9),
        CELL_GCM_TRANSFER_SCALE_FORMAT_EYB8ECR8EYA8ECB8 = (0xA),
        CELL_GCM_TRANSFER_SCALE_FORMAT_ECR8EYB8ECB8EYA8 = (0xB),
        CELL_GCM_TRANSFER_SCALE_FORMAT_A8B8G8R8 = (0xC),
        CELL_GCM_TRANSFER_SCALE_FORMAT_X8B8G8R8 = (0xD),

        CELL_GCM_TRANSFER_OPERATION_SRCCOPY_AND = (0),
        CELL_GCM_TRANSFER_OPERATION_ROP_AND = (1),
        CELL_GCM_TRANSFER_OPERATION_BLEND_AND = (2),
        CELL_GCM_TRANSFER_OPERATION_SRCCOPY = (3),
        CELL_GCM_TRANSFER_OPERATION_SRCCOPY_PREMULT = (4),
        CELL_GCM_TRANSFER_OPERATION_BLEND_PREMULT = (5),

        CELL_GCM_TRANSFER_ORIGIN_CENTER = (1),
        CELL_GCM_TRANSFER_ORIGIN_CORNER = (2),

        CELL_GCM_TRANSFER_INTERPOLATOR_ZOH = (0),
        CELL_GCM_TRANSFER_INTERPOLATOR_FOH = (1),

        // CellGcmTransferSurface, CellGcmTransferSwizzle
        CELL_GCM_TRANSFER_SURFACE_FORMAT_R5G6B5 = (4),
        CELL_GCM_TRANSFER_SURFACE_FORMAT_A8R8G8B8 = (0xA),
        CELL_GCM_TRANSFER_SURFACE_FORMAT_Y32 = (0xB),

        // SetFragmentProgramLoad
        CELL_GCM_SHIFT_SET_SHADER_CONTROL_CONTROL_TXP = (15),
        CELL_GCM_MASK_SET_SHADER_CONTROL_CONTROL_TXP = (0x00008000),

        // MapEaIoAddressWithFlags
        CELL_GCM_IOMAP_FLAG_STRICT_ORDERING = (1 << 1),

        // label
        CELL_GCM_INDEX_RANGE_LABEL_MIN = 64,
        CELL_GCM_INDEX_RANGE_LABEL_MAX = 255,
        CELL_GCM_INDEX_RANGE_LABEL_COUNT = (256 - 64),

        // notify
        CELL_GCM_INDEX_RANGE_NOTIFY_MAIN_MIN = 0,
        CELL_GCM_INDEX_RANGE_NOTIFY_MAIN_MAX = 255,
        CELL_GCM_INDEX_RANGE_NOTIFY_MAIN_COUNT = 256,

        // report
        CELL_GCM_INDEX_RANGE_REPORT_MAIN_MIN = 0,
        CELL_GCM_INDEX_RANGE_REPORT_MAIN_MAX = (1024 * 1024 - 1),
        CELL_GCM_INDEX_RANGE_REPORT_MAIN_COUNT = (1024 * 1024),
        CELL_GCM_INDEX_RANGE_REPORT_LOCAL_MIN = 0,
        CELL_GCM_INDEX_RANGE_REPORT_LOCAL_MAX = 2047,
        CELL_GCM_INDEX_RANGE_REPORT_LOCAL_COUNT = 2048,

        // tile
        CELL_GCM_INDEX_RANGE_TILE_MIN = 0,
        CELL_GCM_INDEX_RANGE_TILE_MAX = 14,
        CELL_GCM_INDEX_RANGE_TILE_COUNT = 15,

        // zcull
        CELL_GCM_INDEX_RANGE_ZCULL_MIN = 0,
        CELL_GCM_INDEX_RANGE_ZCULL_MAX = 7,
        CELL_GCM_INDEX_RANGE_ZCULL_COUNT = 8,

        // field
        CELL_GCM_DISPLAY_FIELD_TOP = 1,
        CELL_GCM_DISPLAY_FIELD_BOTTOM = 0,

        // flip status
        CELL_GCM_DISPLAY_FLIP_STATUS_DONE = 0,
        CELL_GCM_DISPLAY_FLIP_STATUS_WAITING = 1,

        // zcull align
        CELL_GCM_ZCULL_ALIGN_OFFSET = (4096),
        CELL_GCM_ZCULL_ALIGN_WIDTH = (64),
        CELL_GCM_ZCULL_ALIGN_HEIGHT = (64),
        CELL_GCM_ZCULL_ALIGN_CULLSTART = (4096),
        CELL_GCM_ZCULL_COMPRESSION_TAG_BASE_MAX = (0x7FF),
        CELL_GCM_ZCULL_RAM_SIZE_MAX = (0x00300000),

        // tile align
        CELL_GCM_TILE_ALIGN_OFFSET = (0x00010000),
        CELL_GCM_TILE_ALIGN_SIZE = (0x00010000),
        CELL_GCM_TILE_LOCAL_ALIGN_HEIGHT = (32),
        CELL_GCM_TILE_MAIN_ALIGN_HEIGHT = (64),

        CELL_GCM_TILE_ALIGN_BUFFER_START_BOUNDARY = (8),

        // ucode align
        CELL_GCM_FRAGMENT_UCODE_LOCAL_ALIGN_OFFSET = (64),
        CELL_GCM_FRAGMENT_UCODE_MAIN_ALIGN_OFFSET = (128),

        // surface align
        CELL_GCM_SURFACE_LINEAR_ALIGN_OFFSET = (64),
        CELL_GCM_SURFACE_SWIZZLE_ALIGN_OFFSET = (128),

        // texture align
        CELL_GCM_TEXTURE_SWIZZLE_ALIGN_OFFSET = (128),
        CELL_GCM_TEXTURE_CUBEMAP_ALIGN_OFFSET = (128),
        CELL_GCM_TEXTURE_SWIZZLED_CUBEMAP_FACE_ALIGN_OFFSET = (128),

        // cache line size
        CELL_GCM_VERTEX_TEXTURE_CACHE_LINE_SIZE = (32),
        CELL_GCM_L2_TEXTURE_CACHE_LOCAL_LINE_SIZE = (64),
        CELL_GCM_L2_TEXTURE_CACHE_MAIN_LINE_SIZE = (128),

        CELL_GCM_IDX_FRAGMENT_UCODE_INSTRUCTION_PREFETCH_COUNT = (16),

        CELL_GCM_DRAW_INDEX_ARRAY_INDEX_RANGE_MAX = (0x000FFFFF),

        // cursor alignment
        CELL_GCM_CURSOR_ALIGN_OFFSET = (2048),
    }
}
