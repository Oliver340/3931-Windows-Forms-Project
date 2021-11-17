#ifdef __cplusplus
#define EXPORT extern "C" __declspec (dllexport)
#else
#define EXPORT __declspec (dllexport)
#endif

EXPORT INT start();
EXPORT PBYTE getPSaveBuffer();
EXPORT DWORD getSizePSaveBuffer();
EXPORT VOID setPSaveBuffer(PBYTE values, DWORD length);
EXPORT VOID setSizePSaveBuffer(DWORD length);

#ifdef UNICODE
#define Function start
#define Function getPSaveBuffer
#define Function getSizePSaveBuffer
#define Function setPSaveBuffer
#define Function setSizePSaveBuffer
#endif