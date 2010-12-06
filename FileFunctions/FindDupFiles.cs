using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using Hashing;

namespace FileFunctions
{
    class FindDupFiles
    {
        public ArrayList dupFiles;
        
        public void findDups(foundFiles[] files)
        {
            var table = new Hashtable();
            dupFiles = new ArrayList();
            var dupNumber = 1;
            foreach (var item in files)
            {
                if (!table.Contains(item.checksum))
                {
                    table.Add(item.checksum, 0);
                }
                var count = (int)table[item.checksum] + 1;
                table[item.checksum] = count;
                if (count >= 2)
                {
                    var dupFile = new foundFiles { fileName = item.fileName, 
                        checksum = item.checksum, 
                        fullFileName = item.fullFileName, 
                        duplicationNumber = dupNumber++};
                    dupFiles.Add(dupFile);
                }
            }
        }

    }

}
