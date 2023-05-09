#define WIND_IMPORT
#define WIND__API

typedef enum ENUMBool
{
    Bool_False = 0,
    Bool_True = 1,
} EBool;

typedef enum ENUMResult
{
    Result_OK = 0,
    Result_Error = 1,
} EResult;

WIND_IMPORT EResult WIND__API RsbUnpack(const char *rsbPath, const char *unpackPath, const char *ptxHandlerType, int logLevel, EBool useGroupFolder, EBool throwException);

WIND_IMPORT EResult WIND__API RsbUnpackU8(const char *rsbPath, const char *unpackPath, const char *ptxHandlerType, int logLevel, EBool useGroupFolder, EBool throwException);

WIND_IMPORT EResult WIND__API RsbAddContentPipeline(const char *unpackPath, const char *pipelineName, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND__API RsbAddContentPipelineU8(const char* unpackPath, const char* pipelineName, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND__API RsbPack(const char *unpackPath, const char *rsbPath, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND__API RsbPackU8(const char *unpackPath, const char *rsbPath, int logLevel, EBool throwException);

WIND_IMPORT size_t WIND__API GetErrorSize();

WIND_IMPORT size_t WIND__API GetErrorSizeU8();

WIND_IMPORT EResult WIND__API GetError(char* buffer);

WIND_IMPORT EResult WIND__API GetErrorU8(char* buffer);
