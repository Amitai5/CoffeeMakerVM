using CoffeeMakerVM.ClassInfos;
using CoffeeMakerVM.Enums;
using CoffeeMakerVM.Heap;
using CoffeMakerVM;
using CoffeMakerVM.ConstantPool;
using CoffeMakerVM.ConstantPool.Classes;
using CoffeMakerVM.ConstantPool.Numerics;
using System;

namespace CoffeeMakerVM
{
    public readonly struct MethodFrame
    {
        public ushort MaxStack { get; }
        public ushort MaxLocals { get; }
        public ReadOnlyMemory<byte> Code { get; }
        private JavaClassFile ParentClassFile { get; }

        public MethodFrame(ReadOnlyMemory<byte> fileData, JavaClassFile classFile)
        {
            MaxStack = fileData.ReadUshort();
            MaxLocals = fileData.ReadUshort();
            uint codeLength = fileData.ReadUint();
            Code = fileData.Slice(0, (int)codeLength);

            //Set Class File
            ParentClassFile = classFile;
        }

        public ReturnValue Execute(ReadOnlySpan<int> paramaters = default)
        {
            Span<int> locals = stackalloc int[MaxLocals];
            Span<int> stack = stackalloc int[MaxStack];
            ReadOnlySpan<byte> code = Code.Span;
            int programCount = 0;
            int stackPointer = 0;

            //Get Params
            for (int i = 0; i < paramaters.Length; i++)
            {
                locals[i] = paramaters[i];
            }

            while (true)
            {
                int oldProgramCount = programCount;
                byte opCode = code[programCount++];

                switch ((OpCodes)opCode)
                {
                    case OpCodes.Nop:
                        break;
                    case OpCodes.Pop:
                        stack.Pop(ref stackPointer);
                        break;
                    case OpCodes.Dup:
                        int dupNum = stack.Pop(ref stackPointer);
                        stack.Push(ref stackPointer, dupNum);
                        stack.Push(ref stackPointer, dupNum);
                        break;

                    //Pushes
                    case OpCodes.Bipush:
                        stack.Push(ref stackPointer, code[programCount++]);
                        break;
                    case OpCodes.Sipush:
                        {
                            byte high = code[programCount++];
                            byte low = code[programCount++];
                            ushort val = (ushort)((high << 8) | low);
                            short sVal = (short)val;

                            stack.Push(ref stackPointer, sVal);
                        }
                        break;
                    case OpCodes.Ldc:
                        {
                            byte index = code[programCount++];
                            ConstantPoolInfo constData = ParentClassFile.Constants[index];
                            switch (constData)
                            {
                                case ConstantFloatInfo floatVal:
                                    stack.Push(ref stackPointer, floatVal.Bytes);
                                    break;
                                case ConstantIntegerInfo intVal:
                                    stack.Push(ref stackPointer, intVal.Bytes);
                                    break;
                                case ConstantLongInfo longVal:
                                    stack.PushLong(ref stackPointer, longVal.HighBytes, longVal.LowBytes);
                                    break;
                                default:
                                    throw new NotSupportedException("Does Not Contain A Cast For Type!");
                            }
                        }
                        break;

                    #region Long Math
                    case OpCodes.Ladd:
                        {
                            (int firstLongHigh, int firstLongLow) = stack.PopLong(ref stackPointer);
                            (int secondLongHigh, int secondLongLow) = stack.PopLong(ref stackPointer);

                            long firstLong = (ushort)((firstLongHigh << 32) | firstLongLow);
                            long secondLong = (ushort)((secondLongHigh << 32) | secondLongLow);

                            long resultLong = secondLong + firstLong;
                            stack.PushLong(ref stackPointer, (int)(resultLong >> 32), (int)(resultLong));
                        }
                        break;
                    case OpCodes.Lsub:
                        {
                            (int firstLongHigh, int firstLongLow) = stack.PopLong(ref stackPointer);
                            (int secondLongHigh, int secondLongLow) = stack.PopLong(ref stackPointer);

                            long firstLong = (ushort)((firstLongHigh << 32) | firstLongLow);
                            long secondLong = (ushort)((secondLongHigh << 32) | secondLongLow);

                            long resultLong = secondLong - firstLong;
                            stack.PushLong(ref stackPointer, (int)(resultLong >> 32), (int)(resultLong));
                        }
                        break;
                    case OpCodes.Lmul:
                        {
                            (int firstLongHigh, int firstLongLow) = stack.PopLong(ref stackPointer);
                            (int secondLongHigh, int secondLongLow) = stack.PopLong(ref stackPointer);

                            long firstLong = (ushort)((firstLongHigh << 32) | firstLongLow);
                            long secondLong = (ushort)((secondLongHigh << 32) | secondLongLow);

                            long resultLong = secondLong * firstLong;
                            stack.PushLong(ref stackPointer, (int)(resultLong >> 32), (int)(resultLong));
                        }
                        break;
                    case OpCodes.Ldiv:
                        {
                            (int firstLongHigh, int firstLongLow) = stack.PopLong(ref stackPointer);
                            (int secondLongHigh, int secondLongLow) = stack.PopLong(ref stackPointer);

                            long firstLong = (ushort)((firstLongHigh << 32) | firstLongLow);
                            long secondLong = (ushort)((secondLongHigh << 32) | secondLongLow);

                            long resultLong = secondLong / firstLong;
                            stack.PushLong(ref stackPointer, (int)(resultLong >> 32), (int)(resultLong));
                        }
                        break;
                    #endregion Long Math

                    #region Long Constants
                    case OpCodes.Lconst_0:
                        stack.PushLong(ref stackPointer, 0, 0);
                        break;
                    case OpCodes.Lconst_1:
                        stack.PushLong(ref stackPointer, 0, 1);
                        break;
                    #endregion

                    #region Comparing Codes
                    case OpCodes.Lcmp:
                        {
                            (int firstLongHigh, int firstLongLow) = stack.PopLong(ref stackPointer);
                            (int secondLongHigh, int secondLongLow) = stack.PopLong(ref stackPointer);

                            long firstLong = (ushort)((firstLongHigh << 32) | firstLongLow);
                            long secondLong = (ushort)((secondLongHigh << 32) | secondLongLow);

                            //Run The Check
                            if (firstLong == secondLong)
                            {
                                stack.Push(ref stackPointer, 0);
                            }
                            else if (firstLong > secondLong)
                            {
                                stack.Push(ref stackPointer, 1);
                            }
                            else
                            {
                                stack.Push(ref stackPointer, -1);
                            }
                        }
                        break;
                    #endregion Comparing Codes

                    #region Load And Store Longs
                    case OpCodes.Ldc2_w:
                        {
                            byte byte1 = code[programCount++];
                            byte byte2 = code[programCount++];
                            ushort index = (ushort)((byte1 << 8) + byte2);

                            int lowBytes = ((ConstantLongInfo)ParentClassFile.Constants[index]).LowBytes;
                            int highBytes = ((ConstantLongInfo)ParentClassFile.Constants[index]).HighBytes;
                            stack.PushLong(ref stackPointer, highBytes, lowBytes);
                        }
                        break;
                    case OpCodes.Lload_0:
                        stack.PushLong(ref stackPointer, locals[0], locals[1]);
                        break;
                    #endregion Load And Store Longs

                    #region Integer Store And Load
                    case OpCodes.Istore_0:
                        locals[0] = stack.Pop(ref stackPointer);
                        break;
                    case OpCodes.Istore_1:
                        locals[1] = stack.Pop(ref stackPointer);
                        break;
                    case OpCodes.Istore_2:
                        locals[2] = stack.Pop(ref stackPointer);
                        break;
                    case OpCodes.Istore_3:
                        locals[3] = stack.Pop(ref stackPointer);
                        break;

                    case OpCodes.Iload_0:
                        stack.Push(ref stackPointer, locals[0]);
                        break;
                    case OpCodes.Iload_1:
                        stack.Push(ref stackPointer, locals[1]);
                        break;
                    case OpCodes.Iload_2:
                        stack.Push(ref stackPointer, locals[2]);
                        break;
                    case OpCodes.Iload_3:
                        stack.Push(ref stackPointer, locals[3]);
                        break;
                    #endregion Integer Store And Load

                    #region Address Store And Load
                    case OpCodes.aload_0:
                        {
                            int localAddress = locals[0];
                            stack.Push(ref stackPointer, localAddress);
                        }
                        break;
                    case OpCodes.aload_1:
                        {
                            int localAddress = locals[1];
                            stack.Push(ref stackPointer, localAddress);
                        }
                        break;
                    case OpCodes.aload_2:
                        {
                            int localAddress = locals[2];
                            stack.Push(ref stackPointer, localAddress);
                        }
                        break;
                    case OpCodes.aload_3:
                        {
                            int localAddress = locals[3];
                            stack.Push(ref stackPointer, localAddress);
                        }
                        break;
                    #endregion Address Store And Load

                    #region Integer Math
                    case OpCodes.Iadd:
                        {
                            //Get The Integer First And Second Nums
                            int secondNum = stack.Pop(ref stackPointer);
                            int firstNum = stack.Pop(ref stackPointer);

                            stack.Push(ref stackPointer, firstNum + secondNum);
                        }
                        break;
                    case OpCodes.Isub:
                        {
                            //Get The Integer First And Second Nums
                            int secondNum = stack.Pop(ref stackPointer);
                            int firstNum = stack.Pop(ref stackPointer);

                            stack.Push(ref stackPointer, firstNum - secondNum);
                        }
                        break;
                    case OpCodes.Imul:
                        {
                            //Get The Integer First And Second Nums
                            int secondNum = stack.Pop(ref stackPointer);
                            int firstNum = stack.Pop(ref stackPointer);

                            stack.Push(ref stackPointer, firstNum * secondNum);
                        }
                        break;
                    case OpCodes.Idiv:
                        {
                            //Get The Integer First And Second Nums
                            int secondNum = stack.Pop(ref stackPointer);
                            int firstNum = stack.Pop(ref stackPointer);

                            stack.Push(ref stackPointer, firstNum / secondNum);
                        }
                        break;
                    case OpCodes.Irem:
                        {
                            //Get The Integer First And Second Nums
                            int secondNum = stack.Pop(ref stackPointer);
                            int firstNum = stack.Pop(ref stackPointer);

                            stack.Push(ref stackPointer, firstNum % secondNum);
                        }
                        break;
                    #endregion Integer Math

                    #region Integer Constants
                    case OpCodes.Iconst_m1:
                        stack.Push(ref stackPointer, -1);
                        break;
                    case OpCodes.Iconst_0:
                        stack.Push(ref stackPointer, 0);
                        break;
                    case OpCodes.Iconst_1:
                        stack.Push(ref stackPointer, 1);
                        break;
                    case OpCodes.Iconst_2:
                        stack.Push(ref stackPointer, 2);
                        break;
                    case OpCodes.Iconst_3:
                        stack.Push(ref stackPointer, 3);
                        break;
                    case OpCodes.Iconst_4:
                        stack.Push(ref stackPointer, 4);
                        break;
                    case OpCodes.Iconst_5:
                        stack.Push(ref stackPointer, 5);
                        break;
                    #endregion Integer Constants

                    #region If Checks
                    case OpCodes.Ifeq:
                        {
                            int checkingNum = stack.Pop(ref stackPointer);
                            if (checkingNum == 0)
                            {
                                int branchOffset = code[programCount++] << 8;
                                branchOffset += code[programCount++];
                                programCount = oldProgramCount + branchOffset;
                            }
                            else
                            {
                                programCount += 2;
                            }
                        }
                        break;
                    case OpCodes.Ifne:
                        {
                            int checkingNum = stack.Pop(ref stackPointer);
                            if (checkingNum != 0)
                            {
                                int branchOffset = code[programCount++] << 8;
                                branchOffset += code[programCount++];
                                programCount = oldProgramCount + branchOffset;
                            }
                            else
                            {
                                programCount += 2;
                            }
                        }
                        break;
                    case OpCodes.Ifqt:
                        {
                            int checkingNum = stack.Pop(ref stackPointer);
                            if (checkingNum > 0)
                            {
                                int branchOffset = code[programCount++] << 8;
                                branchOffset += code[programCount++];
                                programCount = oldProgramCount + branchOffset;
                            }
                            else
                            {
                                programCount += 2;
                            }
                        }
                        break;
                    case OpCodes.Ifle:
                        {
                            int checkingNum = stack.Pop(ref stackPointer);
                            if (checkingNum <= 0)
                            {
                                int branchOffset = code[programCount++] << 8;
                                branchOffset += code[programCount++];
                                programCount = oldProgramCount + branchOffset;
                            }
                            else
                            {
                                programCount += 2;
                            }
                        }
                        break;
                    case OpCodes.If_icmpge:
                        {
                            int secondNum = stack.Pop(ref stackPointer);
                            int firstNum = stack.Pop(ref stackPointer);
                            if (firstNum >= secondNum)
                            {
                                int branchOffset = code[programCount++] << 8;
                                branchOffset += code[programCount++];
                                programCount = oldProgramCount + branchOffset;
                            }
                            else
                            {
                                programCount += 2;
                            }
                        }
                        break;
                    case OpCodes.If_icmpgt:
                        {
                            int secondNum = stack.Pop(ref stackPointer);
                            int firstNum = stack.Pop(ref stackPointer);
                            if (firstNum > secondNum)
                            {
                                int branchOffset = code[programCount++] << 8;
                                branchOffset += code[programCount++];
                                programCount = oldProgramCount + branchOffset;
                            }
                            else
                            {
                                programCount += 2;
                            }
                        }
                        break;
                    case OpCodes.If_icmpne:
                        {
                            int secondNum = stack.Pop(ref stackPointer);
                            int firstNum = stack.Pop(ref stackPointer);
                            if (firstNum != secondNum)
                            {
                                int branchOffset = code[programCount++] << 8;
                                branchOffset += code[programCount++];
                                programCount = oldProgramCount + branchOffset;
                            }
                            else
                            {
                                programCount += 2;
                            }
                        }
                        break;
                    #endregion If Checks

                    #region Static Fields
                    case OpCodes.GetStatic:
                        {
                            byte high = code[programCount++];
                            byte low = code[programCount++];
                            ushort val = (ushort)((high << 8) + low);
                            ConstantFieldRefInfo fieldRefInfo = (ConstantFieldRefInfo)ParentClassFile.Constants[val];
                            stack.Push(ref stackPointer, ParentClassFile.StaticFields[(fieldRefInfo.ClassName, fieldRefInfo.Name, fieldRefInfo.Type)].IntegerData);
                        }
                        break;
                    case OpCodes.PutStatic:
                        {
                            byte high = code[programCount++];
                            byte low = code[programCount++];
                            ushort val = (ushort)((high << 8) + low);
                            ConstantFieldRefInfo fieldRefInfo = (ConstantFieldRefInfo)ParentClassFile.Constants[val];

                            if (fieldRefInfo.Type == "I") //Load Int
                            {
                                ParentClassFile.StaticFields[(fieldRefInfo.ClassName, fieldRefInfo.Name, fieldRefInfo.Type)].IntegerData = stack.Pop(ref stackPointer);
                            }
                            else if (fieldRefInfo.Type == "J") //Load Long
                            {
                                (int highBytes, int lowBytes) = stack.PopLong(ref stackPointer);
                                ParentClassFile.StaticFields[(fieldRefInfo.ClassName, fieldRefInfo.Name, fieldRefInfo.Type)].DataLow = lowBytes;
                                ParentClassFile.StaticFields[(fieldRefInfo.ClassName, fieldRefInfo.Name, fieldRefInfo.Type)].DataHigh = highBytes;
                            }
                        }
                        break;
                    #endregion Static Fields

                    //Incramentors
                    case OpCodes.Iinc:
                        {
                            byte index = code[programCount++];
                            byte constVal = code[programCount++];
                            locals[index] += constVal;
                        }
                        break;

                    #region Method Calls
                    case OpCodes.Invokestatic:
                        {
                            programCount++;
                            byte index = code[programCount];
                            ConstantMethodRefInfo methodRef = (ConstantMethodRefInfo)ParentClassFile.Constants[index];
                            MethodInfo methodInfo = ParentClassFile.StaticMethods[(methodRef.NameAndType, methodRef.MethodType)];

                            //Get Param Count
                            (int paramCount, int retSize) = Utilities.FunctionParameterCount(methodInfo.Descriptor);
                            int[] funcParamaters = new int[paramCount];
                            for (int i = 0; i < paramCount; i++)
                            {
                                funcParamaters[i] = stack.Pop(ref stackPointer);
                            }

                            ReturnValue retValue = methodInfo.Frame.Execute(funcParamaters);
                            if (retValue.RetType != ReturnType.Void)
                            {
                                if (retValue.RetType == ReturnType.Int)
                                {
                                    stack.Push(ref stackPointer, (int)retValue);
                                }
                                else if(retValue.RetType == ReturnType.Long)
                                {
                                    stack.PushLong(ref stackPointer, (int)(retValue.LongValue >> 32), (int)(retValue.LongValue));
                                }
                            }
                            programCount++;
                        }
                        break;
                    case OpCodes.Invokespecial:
                        {
                            programCount++;
                            byte index = code[programCount];
                            int classIndex = stack.Pop(ref stackPointer);
                            JavaClassFile otherClass = ProgramHeap.GetObject(classIndex).ClassFile;
                            ConstantMethodRefInfo methodRef = (ConstantMethodRefInfo)ParentClassFile.Constants[index];
                            MethodInfo methodInfo = otherClass.InstanceMethods[(methodRef.NameAndType, methodRef.MethodType)];

                            //Get Param Count
                            (int paramCount, int retSize) = Utilities.FunctionParameterCount(methodInfo.Descriptor);
                            paramCount++;

                            //Push Params
                            int[] funcParamaters = new int[paramCount];
                            for (int i = 0; i < paramCount; i++)
                            {
                                funcParamaters[i] = stack.Pop(ref stackPointer);
                            }

                            ReturnValue retValue = methodInfo.Frame.Execute(funcParamaters);
                            if (retValue.RetType != ReturnType.Void)
                            {
                                if (retValue.RetType == ReturnType.Int)
                                {
                                    stack.Push(ref stackPointer, (int)retValue);
                                }
                                else if (retValue.RetType == ReturnType.Long)
                                {
                                    stack.PushLong(ref stackPointer, (int)(retValue.LongValue >> 32), (int)(retValue.LongValue));
                                }
                            }
                            programCount++;
                        }
                        break;
                    #endregion Method Calls

                    #region Objects
                    case OpCodes.New:
                        {
                            byte high = code[programCount++];
                            byte low = code[programCount++];
                            ushort val = (ushort)((high << 8) | low);

                            ConstantClassInfo classInfo = (ConstantClassInfo)ParentClassFile.Constants[val];
                            JavaClassFile NewJavaClass = new JavaClassFile($"{classInfo.Name}.class");
                            stack.Push(ref stackPointer, ProgramHeap.AddObject(new HeapObject(NewJavaClass)));
                        }
                        break;
                    #endregion

                    #region Returns
                    case OpCodes.Return:
                        return new ReturnValue(null);

                    case OpCodes.Ireturn:
                    case OpCodes.Freturn:
                    case OpCodes.Lreturn:
                        return new ReturnValue(stack.Pop(ref stackPointer));
                    #endregion Returns

                    default:
                        throw new InvalidOperationException($"Invalid OpCode! Hex Value: {opCode:X2}");
                }
            }
        }
    }
}