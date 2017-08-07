using System;
using System.IO;
using System.Text;

namespace Encodinglib
{
    public class EncodingType
    {
        public static Encoding GetFileEncodeType(string filename)
        {
            FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            BinaryReader br = new System.IO.BinaryReader(fs);
            Byte[] buffer = br.ReadBytes(2);
            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    br.Dispose();
                    br.Close();
                    fs.Dispose();
                    fs.Close();
                    return System.Text.Encoding.UTF8;
                }
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    br.Dispose();
                    br.Close();
                    fs.Dispose();
                    fs.Close();
                    return System.Text.Encoding.BigEndianUnicode;
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    br.Dispose();
                    br.Close();
                    fs.Dispose();
                    fs.Close();
                    return System.Text.Encoding.Unicode;
                }
                else
                {
                    br.Dispose();
                    br.Close();
                    fs.Dispose();
                    fs.Close();
                    return System.Text.Encoding.Default;
                }
            }
            else
            {
                br.Dispose();
                br.Close();
                fs.Dispose();
                fs.Close();
                return System.Text.Encoding.Default;
            }
          

        }
    }
}
