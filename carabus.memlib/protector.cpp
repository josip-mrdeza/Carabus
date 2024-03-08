#include <cmath>
#include <windows.h>
void change_key(int* key)
{
    auto current = *key;
    current *= 9;
    if (current > 1000000)
    {
        current -= 2000000;
    }
    *key = current;
}
extern "C" __declspec(dllexport) void protect_buffer(byte* buffer, const int len, int key)
{
    if (key == 0)
    {
        return;
    }
    for (int i = 0; i < len; i++)
    {
        change_key(&key);
        auto b = buffer[i];
        b -= (int) (sin(key) - atan(key));
        b += i * 2;
        b += (int) sqrt(key);    
        buffer[i] = b;
    }
}
extern "C" __declspec(dllexport) void release_buffer(byte* buffer, const int len, int key)
{
    if (key == 0)
    {
        return;
    }
    for (int i = 0; i < len; i++)
    {
        change_key(&key);
        auto b = buffer[i];
        b += (int) (sin(key) - atan(key));
        b -= i * 2;
        b -= (int) sqrt(key);
        buffer[i] = b;
    }
}
