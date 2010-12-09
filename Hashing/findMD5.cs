using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Hashing
{
    public class findMD5
    {
        /// <summary>
        /// method for getting a files MD5 hash, say for
        /// a checksum operation
        /// </summary>
        /// <param name="file">the file we want the has from</param>
        /// <returns>MD5 of File</returns>
        /// 
        public string getFilesMD5Hash(string file)
        {
            //MD5 hash provider for computing the hash of the file
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            //open the file
            FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);

            //calculate the files hash
            md5.ComputeHash(stream);

            //close our stream
            stream.Close();

            //byte array of files hash
            byte[] hash = md5.Hash;

            //string builder to hold the results
            StringBuilder sb = new StringBuilder();

            //loop through each byte in the byte array
            foreach (byte b in hash)
            {
                //format each byte into the proper value and append
                //current value to return value
                sb.Append(string.Format("{0:X2}", b));
            }

            //return the MD5 hash of the file
            return sb.ToString();
        }
    }
}
