using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
namespace ELibrary.Utils
{



    /*
     * 问题：效率不是很高，简单读取速度20M/s左右
     * 追踪toString占用时间较多，考虑一次解析一行数据，然后截取
     */
    public class CsvReader
    {
        private readonly TextReader reader;
        public StringBuilder stb = new StringBuilder();

        private const char Separator = ',';
        private const char NewLine = '\n';
        private const char NewLineBefore = '\r';
        private const char StringSign = '\"';
        private const int ReadEnd = -1;
        private int nowToken = 0;  //-1表示数据全部读取完毕
                                   //str++: 缓存满，逗号，换行，引号
                                   //,a"\n
        private int nowRowIndex = -1;

        public int NowRowIndex
        {
            get { return nowRowIndex; }
        }
        public CsvReader(Stream stream,Encoding encoding= null)
        {
            if(encoding==null)
                encoding = Encoding.UTF8;
            reader = new StreamReader(stream, encoding); // .GetEncoding("gb2312")
        }
        public CsvReader(TextReader tr)
        {
            reader = tr;
        }

        public List<string[]> ReadCsv()
        {
            var list = new List<string[]>();
            while (nowToken != ReadEnd)
            {
                list.Add(ReadRow());
            }
            return list;
        }

        public string[] ReadRow()
        {
            lock (this)
            {
                var list = new List<string>();
                int i = 0;
                string itemBef = null;
                do
                {
                   var  item = ReadItem();
                    if (item == null)
                        break;
                    list.Add(item);
                    itemBef = item;
                    nowRowIndex++;
                } while (nowToken != ReadEnd && nowToken != NewLine);
                if(list.Count==0)
                    return null;
                list[list.Count-1]=itemBef.TrimEnd(NewLineBefore);
                return list.ToArray();
            }
        }

        private string ReadItem()
        {
            int chr = reader.Read();
            if (chr < 0)
            {
                nowToken = ReadEnd;
                return null;
            }
            var nChar = (char)chr;

            if (chr == Separator)
            {
                nowToken = Separator;
                return "";
            }
            if (chr == NewLine)
            {
                nowToken = NewLine;
                return null;
            }
            stb.Clear();

            if (chr == StringSign)
            {
                nowToken = StringSign;
                return ParserString();
            }

            stb.Append(nChar);
            return charAppend();
        }

        private string ParserString()
        {
            int chr;
            while ((chr = reader.Read()) >= 0)
            {
                char nChar = (char)chr;
                if (nChar == StringSign)
                {
                    chr = reader.Read();
                    if (chr < 0)
                    {
                        nowToken = ReadEnd;
                        return stb.ToString();
                    }
                    else if (chr == StringSign)
                    {
                        stb.Append(StringSign);
                    }
                    else
                    {
                        nowToken = 0;
                        return charAppend(chr);
                    }
                }
                else
                    stb.Append(nChar);
            }
            return stb.ToString();
        }
        private string charAppend(int chr)
        {
            if (chr == Separator)
            {
                nowToken = Separator;
                return stb.ToString();
            }
            else if (chr == NewLine)
            {
                nowToken = NewLine;
                return stb.ToString();
            }
            return charAppend();
        }
        private string charAppend()
        {
            int chr;
            while ((chr = reader.Read()) >= 0)
            {
                if (chr == Separator)
                {
                    nowToken = Separator;
                    return stb.ToString();
                }
                else if (chr == NewLine)
                {
                    nowToken = NewLine;
                    return stb.ToString();
                }
                stb.Append((char)chr);
            }
            nowToken = ReadEnd;
            return stb.ToString();
        }
    }

    public class CsvWriter
    {
        private const char Separator = ',';
        private const char NewLine = '\n';
        private const char StringSign = '\"';
        readonly char[] changeChar = new char[] { Separator, StringSign, NewLine };
        readonly TextWriter writer;
        public CsvWriter(Stream stream)
        {
            writer = new StreamWriter(stream, Encoding.UTF8);
        }
        public CsvWriter(TextWriter tw)
        {
            writer = tw;
        }
        /// <summary>
        /// Writes a single row to a CSV file.
        /// </summary>
        /// <param name="row">The row to be written</param>
        public void WriteRow(IEnumerable<IComparable> row)
        {
            var isFirst = true;
            foreach (var value in row)
            {
                if (isFirst)
                    isFirst = false;
                else
                    writer.Write(Separator);
                if (value == null)
                    continue;
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (value is string)
                {
                    var str = (string)value;
                    StrWrite(str);
                }
                else if (value is IConvertible || value is Guid)
                {
                    writer.Write(value);
                }
                else
                {
                    var str = value.ToString();
                    StrWrite(str);
                }
            }
            writer.WriteLine();
        }

        private void StrWrite(string? str)
        {
            if (str.IndexOfAny(changeChar) != -1)
            {
                writer.Write('\"');
                writer.Write(str.Replace("\"", "\"\""));
                writer.Write('\"');
            }
            else
                writer.Write(str);
        }

        public void WriteAllLines(IEnumerable<IEnumerable<IComparable>> data, IEnumerable<string> head = null)
        {
            if (head != null)
                WriteRow(head);
            foreach (var row in data)
            {
                WriteRow(row);
            }
            writer.Flush();
        }

        public static string ToCsv(IEnumerable<IEnumerable<string>> data)
        {
            var sw = new StringWriter();
            new CsvWriter(sw).WriteAllLines(data);
            return sw.ToString();
        }
        public static void ToCsvFile(string path, IEnumerable<IEnumerable<string>> data)
        {
            using (var sw = File.Open(path, FileMode.OpenOrCreate))
            {
                new CsvWriter(sw).WriteAllLines(data);
            }
        }

        public static string ToCsv(DataTable data, bool isHead = false)
        {
            string[][] array = DataToArr(data);
            using (var sw = new StringWriter())
            {
                if (isHead)
                {
                    //var head = data.Columns.Cast<DataColumn>().Select(p => p.ColumnName).ToArray();
                    string[] head = GetTableColNames(data);
                    new CsvWriter(sw).WriteAllLines(array, head);
                }
                else
                    new CsvWriter(sw).WriteAllLines(array);
                return sw.ToString();
            }
        }

        public static string[] GetTableColNames(DataTable data)
        {
            var head = new string[data.Columns.Count];
            for (int i = 0; i < data.Columns.Count; i++)
            {
                var item = data.Columns[i];
                head[i] = item.ColumnName;
            }

            return head;
        }

        public static string[][] DataToArr(DataTable data)
        {
            var array = new string[data.Rows.Count][];
            for (int i = 0; i < data.Rows.Count; i++)
            {
                var item = data.Rows[i];
                var arr = item.ItemArray;
                var tmpArray = new string[arr.Length];
                for (int j = 0; j < arr.Length; j++)
                {
                    tmpArray[j] = arr[j] as string;
                }
                array[i] = tmpArray;
            }

            return array;
        }

        public static void ToCsvFile(string path, DataTable data, bool isHead = false)
        {
            //var array = data.Rows.Cast<DataRow>().Select(p => p.ItemArray.Select(x => x as string).ToArray()).ToArray();
            string[][] array = DataToArr(data);
            using (var sw = File.Open(path, FileMode.OpenOrCreate))
            {
                if (isHead)
                {
                    //var head = data.Columns.Cast<DataColumn>().Select(p => p.ColumnName).ToArray();
                    string[] head = GetTableColNames(data);
                    new CsvWriter(sw).WriteAllLines(array, head);
                }
                else
                    new CsvWriter(sw).WriteAllLines(array);
            }
        }
    }
}


