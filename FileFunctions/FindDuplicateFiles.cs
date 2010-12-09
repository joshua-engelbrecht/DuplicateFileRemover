using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using Hashing;

namespace FileFunctions
{
    public class FindDuplicateFiles
    {
        /// <summary>
        /// Find All the duplicate Files By CheckSum
        /// </summary>
        /// <param name="files">ArrayList of Files</param>
        /// <returns>ArrayList</returns>
        public ArrayList findDuplicatesByCheckSum(ArrayList files)
        {
            var duplicateFiles = new ArrayList();
            var count = 0;
            var duplicateId = 0;
            fileStruct previous = new fileStruct { checksum = "000" };

            foreach (fileStruct file in files)
            {
                if (String.Equals(file.checksum,previous.checksum))
                {
                    if (count == 0)
                    {
                        duplicateId++;
                        previous.duplicationNumber = duplicateId;
                        duplicateFiles.Add(previous);
                    }
                    file.duplicationNumber = duplicateId;
                    duplicateFiles.Add(file);
                    count++;
                }
                else
                {
                    previous = file;
                    count = 0;
                }

            }
            return duplicateFiles;
        }

        public ArrayList findDuplicatesByFileName(ArrayList files)
        {
            var duplicateFiles = new ArrayList();
            var count = 0;
            var duplicateId = 0;
            fileStruct previous = new fileStruct { fileName = "" };

            foreach (fileStruct file in files)
            {
                if (String.Equals(file.fileName, previous.fileName))
                {
                    if (count == 0)
                    {
                        duplicateId++;
                        previous.duplicationNumber = duplicateId;
                        duplicateFiles.Add(previous);
                    }
                    file.duplicationNumber = duplicateId;
                    duplicateFiles.Add(file);
                    count++;
                }
                else
                {
                    previous = file;
                    count = 0;
                }

            }
            return duplicateFiles;
        }
    }

}
