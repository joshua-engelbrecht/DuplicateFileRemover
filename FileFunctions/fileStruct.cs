using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileFunctions
{
    public class fileStruct
    {
        /// <summary>
        /// Name of the file
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// The CheckSum of the file
        /// </summary>
        public string checksum { get; set; }
        /// <summary>
        /// Full Path of the file
        /// </summary>
        public string fullPath { get; set; }
        /// <summary>
        /// Duplication Identifier
        /// </summary>
        public int duplicationNumber { get; set; }
        /// <summary>
        /// File Creation Date
        /// </summary>
        public DateTime creationDate { get; set; }
    }
}
