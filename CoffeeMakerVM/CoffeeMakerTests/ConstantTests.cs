using CoffeMakerVM;
using CoffeMakerVM.ConstantPool.Numerics;
using System;
using Xunit;

namespace CoffeeMakerTests
{
    public class ConstantTests
    {
        [Fact]
        public void TestDoubleInfo()
        {
            double myDouble = 1.25f;
            ReadOnlySpan<byte> doubleInfoData = new byte[]
            {
                6, 0x3F, 0xF4, 0, 0, 0, 0, 0, 0
            };

            //Try The Class And Assert
            ConstantDoubleInfo doubleInfo = new ConstantDoubleInfo(ref doubleInfoData);
            Assert.Equal(myDouble, doubleInfo.DoubleValue, 15);
        }

        [Fact]
        public void TestFloatInfo()
        {
            float myFloat = 1.90625f;
            ReadOnlySpan<byte> floatInfoData = new byte[]
            {
                6, 0x3F, 0xF4, 0, 0, 0, 0, 0, 0
            };

            //Try The Class And Assert
            ConstantFloatInfo floatInfo = new ConstantFloatInfo(ref floatInfoData);
            Assert.Equal(myFloat, floatInfo.FloatValue, 15);
        }

        [Fact]
        public void TestLongInfo()
        {
            long myLong = 4608308318706860032;
            ReadOnlySpan<byte> longInfoData = new byte[]
            {
                6, 0x3F, 0xF4, 0, 0, 0, 0, 0, 0
            };

            //Try The Class And Assert
            ConstantLongInfo myLongInfo = new ConstantLongInfo(ref longInfoData);
            Assert.Equal(myLong, myLongInfo.LongValue);
        }
    }
}
