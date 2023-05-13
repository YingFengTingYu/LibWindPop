#pragma once
#include "LibWindPop.h"
#include "PtxHandlerAndroidV3H.h"
#include "TestPtxHandlerPS3V1.h"

void sample1_unpack_rsb_for_pvz1_free(const char* rsbPath, const char* unpackPath);
void sample2_pack_rsb_for_pvz1_free(const char* unpackPath, const char* newRsbPath);
void sample3_regist_ptx_handler_and_unpack_rsb_for_bejeweled3_ps3(const char* rsbPath, const char* unpackPath);
void sample4_regist_ptx_handler_and_pack_rsb_for_bejeweled3_ps3(const char* unpackPath, const char* newRsbPath);
void sample5_regist_high_quality_ptx_handler_and_unpack_rsb_for_pvz2_android_row(const char* rsbPath, const char* unpackPath);
void sample6_regist_high_quality_ptx_handler_and_pack_rsb_for_pvz2_android_row(const char* unpackPath, const char* newRsbPath);
