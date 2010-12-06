using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using Hashing;

namespace FileFunctions
{
    public class fileFunctions
    {
        public ArrayList getFiles(string pattern, string directory, SearchOption traverse)
        {
            var listOfFiles = new ArrayList();
            var di = new DirectoryInfo(directory);
            var files = di.GetFiles(pattern, traverse);
            foreach (var file in files){
                var findHash = new findMD5();
                var md5 = findHash.getFilesMD5Hash(file.FullName);
                var fStruct = new fileStruct{ checksum=md5, fileName=file.Name, fullFileName=file.FullName};
                listOfFiles.Add(fStruct);
            }
            return listOfFiles;
        }

        public ArrayList sortFiles(ArrayList listOFiles)
        {
            var cmp = new CompFiles();
            listOFiles.Sort(cmp);
            return listOFiles;
        }
    }
}
