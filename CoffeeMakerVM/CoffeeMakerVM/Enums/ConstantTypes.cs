using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeMakerVM.Enums
{
    public enum ConstantTypes : byte
    {
        Utf8 = 1,
        Long = 5,
        Float = 4,
        Class = 7,
        String = 8,
        Double = 6,
        Integer = 3,
        FieldRef = 9,
        MethodRef = 10,
        MethodType = 16,
        NameAndType = 12,
        MethodHandle = 15,
        InvokeDynamic = 18,
        InterfaceMethodRef = 11
    }
}
