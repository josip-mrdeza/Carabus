using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Carabus.Shield.Memory.Collections
{
    public class ProtectedCollectionBase<T>
    {
        private readonly IList<T> _collection;
        public IList<T> Value => _collection;

        public ProtectedCollectionBase(IList<T> collection)
        {
            _collection = collection;
        }

        public unsafe void EncryptManaged(int key)
        {
            Type[] typesInClass = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
                                           .Select(y => y.FieldType).ToArray();
            for (int i = 0; i < _collection.Count; i++)
            {
                var value = _collection[i];
                //var managedSize = Marshal.ReadInt32(typeof(T).TypeHandle.Value, 4) - 8;
                GCHandle handle = default;
                IntPtr addy;
                try
                {
                    handle = GCHandle.Alloc(value, GCHandleType.Pinned);
                    addy = handle.AddrOfPinnedObject();
                }
                catch
                {
                    T* ptr = &value;
                    addy = Marshal.ReadIntPtr(new IntPtr(ptr)) + 4;
                }
                for (int k = 0; k < typesInClass.Length; k++)
                {
                    var type = typesInClass[k];
                    if (type.IsValueType)
                    {
                        var managedSize = Marshal.SizeOf(type);
                        for (int j = 0; j < managedSize; j++)
                        {
                            InternalMemFunctions.ProtectBuffer_Import_Unsafe((byte*) addy, 1, key);
                            addy = IntPtr.Add(addy, 1);
                        }
                    }
                    else
                    {
                        addy = EncryptManagedNested(type, key, ref addy);
                    }
                }

                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        private unsafe IntPtr EncryptManagedNested(Type type, int key, ref IntPtr addy)
        {
            Type[] members = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                                      .Select(y => y.FieldType).ToArray();
            if (members.Length == 0 && !type.IsValueType)
            {
                if (type == typeof(string))
                {
                    var address = Marshal.ReadIntPtr(addy);
                    var structSize = Marshal.ReadInt32(address + 4);
                    IntPtr ptr = address + 8;
                    for (int z = 0; z < structSize * 2; z++)
                    {
                        InternalMemFunctions.ProtectBuffer_Import_Unsafe((byte*)ptr, 1, key);
                        ptr = IntPtr.Add(ptr, 1);
                    }
                }
                addy += IntPtr.Size;
                return addy;
            }
            for (int j = 0; j < members.Length; j++)
            {
                var memType = members[j];
                if (!memType.IsValueType)
                {
                    var address = Marshal.ReadIntPtr(addy);
                    EncryptManagedNested(memType, key, ref address);
                    addy += IntPtr.Size;
                    continue;
                }

                var structSize = Marshal.SizeOf(memType);
                for (int z = 0; z < structSize; z++)
                {
                    InternalMemFunctions.ProtectBuffer_Import_Unsafe((byte*) addy, 1, key);
                    addy = IntPtr.Add(addy, 1);
                }
            }

            return addy;
        }

        public unsafe void Encrypt(int key)
        {
            if (!typeof(T).IsValueType)
            {
                EncryptManaged(key);
                return;
            }
            fixed (T* value = _collection.GetInternalArray())
            {
                InternalMemFunctions.ProtectBuffer_Import_Unsafe((byte*)value, sizeof(T)*_collection.Count, key);
            }
        }

        public unsafe void DecryptNestedManaged(ref IntPtr addy, Type type, int key)
        {
            Type[] members = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                                 .Select(y => y.FieldType).ToArray();
            if (members.Length == 0 && !type.IsValueType)
            {
                if (type == typeof(string))
                {
                    var address = Marshal.ReadIntPtr(addy);
                    var structSize = Marshal.ReadInt32(address + 4);
                    IntPtr ptr = address + 8;
                    for (int z = 0; z < structSize * 2; z++)
                    {
                        InternalMemFunctions.ReleaseBuffer_Import_Unsafe((byte*)ptr, 1, key);
                        ptr = IntPtr.Add(ptr, 1);
                    }
                }
                addy += IntPtr.Size;
                return;
            }
            for (int j = 0; j < members.Length; j++)
            {
                var memType = members[j];
                if (!memType.IsValueType)
                {
                    var address = Marshal.ReadIntPtr(addy);
                    DecryptNestedManaged(ref address, memType, key);
                    addy += 4;
                    continue;
                }

                var structSize = Marshal.SizeOf(memType);
                for (int z = 0; z < structSize; z++)
                {
                    InternalMemFunctions.ReleaseBuffer_Import_Unsafe((byte*) addy, 1, key);
                    addy = IntPtr.Add(addy, 1);
                }
            }
        }
        public unsafe void DecryptManaged(int key)
        {
            Type[] typesInClass = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
                                           .Select(y => y.FieldType).ToArray();
            for (int i = 0; i < _collection.Count; i++)
            {
                var value = _collection[i];
                //var managedSize = Marshal.ReadInt32(typeof(T).TypeHandle.Value, 4) - 8;
                GCHandle handle = default;
                IntPtr addy;
                try
                {
                    handle = GCHandle.Alloc(value, GCHandleType.Pinned);
                    addy = handle.AddrOfPinnedObject();
                }
                catch
                {
                    T* ptr = &value;
                    addy = Marshal.ReadIntPtr(new IntPtr(ptr)) + 4;
                }
                for (int k = 0; k < typesInClass.Length; k++)
                {
                    var type = typesInClass[k];
                    if (type.IsValueType)
                    {
                        var managedSize = Marshal.SizeOf(type);
                        for (int j = 0; j < managedSize; j++)
                        {
                            InternalMemFunctions.ReleaseBuffer_Import_Unsafe((byte*) addy, 1, key);
                            addy = IntPtr.Add(addy, 1);
                        }
                    }
                    else
                    {
                        DecryptNestedManaged(ref addy, type, key);
                    }
                }

                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }
        public unsafe void Decrypt(int key)
        {
            if (!typeof(T).IsValueType)
            {
                DecryptManaged(key);
                return;
            }
            fixed (T* value = _collection.GetInternalArray())
            {
                InternalMemFunctions.ReleaseBuffer_Import_Unsafe((byte*)value, sizeof(T)*_collection.Count, key);
            }
        }
    }
}