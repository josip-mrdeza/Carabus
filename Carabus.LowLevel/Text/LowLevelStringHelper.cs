using System;

namespace Carabus.LowLevel.Text;

public static class LowLevelStringHelper
{
    public static void ReplaceNoMemAlloc(this string s, char original, char replacement)
    {
        unsafe
        {
            fixed (char* ptr = s)
            {
                char* copy = ptr;
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == original)
                    {
                        copy[i] = replacement;
                    }
                }
            }
        }
    }
}