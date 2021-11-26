using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeMakerVM.Enums
{
    public enum OpCodes : byte
    {
        Nop = 0x00,
        Ldc = 0x12,
        Dup = 0x59,

        //Pushes
        Bipush = 0x10,
        Sipush = 0x11,

        //Objects
        New = 0xBB,

        //Int Constants
        Iconst_m1 = 0x02,
        Iconst_0 = 0x03,
        Iconst_1 = 0x04,
        Iconst_2 = 0x05,
        Iconst_3 = 0x06,
        Iconst_4 = 0x07,
        Iconst_5 = 0x08,

        //Long Constants
        Lconst_0 = 0x09,
        Lconst_1 = 0x0A,

        //Load Longs,
        Ldc2_w = 0x14,
        Lload_0 = 0x1E,

        //Load Ints
        Iload_0 = 0x1A,
        Iload_1 = 0x1B,
        Iload_2 = 0x1C,
        Iload_3 = 0x1D,

        //Load Addresses
        aload_0 = 0x2A,
        aload_1 = 0x2B,
        aload_2 = 0x2C,
        aload_3 = 0x2D,

        //Store Ints
        Istore_0 = 0x3B,
        Istore_1 = 0x3C,
        Istore_2 = 0x3D,
        Istore_3 = 0x3E,

        //Incramentors
        Iinc = 0x84,

        //Stack Commands
        Pop = 0x57,

        //Int Math
        Iadd = 0x60,
        Isub = 0x64,
        Imul = 0x68,
        Idiv = 0x6C,
        Irem = 0x70,

        //Long Math
        Ladd = 0x61,
        Lsub = 0x65,
        Lmul = 0x69,
        Ldiv = 0x6D,

        //Comparing Codes
        Lcmp = 0x94,

        //If Checks
        Ifeq = 0x99,
        Ifne = 0x9A,
        Ifqt = 0x9D,
        Ifle = 0x9E,
        If_icmpne = 0xA0,
        If_icmpge = 0xA2,
        If_icmpgt = 0xA3,

        //Shifts
        ishr = 0x7A,

        //Returns
        Ireturn = 0xAC,
        Lreturn = 0xAD,
        Freturn = 0xAE,
        Return = 0xB1,

        //Static Fields
        GetStatic = 0xB2,
        PutStatic = 0xB3,

        //Method Calls
        Invokestatic = 0xB8,
        Invokespecial = 0xB7
    }
}