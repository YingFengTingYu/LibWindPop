#pragma once
#include<io.h>
#ifdef __cplusplus
#include<vector>
#include<string>
#include<fstream>
using namespace std;
extern "C" {
#endif
    void RsbEncryptorOnStartBuild(const char* unpackPath);
    void RsbEncryptorOnEndBuild(const char* rsbPath);
    void RsbEncryptorOnAdd(const char* unpackPath);
#ifdef __cplusplus          
}
#endif
