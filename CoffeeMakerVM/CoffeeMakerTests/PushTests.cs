using CoffeeMakerVM;
using CoffeMakerVM;
using System;
using Xunit;

namespace CoffeeMakerTests
{
    public class PushTests
    {
        [Theory]
        [InlineData(0x00, 0x00, 0)]
        [InlineData(0x00, 0x01, 1)]
        [InlineData(0xFF, 0xFF, -1)]
        [InlineData(0x0B, 0xB8, 3000)]
        [InlineData(0x80, 0x00, short.MinValue)]
        [InlineData(0x7F, 0xFF, short.MaxValue)]
        public void SiPushTest(byte highData, byte lowData, short retData)
        {
            //Create The Frame To Test
            MethodFrame frame = new MethodFrame(new byte[]
            {
                //1 Stack, 0 Locals // 4 Bytes Of Code
                0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04,
                0x11, highData, lowData, 0xAC
            }, null);

            int retVal = (int)frame.Execute();
            Assert.True(retData == retVal, $"The Return Value \"{retData}\" Did Not Match The Predicted Value Of \"{retVal}\"");
        }

        [Theory]
        [InlineData(0x00, 0x00, 0)]
        [InlineData(0x00, 0x01, 1)]
        [InlineData(0xFF, 0xFF, -1)]
        public void PushAndPopLongTest(int low, int high, long result)
        {
            Span<int> stack = stackalloc int[2];
            int stackPointer = 0;

            //Push The Long Value
            Utilities.PushLong(ref stack, ref stackPointer, high, low);
            Assert.True(stack[0] == high, $"The Pushed High Value Of \"{stack[0]}\" Did Not Match The Predicted High Value Of \"{high}\"");

            //Assert.True(retData == retVal, $"The Return Value \"{retData}\" Did Not Match The Predicted Value Of \"{retVal}\"");
        }
    }
}
