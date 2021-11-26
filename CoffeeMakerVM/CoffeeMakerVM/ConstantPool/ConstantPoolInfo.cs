using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeMakerVM.ConstantPool
{
    public abstract class ConstantPoolInfo
    {
        public byte Tag { get; }
        public ConstantPoolInfo(ref ReadOnlySpan<byte> data)
        {
            Tag = data.ReadByte();
        }
        public abstract void UpdateInfo(ConstantPoolInfo[] constants);
    }
}