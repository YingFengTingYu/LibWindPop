#include "main.h"

int main() {
	const char rawRsbPath[] = "D:\\main.rsb";
	const char newRsbPath[] = "D:\\main_new.rsb";
	const char unpackPath[] = "D:\\main_unpack_rsb";
	sample5_regist_high_quality_ptx_handler_and_unpack_rsb_for_pvz2_android_row(rawRsbPath, unpackPath);
	sample6_regist_high_quality_ptx_handler_and_pack_rsb_for_pvz2_android_row(unpackPath, newRsbPath);
}

void sample1_unpack_rsb_for_pvz1_free(const char* rsbPath, const char* unpackPath) {
	RsbUnpack(rsbPath, unpackPath, PtxHandleriOSV3, Bool_False, 0, Bool_False); // unpack rsb
	RsbAddContentPipeline(unpackPath, RsbPipelineEncodePtxFromPng, 0, Bool_False); // decode all ptx
}

void sample2_pack_rsb_for_pvz1_free(const char* unpackPath, const char* newRsbPath) {
	RsbPack(unpackPath, newRsbPath, 0, Bool_False); // pack rsb
}

void sample3_regist_ptx_handler_and_unpack_rsb_for_bejeweled3_ps3(const char* rsbPath, const char* unpackPath) {
	const char ptx_handler_type[] = "TestPtxHandlerPS3V1"; // define name
	PtxRsbRegistHandler(ptx_handler_type, Bool_False, &TestPtxHandlerPS3V1_GetPtxSize, &TestPtxHandlerPS3V1_GetPtxSizeWithoutAlpha, &TestPtxHandlerPS3V1_DecodePtx, &TestPtxHandlerPS3V1_EncodePtx, &TestPtxHandlerPS3V1_PeekEncodedPtxInfo); // regist native ptx handler
	RsbUnpack(rsbPath, unpackPath, ptx_handler_type, Bool_False, 0, Bool_False); // unpack rsb
	RsbAddContentPipeline(unpackPath, RsbPipelineEncodePtxFromPng, 0, Bool_False); // decode all ptx
}

void sample4_regist_ptx_handler_and_pack_rsb_for_bejeweled3_ps3(const char* unpackPath, const char* newRsbPath) {
	const char ptx_handler_type[] = "TestPtxHandlerPS3V1"; // define name
	PtxRsbRegistHandler(ptx_handler_type, Bool_False, &TestPtxHandlerPS3V1_GetPtxSize, &TestPtxHandlerPS3V1_GetPtxSizeWithoutAlpha, &TestPtxHandlerPS3V1_DecodePtx, &TestPtxHandlerPS3V1_EncodePtx, &TestPtxHandlerPS3V1_PeekEncodedPtxInfo); // regist native ptx handler
	RsbPack(unpackPath, newRsbPath, 0, Bool_False); // pack rsb
}

void sample5_regist_high_quality_ptx_handler_and_unpack_rsb_for_pvz2_android_row(const char* rsbPath, const char* unpackPath) {
	const char ptx_handler_type[] = "PtxHandlerAndroidV3H"; // define name
	PtxRsbRegistHandler(ptx_handler_type, Bool_False, &PtxHandlerAndroidV3H_GetPtxSize, &PtxHandlerAndroidV3H_GetPtxSizeWithoutAlpha, &PtxHandlerAndroidV3H_DecodePtx, &PtxHandlerAndroidV3H_EncodePtx, &PtxHandlerAndroidV3H_PeekEncodedPtxInfo); // regist native ptx handler
	RsbUnpack(rsbPath, unpackPath, ptx_handler_type, Bool_False, 0, Bool_False); // unpack rsb
	RsbAddContentPipeline(unpackPath, RsbPipelineEncodePtxFromPng, 0, Bool_False); // decode all ptx
}

void sample6_regist_high_quality_ptx_handler_and_pack_rsb_for_pvz2_android_row(const char* unpackPath, const char* newRsbPath) {
	const char ptx_handler_type[] = "PtxHandlerAndroidV3H"; // define name
	PtxRsbRegistHandler(ptx_handler_type, Bool_False, &PtxHandlerAndroidV3H_GetPtxSize, &PtxHandlerAndroidV3H_GetPtxSizeWithoutAlpha, &PtxHandlerAndroidV3H_DecodePtx, &PtxHandlerAndroidV3H_EncodePtx, &PtxHandlerAndroidV3H_PeekEncodedPtxInfo); // regist native ptx handler
	RsbPack(unpackPath, newRsbPath, 0, Bool_False); // pack rsb
}
