using CoffeeMakerVM.Enums;
using System;

namespace CoffeeMakerVM
{
    public class ReturnValue
    {
        public long? LongValue { get; private set; }
        public ReturnType RetType { get; private set; }
        public int? IntegerValue { get; private set; }

        public ReturnValue(object retData, bool isReference = false)
        {
            //Get Object Type
            Type returnType = retData?.GetType();
            TypeCode retTypeCode = Type.GetTypeCode(returnType);

            //Case Item
            switch (retTypeCode)
            {
                case TypeCode.Int32:
                    RetType = ReturnType.Int;
                    IntegerValue = (int)retData;
                    break;
                case TypeCode.Int64:
                    LongValue = (long)retData;
                    RetType = ReturnType.Long;
                    break;
                case TypeCode.Empty:
                    LongValue = null;
                    IntegerValue = null;
                    RetType = ReturnType.Void;
                    break;
                default:
                    throw new InvalidCastException($"The Specified Type \"{RetType}\" Is Not Supported!");
            }

            //Check For References
            if (isReference)
            {
                RetType = ReturnType.Reference;
            }
        }

        public static explicit operator int(ReturnValue returnValue)
        {
            return returnValue.IntegerValue.Value;
        }

        public static explicit operator long(ReturnValue returnValue)
        {
            return returnValue.LongValue.Value;
        }
    }
}
