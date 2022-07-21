using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using System;
using System.IO;
using System.Text;
namespace ELibrary.Utils
{
    /// <summary> 
    /// 获取文件的编码格式 
    /// </summary> 
    public class GuessEncoding
    {
        static GuessEncoding() {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public static void test()
        {
            
            var vv = GuessEncoding.GetType(@"E:\tmpfile\xxx\ccc.csv", Encoding.GetEncoding("gb2312"));
            var txt = File.ReadAllText(@"E:\tmpfile\xxx\ccc.csv", vv);
            Console.WriteLine(txt);
        }
        /// <summary> 
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
        /// </summary> 
        /// <param name=“FILE_NAME“>文件路径</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetType(string FILE_NAME, Encoding encodingDef = null)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs, encodingDef);
            fs.Close();
            return r;
        }
        byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
        byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
        byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
        /// <summary> 
        /// 通过给定的文件流，判断文件的编码类型 
        /// </summary> 
        /// <param name=“fs“>文件流</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetType(FileStream fs, Encoding encodingDef = null)
        {
            Encoding reVal = encodingDef ?? Encoding.Default;
            BinaryReader r = new BinaryReader(fs, reVal, true);
            int i = fs.Length > 1024 ? 1024 : (int)fs.Length;
            byte[] ss = r.ReadBytes(i);
            if ((ss.Length > 2 && ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss.Length > 2 && ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss.Length > 2 && ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            else if (IsUTF8Bytes(ss))
                reVal = Encoding.UTF8;
            r.Close();
            fs.Position = 0;
            return reVal;

        }

        /// <summary> 
        /// 判断是否是不带 BOM 的 UTF8 格式 
        /// </summary> 
        /// <param name=“data“></param> 
        /// <returns></returns> 
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1&&data.Length<1000)
            {
                return false;
                //throw new Exception("非预期的byte格式");
            }
            return true;
        }

    }

}



