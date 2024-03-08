using System.Runtime.InteropServices;

namespace Carabus.Shield.Memory
{
    public static class InternalMemFunctions
    {
        [DllImport("carabus.memlib.dll", EntryPoint = "protect_buffer",CallingConvention = CallingConvention.StdCall)]
        internal static extern void ProtectBuffer_Import(byte[] buffer, int len, int key);
        [DllImport("carabus.memlib.dll", EntryPoint = "release_buffer",CallingConvention = CallingConvention.StdCall)]
        internal static extern void ReleaseBuffer_Import(byte[] buffer, int len, int key);
        [DllImport("carabus.memlib.dll", EntryPoint = "protect_buffer",CallingConvention = CallingConvention.StdCall)]
        internal static extern unsafe void ProtectBuffer_Import_Unsafe(byte* buffer, int len, int key);
        [DllImport("carabus.memlib.dll", EntryPoint = "release_buffer",CallingConvention = CallingConvention.StdCall)]
        internal static extern unsafe void ReleaseBuffer_Import_Unsafe(byte* buffer, int len, int key);

        public static void ProtectBuffer(byte[] buffer, int key)
        {
            ProtectBuffer_Import(buffer, buffer.Length, key);
        }
        public static void ReleaseBuffer(byte[] buffer, int key)
        {
            ReleaseBuffer_Import(buffer, buffer.Length, key);
        }
    }
}