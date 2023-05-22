#include "main.h"

int main() {
	const char rawPakPath[] = "D:\\main.pak";
	const char newPakPath[] = "D:\\main_new.pak";
	const char unpackPath[] = "D:\\main_unpack_pak";
	sample1_unpack_pak_for_pc(rawPakPath, unpackPath);
	sample2_pack_pak_for_pc(unpackPath, newPakPath);
}

void sample1_unpack_pak_for_pc(const char* pakPath, const char* unpackPath) {
	PakUnpack(pakPath, unpackPath, Bool_False, Bool_False, 0, Bool_False); // Pak for Windows and macOS does not have zlib and align
}

void sample2_pack_pak_for_pc(const char* unpackPath, const char* newPakPath) {
	PakPack(unpackPath, newPakPath, 0, Bool_False);
}
