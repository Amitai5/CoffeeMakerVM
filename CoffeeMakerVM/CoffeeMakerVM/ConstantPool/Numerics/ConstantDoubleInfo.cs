using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CoffeMakerVM.ConstantPool.Numerics
{
    public class ConstantDoubleInfo : ConstantPoolInfo
    {
        public double DoubleValue { get; }
        public long LongValue { get; }

        public int HighBytes { get; }
        public int LowBytes { get; }

        public ConstantDoubleInfo(ref ReadOnlySpan<byte> fileData) 
            : base(ref fileData)
        {
            HighBytes = (int)fileData.ReadUint();
            LowBytes = (int)fileData.ReadUint();

            //Get Acutal Long Value
            ulong lValue = (ulong)HighBytes << 32;
            lValue |= (uint)LowBytes;
            LongValue = (long)lValue;

            //Get Double Value
            long localVal = LongValue;
            DoubleValue = Unsafe.As<long, double>(ref localVal);
        }

        public override void UpdateInfo(ConstantPoolInfo[] constants)
        {

        }
    }
}
