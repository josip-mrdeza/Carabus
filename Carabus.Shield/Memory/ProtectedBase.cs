using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace Carabus.Shield.Memory
{
    public class ProtectedBase<T> where T: unmanaged
    {
        private T _original;

        public T Value
        {
            get => _original;
            set => _original = value;
        }

        public ProtectedBase(T original)
        {
            _original = original;
        }
        public unsafe void Encrypt(int key)
        {
            fixed (T* og = &_original)
            {
                InternalMemFunctions.ProtectBuffer_Import_Unsafe((byte*) og, sizeof(T), key);
            }
        }

        public unsafe void Decrypt(int key)
        {
            fixed (T* og = &_original)
            {
                InternalMemFunctions.ReleaseBuffer_Import_Unsafe((byte*) og, sizeof(T), key);
            }
        }
    }
}