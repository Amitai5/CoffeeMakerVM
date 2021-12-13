using CoffeeMakerVM;
using CoffeeMakerVM.ClassInfos;
using CoffeeMakerVM.Enums;
using CoffeMakerVM.ConstantPool;
using CoffeMakerVM.ConstantPool.Classes;
using CoffeMakerVM.ConstantPool.Numerics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CoffeMakerVM
{
    public class JavaClassFile
    {
        public string ClassName { get; private set; }
        public string SuperClass { get; private set; }

        public ConstantPoolInfo[] Constants { get; private set; }
        protected ReadOnlyMemory<FieldInfo> Fields { get; private set; }
        protected ReadOnlyMemory<string> Interfaces { get; private set; }
        protected ReadOnlyMemory<MethodInfo> Methods { get; private set; }
        protected ReadOnlyMemory<AttributeInfo> Attributes { get; private set; }

        //Store Them Publically
        public Dictionary<(string name, string descriptor), MethodInfo> StaticMethods { get; private set; }
        public List<(string className, string name, string descriptor)> InstanceFields { get; private set; }
        public Dictionary<(string name, string descriptor), MethodInfo> InstanceMethods { get; private set; }
        public Dictionary<(string className, string name, string descriptor), FieldData> StaticFields { get; private set; }
        private readonly string BaseClassPath = $@"{Environment.CurrentDirectory.Split("CoffeeMakerVM")[0]}CoffeeMakerVM\JavaByteCode\build\classes\java\main";

        public JavaClassFile(string filePath)
        {
            filePath = $"{BaseClassPath}\\{filePath}";
            ReadOnlySpan<byte> fileData = File.ReadAllBytes(filePath);
            var magic = fileData.ReadUint();

            if(magic != 0xCAFEBABE)
            {
                throw new InvalidDataException("The Given JavaClassFile Is Not In Fact A Proper Java Class File!");
            }
            var minor = fileData.ReadUshort();
            var major = fileData.ReadUshort();

            ParseConstants(ref fileData);
            ushort accessFlags = fileData.ReadUshort();

            ushort thisClass = fileData.ReadUshort();
            ClassName = ((ConstantClassInfo)Constants[thisClass]).Name;
            
            ushort superClass = fileData.ReadUshort();
            if (superClass != 0)
            {
                SuperClass = ((ConstantClassInfo)Constants[superClass]).Name;
            }

            GetInterfaces(ref fileData);
            GetFields(ref fileData);
            GetMethods(ref fileData);
            GetAttributes(ref fileData);
                        //Get Static And Instance Methods
            StaticMethods = new Dictionary<(string name, string descriptor), MethodInfo>();
            InstanceMethods = new Dictionary<(string name, string descriptor), MethodInfo>();
            for (int i = 0; i < Methods.Length; i++)
            {
                MethodInfo mInfo = Methods.Span[i];
                if((mInfo.AccessFlags & 0x0008) == 0x0008)
                {
                    //Static Method
                    StaticMethods.Add((mInfo.Name, mInfo.Descriptor), mInfo);
                }
                else
                {
                    //Instance Method
                    InstanceMethods.Add((mInfo.Name, mInfo.Descriptor), mInfo);
                }
            }

            //Get Static And Instance Fields
            StaticFields = new Dictionary<(string className, string name, string descriptor), FieldData>();
            InstanceFields = new List<(string className, string name, string descriptor)>();
            for (int i = 0; i < Fields.Length; i++)
            {
                FieldInfo fInfo = Fields.Span[i];
                if ((fInfo.AccessFlags & 0x0008) == 0x0008)
                {
                    //Static Method
                    StaticFields.Add((ClassName, fInfo.Name, fInfo.Descriptor), new FieldData());
                }
                else
                {
                    //Instance Method
                    InstanceFields.Add((ClassName, fInfo.Name, fInfo.Descriptor));
                }
            }

            //Call The Init Function
            if (StaticMethods.ContainsKey(("<clinit>", "()V")))
            {
                StaticMethods[("<clinit>", "()V")].Frame.Execute();
            }
        }

        private void GetFields(ref ReadOnlySpan<byte> fileData)
        {
            ushort fieldCount = fileData.ReadUshort();
            FieldInfo[] fields = new FieldInfo[fieldCount];

            for (int i = 0; i < fieldCount; i++)
            {
                fields[i] = new FieldInfo(ref fileData, Constants);
            }
            Fields = fields;
        }
        private void GetMethods(ref ReadOnlySpan<byte> fileData)
        {
            ushort methodCount = fileData.ReadUshort();
            MethodInfo[] methods = new MethodInfo[methodCount];

            for (int i = 0; i < methodCount; i++)
            {
                methods[i] = new MethodInfo(ref fileData, this);
            }
            Methods = methods;
        }
        private void GetAttributes(ref ReadOnlySpan<byte> fileData)
        {
            ushort attributeCount = fileData.ReadUshort();
            AttributeInfo[] atrributes = new AttributeInfo[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                atrributes[i] = new AttributeInfo(ref fileData, Constants);
            }
            Attributes = atrributes;
        }
        private void GetInterfaces(ref ReadOnlySpan<byte> fileData)
        {
            ushort interfaceCount = fileData.ReadUshort();
            string[] interfaces = new string[interfaceCount];

            for (int i = 0; i < interfaceCount; i++)
            {
                interfaces[i] = ((ConstantFieldRefInfo)Constants[fileData.ReadUint()]).Name;
            }
            Interfaces = interfaces;
        }

        private void ParseConstants(ref ReadOnlySpan<byte> fileData)
        {
            var cpCount = fileData.ReadUshort();
            Constants = new ConstantPoolInfo[cpCount];

            //Get All Constants
            for(int i = 1; i < cpCount; i++)
            {
                byte tag = fileData[0];
                
                switch((ConstantTypes)tag)
                {
                    case ConstantTypes.Utf8:
                        Constants[i] = new ConstantUtf8Info(ref fileData);
                        break;
                    case ConstantTypes.Integer:
                        Constants[i] = new ConstantIntegerInfo(ref fileData);
                        break;
                    case ConstantTypes.Float:
                        Constants[i] = new ConstantFloatInfo(ref fileData);
                        break;
                    case ConstantTypes.Long:
                        Constants[i] = new ConstantLongInfo(ref fileData);
                        i++; //Skipping The Next Index Due To Java Spec
                        break;
                    case ConstantTypes.Double:
                        Constants[i] = new ConstantDoubleInfo(ref fileData);
                        i++; //Skipping The Next Index Due To Java Spec
                        break;
                    case ConstantTypes.Class:
                        Constants[i] = new ConstantClassInfo(ref fileData);
                        break;
                    case ConstantTypes.String:
                        Constants[i] = new ConstantStringInfo(ref fileData);
                        break;
                    case ConstantTypes.FieldRef:
                        Constants[i] = new ConstantFieldRefInfo(ref fileData);
                        break;
                    case ConstantTypes.MethodRef:
                        Constants[i] = new ConstantMethodRefInfo(ref fileData);
                        break;
                    case ConstantTypes.InterfaceMethodRef:
                        Constants[i] = new ConstantInterfaceMethodRefInfo(ref fileData);
                        break;
                    case ConstantTypes.NameAndType:
                        Constants[i] = new ConstantNameAndTypeInfo(ref fileData);
                        break;
                }
            }

            //Re-Update All The Constants
            for(int ii = 1; ii < cpCount; ii++)
            {
                Constants[ii]?.UpdateInfo(Constants);
            }
        }
    }
}