using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeMakerVM.ConstantPool.Numerics
{
    public class ConstantIntegerInfo : ConstantPoolInfo
    {
        public int Bytes { get; }

        public ConstantIntegerInfo(ref ReadOnlySpan<byte> fileData) 
            : base(ref fileData)
        {
            Bytes = (int)fileData.ReadUint();
        }

        public override void UpdateInfo(ConstantPoolInfo[] constants)
        {

        }
    }
}
