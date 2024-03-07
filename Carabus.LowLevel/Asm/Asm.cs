using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Carabus.LowLevel
{
    public static class Asm
    {
        public static TDelegate Prepare<TDelegate>(byte[] asmCode)
        {
            unsafe
            {
                fixed (byte* ptr = asmCode)
                {
                    var memoryAddress = (IntPtr) ptr;
                    // Mark memory as EXECUTE_READWRITE to prevent DEP exceptions
                    if (!VirtualProtectEx(Process.GetCurrentProcess().Handle, memoryAddress,
                                          (UIntPtr) asmCode.Length, 0x40 /* EXECUTE_READWRITE */, out uint _))
                    {
                        throw new Win32Exception();
                    }

                    var myAssemblyFunction = Marshal.GetDelegateForFunctionPointer<TDelegate>(memoryAddress);
                    return myAssemblyFunction;
                }
            }
        }
        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
    }
}