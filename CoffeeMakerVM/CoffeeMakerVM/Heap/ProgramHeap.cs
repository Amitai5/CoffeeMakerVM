using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeMakerVM.Heap
{
    public class ProgramHeap
    {
        private static List<HeapObject> heapStore = new List<HeapObject>()
        {
            null
        };

        public static int AddObject(HeapObject obj)
        {
            heapStore.Add(obj);
            return heapStore.Count - 1;
        }

        public static HeapObject GetObject(int idx)
        {
            return heapStore[idx];
        }
    }
}
