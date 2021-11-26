using CoffeMakerVM;
using CoffeMakerVM.ConstantPool;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeMakerVM.ClassInfos
{
    public class FieldInfo
    {
        public string Name { get; }
        public string Descriptor { get; }
        public ushort AccessFlags { get; }
        public ReadOnlyMemory<AttributeInfo> Attributes { get; }

        public FieldInfo(ref ReadOnlySpan<byte> fileData, ReadOnlySpan<ConstantPoolInfo> constants)
        {
            AccessFlags = fileData.ReadUshort();
            Name = ((ConstantUtf8Info)constants[fileData.ReadUshort()]).StringData;
            Descriptor = ((ConstantUtf8Info)constants[fileData.ReadUshort()]).StringData;

            ushort attributeCount = fileData.ReadUshort();
            AttributeInfo[] attributes = new AttributeInfo[attributeCount];
            for(int i = 0; i < attributeCount; i++)
            {
                attributes[i] = new AttributeInfo(ref fileData, constants);
            }
            Attributes = attributes;
        }
    }
}
