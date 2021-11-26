using CoffeeMakerVM.ClassInfos;
using System;

namespace CoffeMakerVM
{
    class Program
    {
        static void Main(string[] args)
        {
            JavaClassFile classFile = new JavaClassFile("Program.class");
            foreach ((string Name, string Descriptor) methodInfo in classFile.StaticMethods.Keys)
            {
                if (methodInfo.Name == "main" && methodInfo.Descriptor == "([Ljava/lang/String;)I")
                {
                    int retData = (int)classFile.StaticMethods[methodInfo].Frame.Execute();
                    Console.WriteLine(retData);
                    Console.ReadLine();
                }
            }
        }
    }
}