using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Filelib
{
    public class FileHelper
    {
        public static string GetFilePath(string rootPath,string fileName,params string[] folders)
        {
            string folderpath = rootPath;
            foreach(string fo in folders)
            {
                folderpath += "\\"+fo+"\\";
            }
            if(string.IsNullOrEmpty(folderpath))
            {
                return "";
            }
            if(!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
            string FilePath = folderpath+"\\"+ fileName;
            return FilePath;
        }
    }
}
