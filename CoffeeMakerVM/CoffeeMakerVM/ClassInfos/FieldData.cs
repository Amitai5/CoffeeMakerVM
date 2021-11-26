using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeMakerVM.ClassInfos
{
    public class FieldData
    {
        public int DataHigh { get; set; }
        public int DataLow { get; set; }
        public int IntegerData
        {
            get
            {
                return DataLow;
            }
            set
            {
                DataLow = value;
                DataHigh = 0x0000;
            }
        }
        public long LongData
        {
            get
            {
                return (DataHigh << 32) | DataLow;
            }
        }
    }
}
