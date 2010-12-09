using System;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace FileFunctions
{
    public class RemoveFiles
    {
        /// <summary>
        /// Remove the files from the system
        /// </summary>
        /// <param name="file">File to be removed</param>
        /// <param name="permanentDelete">Move to RecycleBin or Just Delete</param>
        public void removeFiles(fileStruct file, bool permanentDelete)
        {
            if (!permanentDelete)
            {
                if(FileSystem.FileExists(file.fullPath))
                    FileSystem.DeleteFile(file.fullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            else
            {
                if (FileSystem.FileExists(file.fullPath))
                    FileSystem.DeleteFile(file.fullPath, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
            }
        }
    }
}
