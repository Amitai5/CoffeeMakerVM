using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeMakerVM.ConstantPool.Classes
{
    public class ConstantClassInfo : ConstantPoolInfo
    {
        public ushort NameIndex { get; }
        public string Name { get; private set; }

        public ConstantClassInfo(ref ReadOnlySpan<byte> fileData)
            : base(ref fileData)
        {
            NameIndex = fileData.ReadUshort();
        }

        public override void UpdateInfo(ConstantPoolInfo[] constants)
        {
            Name = ((ConstantUtf8Info)constants[NameIndex]).StringData;
        }
    }
}