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
        private ArrayList duplicationList = new ArrayList();

        /// <summary>
        /// Gets the List of files with a specific filter in a specific directory
        /// </summary>
        /// <param name="pattern">Pattern of files to find</param>
        /// <param name="directory">Which Directory to look</param>
        /// <param name="traverse">How to Traverse Subdirectories</param>
        /// <returns>ArrayList of Files</returns>
        public ArrayList getFiles(string pattern, string directory, SearchOption traverse)
        {
            var listOfFiles = new ArrayList();
            var di = new DirectoryInfo(directory);
            var files = di.GetFiles(pattern, traverse);
            foreach (var file in files){
                var findHash = new findMD5();
                var md5 = findHash.getFilesMD5Hash(file.FullName);
                var fStruct = new fileStruct{ checksum=md5, fileName=file.Name, fullPath=file.FullName};
                listOfFiles.Add(fStruct);
            }
            return listOfFiles;
        }

        /// <summary>
        /// Sort the files by the fileStruct.checksum field
        /// </summary>
        /// <param name="listOFiles">List of files to sort</param>
        /// <returns>ArrayList</returns>
        public ArrayList sortFilesByCheckSum(ArrayList listOFiles)
        {
            var cmp = new CompareFilesByCheckSum();
            listOFiles.Sort(cmp);
            return listOFiles;
        }
        /// <summary>
        /// Find Files to select
        /// </summary>
        /// <param name="filesToDelete">A list of Files Found that can be deleted</param>
        /// <param name="oldest">Are we looking for the oldest.  True to select the oldest, False to select the newest</param>
        /// <returns>IList of files</returns>
        public IList selectFiles(IList filesToDelete, bool oldest)
        {
            var upperLimit = 0;
            var startPosition = 1;

            var _cmpByDate = new CompareFilesByDate();
            var toReturn = new ArrayList();

            foreach(fileStruct file in filesToDelete){
                if (duplicationList.Count == 0)
                {
                    duplicationList.Add(file);
                    continue;
                }
                var previous = (fileStruct)duplicationList[0];
                if(previous.duplicationNumber == file.duplicationNumber)
                {
                    duplicationList.Add(file);
                }
                else
                {
                    duplicationList.Sort(_cmpByDate);
                    //We want the oldest files, i.e. first on in the sorted list
                    if (oldest)
                    {
                        upperLimit = duplicationList.Count;
                        startPosition = 1;
                    }
                    //We want the newest files, i.e. last on in the sorted list
                    else
                    {
                        startPosition = 0;
                        upperLimit = duplicationList.Count - 1;
                    }
                    for(var i = startPosition; i < upperLimit; i++){
                        toReturn.Add(duplicationList[i]);
                    }
                    duplicationList.Clear();
                    duplicationList.Add(file);
                }
            }
            duplicationList.Sort(_cmpByDate);
            //We want the oldest files, i.e. first on in the sorted list
            if (oldest)
            {
                upperLimit = duplicationList.Count;
                startPosition = 1;
            }
            //We want the newest files, i.e. last on in the sorted list
            else
            {
                startPosition = 0;
                upperLimit = duplicationList.Count - 1;
            }
            for (var i = startPosition; i < upperLimit; i++)
            {
                toReturn.Add(duplicationList[i]);
            }
            duplicationList.Clear();
            return toReturn;
        }
    }
}
