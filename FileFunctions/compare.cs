using System;
using System.Collections;

namespace FileFunctions
{
    public class CompFiles : IComparer
    {
        public int Compare(object obj1, object obj2)
        {
            fileStruct a, b;
            a = (fileStruct)obj1;
            b = (fileStruct)obj2;
            return a.checksum.CompareTo(b.checksum);
        }
    }
}
