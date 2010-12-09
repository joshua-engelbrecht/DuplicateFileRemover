using System;
using System.Collections;

namespace FileFunctions
{
    /// <summary>
    /// Compare files By CheckSum
    /// </summary>
    public class CompareFilesByCheckSum : IComparer
    {
        public int Compare(object obj1, object obj2)
        {
            fileStruct a, b;
            a = (fileStruct)obj1;
            b = (fileStruct)obj2;
            return a.checksum.CompareTo(b.checksum);
        }
    }

    /// <summary>
    /// Compare Files by CreationDate
    /// </summary>
    public class CompareFilesByDate : IComparer
    {
        public int Compare(object obj1, object obj2)
        {
            fileStruct a, b;
            a = (fileStruct)obj1;
            b = (fileStruct)obj2;
            return a.creationDate.CompareTo(b.creationDate);
        }
    }

    /// <summary>
    /// Compare Files by File Name
    /// </summary>
    public class CompareFilesByName : IComparer
    {
        public int Compare(object obj1, object obj2)
        {
            fileStruct a, b;
            a = (fileStruct)obj1;
            b = (fileStruct)obj2;
            return a.fileName.CompareTo(b.fileName);
        }
    }
}
