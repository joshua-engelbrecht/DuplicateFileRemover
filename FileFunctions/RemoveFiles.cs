using System;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace FileFunctions
{
    public class RemoveFiles
    {
        public void removeFiles(IList deleteFiles, bool permanentDelete)
        {
            foreach (fileStruct file in deleteFiles)
            {
                if (!permanentDelete)
                {
                    FileSystem.DeleteFile(file.fullFileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                else
                {
                    FileSystem.DeleteFile(file.fullFileName, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                }
            }
        }
    }
}
