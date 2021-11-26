using CoffeeMakerVM.ClassInfos;
using CoffeMakerVM;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeMakerVM.Heap
{
    public class HeapObject
    {
        public JavaClassFile ClassFile { get; }
        public Dictionary<(string className, string name, string descriptor), FieldData> InstanceFields { get; private set; }

        public HeapObject(JavaClassFile clsFile)
        {
            ClassFile = clsFile;
            InstanceFields = new Dictionary<(string className, string name, string descriptor), FieldData>(ClassFile.InstanceFields.Count);

            //Copy Over The Values
            foreach(var fInfo in ClassFile.InstanceFields)
            {
                InstanceFields.Add(fInfo, new FieldData());
            }
        }
    }
}
