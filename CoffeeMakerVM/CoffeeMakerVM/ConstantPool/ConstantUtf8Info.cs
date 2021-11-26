using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeMakerVM.ConstantPool
{
    public class ConstantUtf8Info : ConstantPoolInfo
    {
        public string StringData { get; }

        public ConstantUtf8Info(ref ReadOnlySpan<byte> fileData) 
            : base(ref fileData)
        {
            ushort length = fileData.ReadUshort();
            ReadOnlySpan<byte> bytes = fileData.Slice(0, length);
            fileData = fileData.Slice(length);

            //Get The String Data
            StringData = Encoding.UTF8.GetString(bytes);
        }

        public override void UpdateInfo(ConstantPoolInfo[] constants)
        {
            
        }
    }
}
