using CoffeMakerVM;
using CoffeMakerVM.ConstantPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoffeeMakerVM.ClassInfos
{
    public class MethodInfo
    {
        public string Name { get; }
        public MethodFrame Frame { get; }
        public string Descriptor { get; }
        public ushort AccessFlags { get; }
        public ReadOnlyMemory<AttributeInfo> Attributes { get; }

        public MethodInfo(ref ReadOnlySpan<byte> fileData, JavaClassFile classFile)
        {
            AccessFlags = fileData.ReadUshort();
            Name = ((ConstantUtf8Info)classFile.Constants[fileData.ReadUshort()]).StringData;
            Descriptor = ((ConstantUtf8Info)classFile.Constants[fileData.ReadUshort()]).StringData;

            ushort attributeCount = fileData.ReadUshort();
            AttributeInfo[] attributes = new AttributeInfo[attributeCount];
            for (int i = 0; i < attributeCount; i++)
            {
                attributes[i] = new AttributeInfo(ref fileData, classFile.Constants);
            }
            Attributes = attributes;

            //Get The Code Attribute
            var codeAttribute = attributes.Where(x => x.Name == "Code").FirstOrDefault();
            if(codeAttribute != null)
            {
                Frame = new MethodFrame(codeAttribute.Info, classFile);
            }
        }
    }
}
