using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeMakerVM.Enums
{
    public enum ParamTypes : ushort
    {
        Int = 'I',
        Long = 'J',
        Byte = 'B',
        Char = 'C',
        Short = 'S',
        Float = 'F',
        Double = 'D',
        Boolean = 'Z',
        ClassName = 'L',
        EndOfParamName = ';',
        ArrayReference = '[',
        EndOfDescriptor = ')',
    }
}
