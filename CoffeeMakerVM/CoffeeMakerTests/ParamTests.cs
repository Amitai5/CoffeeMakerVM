using CoffeMakerVM;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CoffeeMakerTests
{
    public class ParamTests
    {
        [Theory]
        [InlineData("()V", 0, 0)]
        [InlineData("(DF)J", 3, 2)]
        [InlineData("([[I)S", 1, 1)]
        [InlineData("([[[[[[[I)S;", 1, 1)]
        [InlineData("(DFJ)LMANATEE;", 5, 1)]
        [InlineData("(LTestClass;I)I", 2, 1)]
        public void TestDescriptor(string descriptor, int expectedParams, int expectedReturn)
        {
            (int paramCount, int returnCount) = Utilities.FunctionParameterCount(descriptor);
            Assert.True(returnCount == expectedReturn, $"The Return Count {returnCount} Does Not Match The Expected {expectedReturn}");
            Assert.True(paramCount == expectedParams, $"The Param Count {paramCount} Does Not Match The Expected {expectedParams}");
        }
    }
}
