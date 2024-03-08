#include <windows.h>
extern "C" __declspec(dllexport) void protect_buffer(byte* buffer, int len, int key);
extern "C" __declspec(dllexport) void release_buffer(byte* buffer, int len, int key);
void change_key(int* key);
