using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeMakerVM.ConstantPool
{
    public class ConstantNameAndTypeInfo : ConstantPoolInfo
    {
        ushort NameIndex { get; }
        ushort DescriptorIndex { get; }

        public string Name { get; private set; }
        public string Type { get; private set; }

        public ConstantNameAndTypeInfo(ref ReadOnlySpan<byte> fileData) 
            : base(ref fileData)
        {
            NameIndex = fileData.ReadUshort();
            DescriptorIndex = fileData.ReadUshort();
        }

        public override void UpdateInfo(ConstantPoolInfo[] constants)
        {
            Name = ((ConstantUtf8Info)constants[NameIndex]).StringData;
            Type = ((ConstantUtf8Info)constants[DescriptorIndex]).StringData;
        }
    }
}
