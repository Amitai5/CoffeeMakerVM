using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CoffeMakerVM.ConstantPool.Numerics
{
    public class ConstantLongInfo : ConstantPoolInfo
    {
        public long LongValue { get; }
        public int HighBytes { get; }
        public int LowBytes { get; }

        public ConstantLongInfo(ref ReadOnlySpan<byte> fileData) 
            : base(ref fileData)
        {
            HighBytes = (int)fileData.ReadUint();
            LowBytes = (int)fileData.ReadUint();

            //Get Acutal Long Value
            ulong lValue = (ulong)HighBytes << 32;
            lValue |= (uint)LowBytes;
            LongValue = (long)lValue;
        }

        public override void UpdateInfo(ConstantPoolInfo[] constants)
        {
            
        }
    }
}
