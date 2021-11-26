using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeMakerVM.ConstantPool.Classes
{
    public class ConstantFieldRefInfo : ConstantPoolInfo
    {
        public ushort ClassIndex { get; }
        public ushort NameAndTypeIndex { get; }

        public string Name { get; private set; }
        public string Type { get; private set; }
        public string ClassName { get; private set; }

        public ConstantFieldRefInfo(ref ReadOnlySpan<byte> fileData) 
            : base(ref fileData)
        {
            ClassIndex = fileData.ReadUshort();
            NameAndTypeIndex = fileData.ReadUshort();
        }

        public override void UpdateInfo(ConstantPoolInfo[] constants)
        {
            ((ConstantClassInfo)constants[ClassIndex]).UpdateInfo(constants);
            ClassName = ((ConstantClassInfo)constants[ClassIndex]).Name;

            ConstantNameAndTypeInfo NameAndTypeInfo = (ConstantNameAndTypeInfo)constants[NameAndTypeIndex];
            NameAndTypeInfo.UpdateInfo(constants);
            Name = NameAndTypeInfo.Name;
            Type = NameAndTypeInfo.Type;
        }
    }
}
