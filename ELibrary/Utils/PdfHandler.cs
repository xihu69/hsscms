using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ELibrary.Utils
{
    public class PdfHandler
    {
        //turn.js
        //方法一：从已有pdf中拷贝指定的页码范围到一个新的pdf文件中：    
        //1、使用pdfCopyProvider.AddPage()方法    
        public static void ExtractPages(string sourcePdfPath, string outputPdfPath, int startPage, int endPage)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;
            try
            {
                reader = new PdfReader(sourcePdfPath);
                sourceDocument = new Document(reader.GetPageSizeWithRotation(startPage));
                pdfCopyProvider = new PdfCopy(sourceDocument, File.Create(outputPdfPath));
                sourceDocument.Open();
                for (int i = startPage; i <= endPage; i++)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                    pdfCopyProvider.AddPage(importedPage);
                }
                sourceDocument.Close();
                reader.Close();
            }
            catch (Exception ex) { throw ex; }
        }
        public static void ExtractPages(Stream source, Stream output, int startPage, int endPage)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;
            try
            {
                reader = new PdfReader(source);
                sourceDocument = new Document(reader.GetPageSizeWithRotation(startPage));
                pdfCopyProvider = new PdfCopy(sourceDocument, output);
                sourceDocument.Open();
                for (int i = startPage; i <= endPage; i++)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                    pdfCopyProvider.AddPage(importedPage);
                }
            }
            catch (Exception ex) { throw ex; }
            finally {
                sourceDocument?.Close();
                reader?.Close();
            }
        }
        public static (int TotalPage, string FirstName, int BlockCount, int BlockSize) Peging2(string source, string outputFmt, int size = 20)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            try
            {
                reader = new PdfReader(source);
                var num = reader.NumberOfPages;
                var blockCount = num / size;
                if (num % size > 0)
                    blockCount++;
                // var file = new FileInfo(source);
                var dir = Directory.CreateDirectory(source + "_ls");

                sourceDocument = new Document(reader.GetPageSizeWithRotation(1));
                var fname = "";
                pdfCopyProvider = WriteCopy(File.Create(Path.Combine(dir.FullName, "1.pdf")), 1, 20, reader);
                Task.Run(() => {
                    try
                    {
                        BlockCreate(size, reader, sourceDocument, num, blockCount, dir);
                        sourceDocument.Close();
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }

                });


                return (num, fname, blockCount, size);// (TotalPage:num,FirstName: fname, BlockCount:blockCount,BlockSize:size);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void BlockCreate(int size, PdfReader reader, Document sourceDocument, int num, int blockCount, DirectoryInfo dir, int start = 0)
        {
            for (int i = start; i < blockCount; i++)
            {
                var begin = i * size + 1;
                var end = begin + size - 1;
                if (end > num)
                    end = num;
                var blockName = Path.Combine(dir.FullName, i + 1 + ".pdf");
                WriteCopy(File.Create(blockName), begin, end, reader);
            }
        }
        public static PdfSplitInfo? PegingFirstAsyn(string source) {
            var aw = new ManualResetEventSlim();
            PdfSplitInfo finfo =null;
            var progress = new Progress<PdfSplitInfo>(p =>
            {
                if (p.NowBlockNum == 1)
                {
                    finfo = p;
                    aw.Set();
                }
            });
            Task.Run(() => PegingAsyn(source, progress));
            aw.Wait(2000);
            return finfo;
        }
        public static void PegingAsyn(string source, IProgress<PdfSplitInfo> progress) {
            try
            {
                var size = 20;
                var reader = new PdfReader(source);
                var num = reader.NumberOfPages;
                var blockCount = num / size;
                if (num % size > 0)
                    blockCount++;
                // var file = new FileInfo(source);
                var dir = Directory.CreateDirectory(source + "_ls");

                progress.Report(new PdfSplitInfo { BlockCount = blockCount, BlockSize = size, FirstName = null, NowBlockNum = 0, TotalPage = num });
               // var sourceDocument = new Document(reader.GetPageSizeWithRotation(1));
                var fname = "";
                for (int i = 0; i < blockCount; i++)
                {
                    var begin = i * size + 1;
                    var end = begin + size - 1;
                    if (end > num)
                        end = num;
                    var blockName = Path.Combine(dir.FullName, i + 1 + ".pdf");
                    if (i == 0)
                        fname = blockName;
                    var fileSt = File.Create(blockName);
                    var pdfCopyProvider = WriteCopy(fileSt, begin, end, reader, p=>p.AddKeywords($"[{num}-{size}-{i+1}]"));  //【总页-块大小-当前块】
                    pdfCopyProvider.Close();
                    fileSt.Close();
                    progress.Report(new PdfSplitInfo { BlockCount = blockCount, BlockSize = size, FirstName = fname, NowBlockNum = i + 1, TotalPage = num });
                }

                //if (sourceDocument.IsOpen())
                //    sourceDocument.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static (int TotalPage, string FirstName, int BlockCount, int BlockSize) Peging(string source, string outputFmt, int size=20) {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;
          
            try
            {
                reader = new PdfReader(source);
                var num = reader.NumberOfPages;
                var blockCount = num / size;
                if (num % size > 0)
                    blockCount++;
               // var file = new FileInfo(source);
                var dir = Directory.CreateDirectory(source + "_ls");
              
                sourceDocument = new Document(reader.GetPageSizeWithRotation(1));
                var fname = "";
                for (int i = 0; i < blockCount; i++)
                {
                    var begin = i * size+1;
                    var end = begin + size-1;
                    if (end > num)
                        end = num;
                    var blockName = Path.Combine(dir.FullName, i + 1 + ".pdf");
                    if (i == 0)
                        fname = blockName;
                    var fileSt = File.Create(blockName);
                    pdfCopyProvider = WriteCopy(fileSt, begin, end, reader);
                    //fileSt.Close();
                    //pdfCopyProvider.Close();
                }
                sourceDocument.Close();
                reader.Close();
                return (num, fname, blockCount, size);// (TotalPage:num,FirstName: fname, BlockCount:blockCount,BlockSize:size);
            }
            catch (Exception ex) { 
                throw ex;
            }
        }
       
        public class PdfSplitInfo { 
            public string? FirstName { get; set; }
            public int TotalPage { get; set; }
            public int BlockCount { get; set; }
            public int BlockSize { get; set; }
            public int NowBlockNum { get; set; }
        }

        private static PdfCopy WriteCopy(Stream output, int begin,int end, PdfReader reader,Action<Document> openAfter=null)
        {
           var sourceDocument = new Document(reader.GetPageSizeWithRotation(begin));
            
            PdfCopy pdfCopyProvider = new PdfCopy(sourceDocument, output);
            sourceDocument.Open();
            if (openAfter!=null)
                openAfter(sourceDocument);
            for (int i = begin; i <= end; i++)
            {
                var importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                pdfCopyProvider.AddPage(importedPage);
            } 
            sourceDocument.Close();
            return pdfCopyProvider;
        }




        //二、将已有pdf文件中 不连续 的页拷贝至新的pdf文件中。其中需要拷贝的页码存于数组 int[] extractThesePages中
        public void ExtractPages(string sourcePdfPath, string outputPdfPath, int[] extractThesePages)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;
            try
            {
                reader = new PdfReader(sourcePdfPath);
                sourceDocument = new Document(reader.GetPageSizeWithRotation(extractThesePages[0])); pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));
                sourceDocument.Open();
                foreach (int pageNumber in extractThesePages)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, pageNumber); pdfCopyProvider.AddPage(importedPage);
                }
                sourceDocument.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //2、使用 AddTemplate()方法
        private void copypdf(int StartPage, int EndPage, string file1, string splitFileName)
        {//将file1中页码StartPage到EndPage的文件拷贝至splitFileName
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(file1); Document document = new Document(reader.GetPageSizeWithRotation(StartPage));
            //创建document对象                               
            // string splitFileName = "D:\itextsharp_例子\split_pdf_ceshi4.pdf";              
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(splitFileName, FileMode.Create)); //实例化document对象               
            document.Open();
            int rotation;
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage page;              // for (int i = 1; i <= startPage; i++)              while (StartPage <= EndPage)              {                  
            document.SetPageSize(reader.GetPageSizeWithRotation(StartPage)); document.NewPage();
            page = writer.GetImportedPage(reader, StartPage);
            rotation = reader.GetPageRotation(StartPage);
            if (rotation == 90 || rotation == 270)
            {                      //document.NewPage();                        
                if (rotation == 90)
                {
                    cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(StartPage).Height);
                }
                if (rotation == 270)
                {
                    cb.AddTemplate(page, 0, 1.0F, -1.0F, 0, reader.GetPageSizeWithRotation(StartPage).Width, 0);
                }
            }
            else
            {
                cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
            }
            StartPage++;

            try
            {
                document.Close();
                reader = null;
            }
            catch (Exception ex)
            {

            }
        }




    }
}
