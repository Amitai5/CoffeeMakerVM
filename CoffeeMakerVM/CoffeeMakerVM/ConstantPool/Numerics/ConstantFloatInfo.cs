using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CoffeMakerVM.ConstantPool.Numerics
{
    public class ConstantFloatInfo : ConstantPoolInfo
    {
        public int Bytes { get; }
        public float FloatValue { get; }

        public ConstantFloatInfo(ref ReadOnlySpan<byte> fileData)
            : base(ref fileData)
        {
            Bytes = (int)fileData.ReadUint();

            //Get Acutal Float Value
            int localBytes = Bytes;
            FloatValue = Unsafe.As<int, float>(ref localBytes);
        }
        
        public override void UpdateInfo(ConstantPoolInfo[] constants)
        {

        }
    }
}
