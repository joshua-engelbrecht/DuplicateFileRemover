using System;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace FileFunctions
{
    public class RemoveFiles
    {
        public void removeFiles(ArrayList deleteFiles, bool permanentDelete)
        {
            foreach (string fileName in deleteFiles)
            {
                if (!permanentDelete)
                {
                    FileSystem.DeleteFile(fileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                else
                {
                    FileSystem.DeleteFile(fileName, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                }
            }
        }
    }
}
