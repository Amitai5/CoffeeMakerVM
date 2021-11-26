using CoffeeMakerVM;
using CoffeeMakerVM.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CoffeMakerVM
{
    public static class Utilities
    {
        public static ushort SwapEndian(this ushort data)
        {
            ushort retData = (ushort)(data << 8);
            retData |= (ushort)(data >> 8);

            return retData;
        }
        public static uint SwapEndian(this uint data)
        {
            // 3   2  1  0
            // FF FF FF FF

            //  0  0  0  0
            //  0  1  0  0
            //  0  1  2  0
            //  0  1  2  3

            uint retData = data << 24;
            retData |= ((data << 8) & 0x00FF0000);
            retData |= ((data >> 8) & 0x0000FF00);
            retData |= ((data >> 24) & 0x000000FF);
            return retData;
        }

        public static (int paramCount, int returnSize) FunctionParameterCount(ReadOnlySpan<char> descriptor)
        {
            //Check If It Is Invalid
            if (!descriptor.StartsWith("("))
            {
                throw new Exception("The Function Parameter Was Not Valid!");
            }

            int retCount = 1;
            int paramCount = 0;
            for(int i = 0; i < descriptor.Length; i++)
            {
                switch((ParamTypes)descriptor[i])
                {
                    case ParamTypes.EndOfDescriptor:
                        if((ParamTypes)descriptor[i + 1] == ParamTypes.Double || (ParamTypes)descriptor[i + 1] == ParamTypes.Long)
                        {
                            retCount = 2;
                        }
                        else if(descriptor[i + 1] == 'V')
                        {
                            retCount = 0;
                        }
                        return (paramCount, retCount);

                    case ParamTypes.Int:
                    case ParamTypes.Byte:
                    case ParamTypes.Char:
                    case ParamTypes.Short:
                    case ParamTypes.Float:
                    case ParamTypes.Boolean:
                        paramCount++;
                        break;

                    case ParamTypes.Double:
                    case ParamTypes.Long:
                        paramCount += 2;
                        break;

                    case ParamTypes.ClassName:
                        paramCount++;

                        //Go To End Of Class Name
                        for(int ii = i; ii < descriptor.Length; ii++)
                        {
                            if((ParamTypes)descriptor[ii] == ParamTypes.EndOfParamName)
                            {
                                i = ii;
                                break;
                            }
                        }
                        break;
                }
            }

            //WE DIED
            throw new InvalidOperationException("The Loop Should Never Exit...");
        }

        public static void PushLong(this ref Span<int> origStack, ref int stackPointer, int high, int low)
        {
            origStack.Push(ref stackPointer, low);
            origStack.Push(ref stackPointer, high);
        }
        public static (int high, int low) PopLong(this ref Span<int> origStack, ref int stackPointer)
        {
            int high = origStack.Pop(ref stackPointer);
            int low = origStack.Pop(ref stackPointer);
            return (low, high);
        }
        public static void Push(this ref Span<int> origStack, ref int stackPointer, int value)
        {
            origStack[stackPointer] = value;
            stackPointer++;
        }
        public static int Pop(this ref Span<int> origStack, ref int stackPointer)
        {
            stackPointer--;
            return origStack[stackPointer];
        }

        #region Conversions

        public static byte ReadByte(this ref ReadOnlySpan<byte> data)
        {
            byte result = data[0];
            data = data.Slice(1);
            return result;
        }
        public static ushort ReadUshort(this ref ReadOnlySpan<byte> data)
        {
            ushort result = MemoryMarshal.Cast<byte, ushort>(data)[0];
            data = data.Slice(2);
            return result.SwapEndian();
        }
        public static ushort ReadUshort(this ref ReadOnlyMemory<byte> data)
        {
            ushort result = MemoryMarshal.Cast<byte, ushort>(data.Span)[0];
            data = data.Slice(2);
            return result.SwapEndian();
        }

        public static uint ReadUint(this ref ReadOnlySpan<byte> data)
        {
            uint result = MemoryMarshal.Cast<byte, uint>(data)[0];
            data = data.Slice(4);
            return result.SwapEndian();
        }
        public static uint ReadUint(this ref ReadOnlyMemory<byte> data)
        {
            uint result = MemoryMarshal.Cast<byte, uint>(data.Span)[0];
            data = data.Slice(4);
            return result.SwapEndian();
        }

        #endregion Conversions
    }
}
