using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeMakerVM.ConstantPool.Classes
{
    public class ConstantMethodRefInfo : ConstantPoolInfo
    {
        public ushort ClassIndex { get; }
        public ushort NameAndTypeIndex { get; }

        public string ClassName { get; private set; }
        public string MethodType { get; private set; }
        public string NameAndType { get; private set; }

        public ConstantMethodRefInfo(ref ReadOnlySpan<byte> fileData) 
            : base(ref fileData)
        {
            ClassIndex = fileData.ReadUshort();
            NameAndTypeIndex = fileData.ReadUshort();
        }

        public override void UpdateInfo(ConstantPoolInfo[] constants)
        {
            ((ConstantClassInfo)constants[ClassIndex]).UpdateInfo(constants);
            ClassName = ((ConstantClassInfo)constants[ClassIndex]).Name;

            ConstantNameAndTypeInfo nameAndType = (ConstantNameAndTypeInfo)constants[NameAndTypeIndex];
            nameAndType.UpdateInfo(constants);
            NameAndType = nameAndType.Name;
            MethodType = nameAndType.Type;
        }
    }
}
