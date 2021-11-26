using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeMakerVM.ConstantPool
{
    public class ConstantStringInfo : ConstantPoolInfo
    {
        public ushort StringIndex { get; }
        public string StringData { get; private set; }

        public ConstantStringInfo(ref ReadOnlySpan<byte> fileData) 
            : base(ref fileData)
        {
            StringIndex = fileData.ReadUshort();
        }

        public override void UpdateInfo(ConstantPoolInfo[] constants)
        {
            StringData = ((ConstantUtf8Info)constants[StringIndex]).StringData;
        }
    }
}