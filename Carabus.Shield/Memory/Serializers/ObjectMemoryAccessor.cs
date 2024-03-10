using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Carabus.Shield.Memory.Serializers
{
    public static class ObjectMemoryAccessor
    {
        public enum MemoryType
        {
            Text,
            Binary
        }

        public struct OMData
        {
            public byte[] AllocatedBuffer { get; internal set; }
            public IntPtr OptionalActualAddress { get; internal set; }

            public OMData(byte[] buffer)
            {
                AllocatedBuffer = buffer;
                OptionalActualAddress = IntPtr.Zero;
            }

            public OMData(IntPtr optionalActualAddress)
            {
                AllocatedBuffer = null;
                OptionalActualAddress = optionalActualAddress;
            }

            public static implicit operator OMData(byte[] buffer)
            {
                return new OMData(buffer);
            }

            public static implicit operator OMData(IntPtr address)
            {
                return new OMData(address);
            }
        }
        public class ObjectMemory
        {
            public string ObjectTypeFullName { get; }
            public OMData Data { get; internal set; }
            public MemoryType MemType { get; internal set; }
            public List<ObjectMemory> Children { get; internal set; } = new List<ObjectMemory>();
            public ObjectMemory(string objectTypeFullName, byte[] data, MemoryType dataType)
            {
                ObjectTypeFullName = objectTypeFullName;
                Data = data;
                MemType = dataType;
            }

            internal ObjectMemory(string objectTypeFullName)
            {
                ObjectTypeFullName = objectTypeFullName;
            }
            
            public object Reconstruct()
            {
                throw new NotImplementedException();
            }

            public override string ToString()
            {
                if (MemType == MemoryType.Binary)
                {
                    return BitConverter.ToInt32(Data.AllocatedBuffer, 0).ToString();
                }
                return Encoding.UTF8.GetString(Data.AllocatedBuffer);
            }
        }

        public static unsafe ObjectMemory AccessObjMemory(this object obj, bool avoidAllocations)
        {
            GC.TryStartNoGCRegion(1_000);
            var parentType = obj.GetType();
            var pp = new IntPtr(&obj);
            var complx = AccessComplexObjectMemory(ref pp, parentType, null, avoidAllocations);
            complx.Data = pp;
            GC.EndNoGCRegion();

            return complx;
        }

        private static ObjectMemory AccessStringMemory(this ref IntPtr parentAddress, bool avoidAllocations)
        {
            var ofm = new ObjectMemory(typeof(string).FullName);
            var stringAddress = Marshal.ReadIntPtr(parentAddress);
            parentAddress += 4;
            var len = Marshal.ReadInt32(stringAddress + 4) * 2;
            stringAddress += 8;
            if (avoidAllocations)
            {
                ofm.Data = stringAddress;
                ofm.MemType = MemoryType.Text;
                return ofm;
            }
            byte[] buffer = new byte[len];
            Marshal.Copy(stringAddress, buffer, 0, len);
            ofm.Data = buffer;
            ofm.MemType = MemoryType.Text;

            return ofm; 
        }

        private static ObjectMemory AccessSimpleStructMemory<T>(this T simpleStruct, bool avoidAllocations) where T : struct
        {
            ObjectMemory ofm = new ObjectMemory(typeof(T).FullName);
            unsafe
            {
                if (avoidAllocations)
                {
                    ofm.Data = new IntPtr(&simpleStruct);
                    ofm.MemType = MemoryType.Binary;

                    return ofm;
                }
                int structSize = sizeof(T);
                byte[] buffer = new byte[structSize];
                Marshal.Copy(new IntPtr(&simpleStruct), buffer, 0, structSize);
                ofm.Data = buffer;
                ofm.MemType = MemoryType.Binary;
            }
            return ofm;
        }

        private static unsafe ObjectMemory AccessComplexObjectMemory(this ref IntPtr parentPointer, 
            Type complexObjectType, ObjectMemory parent, bool avoidAllocations)
        {
            var parentType = complexObjectType;
            ObjectMemory ofm = new ObjectMemory(parentType.FullName);
            if (parent != null)
            {
                parent.Children.Add(ofm);
            }
            if (!parentType.IsValueType)
            {
                parentPointer = Marshal.ReadIntPtr(parentPointer) + 4; //Read the address the class reference points to.
            }
            var possibleChildren =
                parentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var child in possibleChildren)
            {
                var type = child.FieldType;
                ObjectMemory childMemory = null;
                if (type.IsValueType)
                {
                    childMemory = RetrieveChildMemory_ValueType(ref parentPointer, type, ofm, avoidAllocations);
                }
                else
                {
                    if (type == typeof(string))
                    {
                        childMemory = AccessStringMemory(ref parentPointer, avoidAllocations);
                    }
                    else
                    {
                        childMemory = AccessComplexObjectMemory(ref parentPointer, type, ofm, avoidAllocations);
                    }
                }
                ofm.Children.Add(childMemory);
            }

            return ofm;
        }

        private static ObjectMemory RetrieveChildMemory_ValueType(ref IntPtr parentPointer, Type type, ObjectMemory parent, bool avoidAllocations)
        {
            var ofm = new ObjectMemory(type.FullName);
            var size = Marshal.SizeOf(type);
            if (type.IsPrimitive)
            {
                ofm.MemType = MemoryType.Binary;
                if (avoidAllocations)
                {
                    ofm.Data = parentPointer;
                    parentPointer = IntPtr.Add(parentPointer, size);
                    return ofm;
                }
                ofm.Data = new byte[size];
                Marshal.Copy(parentPointer, ofm.Data.AllocatedBuffer, 0, size);
                parentPointer = IntPtr.Add(parentPointer, size);
            }
            else
            {
                AccessComplexObjectMemory(ref parentPointer, type, parent, avoidAllocations);
            }

            return ofm;
        }
    }
}