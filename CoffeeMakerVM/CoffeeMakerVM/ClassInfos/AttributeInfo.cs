using CoffeMakerVM;
using CoffeMakerVM.ConstantPool;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeMakerVM.ClassInfos
{
    public class AttributeInfo
    {
        public string Name { get; }
        public ReadOnlyMemory<byte> Info { get; }

        public AttributeInfo(ref ReadOnlySpan<byte> fileData, ReadOnlySpan<ConstantPoolInfo> constants)
        {
            Name = ((ConstantUtf8Info)constants[fileData.ReadUshort()]).StringData;
            byte[] info = new byte[fileData.ReadUint()];

            fileData.Slice(0, info.Length).CopyTo(info.AsSpan());
            fileData = fileData.Slice(info.Length);
            Info = info;
        }
    }
}
