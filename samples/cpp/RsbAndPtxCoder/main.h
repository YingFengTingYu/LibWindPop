#pragma once
#include "LibWindPop.h"
#include "PtxHandlerBestQuality.h"
#include "TestPtxHandlerPS3V1.h"
#include "RsbEncryptor.h"
#include "PtxHandlerForPVZFree.h"
#include<stdio.h>

void sample1_unpack_rsb_for_pvz1_free(const char* rsbPath, const char* unpackPath);
void sample2_pack_rsb_for_pvz1_free(const char* unpackPath, const char* newRsbPath);
void sample3_regist_ptx_handler_and_unpack_rsb_for_bejeweled3_ps3(const char* rsbPath, const char* unpackPath);
void sample4_regist_ptx_handler_and_pack_rsb_for_bejeweled3_ps3(const char* unpackPath, const char* newRsbPath);
void sample5_regist_high_quality_ptx_handler_and_unpack_rsb_for_pvz2_android_row(const char* rsbPath, const char* unpackPath);
void sample6_regist_high_quality_ptx_handler_and_pack_rsb_for_pvz2_android_row(const char* unpackPath, const char* newRsbPath);
void sample7_regist_high_quality_ptx_handler_and_unpack_rsb_for_pvz2_ios_row(const char* rsbPath, const char* unpackPath);
void sample8_regist_high_quality_ptx_handler_and_pack_rsb_for_pvz2_ios_row(const char* unpackPath, const char* newRsbPath);
void sample9_regist_rsb_encryptor_pipeline_and_unpack_and_pack_rsb_for_pvz2_android_row(const char* rsbPath, const char* unpackPath, const char* newRsbPath);
void sample10_regist_compressed_ptx_handler_and_unpack_rsb_for_pvz_free_android(const char* rsbPath, const char* unpackPath);
void sample11_regist_compressed_ptx_handler_and_pack_rsb_for_pvz_free_android(const char* unpackPath, const char* newRsbPath);
