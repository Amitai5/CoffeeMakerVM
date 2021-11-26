using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeMakerVM.ConstantPool.Classes
{
    public class ConstantInterfaceMethodRefInfo : ConstantPoolInfo
    {
        public ushort ClassIndex { get; }
        public ushort NameAndTypeIndex { get; }
        
        public string ClassName { get; private set; }
        public string MethodType { get; private set; }
        public string NameAndType { get; private set; }

        public ConstantInterfaceMethodRefInfo(ref ReadOnlySpan<byte> fileData) 
            : base(ref fileData)
        {
            ClassIndex = fileData.ReadUshort();
            NameAndTypeIndex = fileData.ReadUshort();
        }

        public override void UpdateInfo(ConstantPoolInfo[] constants)
        {
            ClassName = ((ConstantUtf8Info)constants[ClassIndex]).StringData;

            ConstantNameAndTypeInfo nameAndType = (ConstantNameAndTypeInfo)constants[NameAndTypeIndex];
            nameAndType.UpdateInfo(constants);
            NameAndType = nameAndType.Name;
            MethodType = nameAndType.Type;
        }
    }
}
