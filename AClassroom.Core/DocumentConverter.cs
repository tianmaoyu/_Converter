using Spire.Pdf;
using Spire.Presentation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AClassroom.Core
{
    public class DocumentConverter
    {
        /// <summary>
        /// 转换为 png 图片 并保存
        /// </summary>
        /// <param name="saveDirectory">要保存的路径</param>
        /// <param name="fileName">转化的文件名称</param>
        /// <param name="pageInfos">转化后的图片信息</param>
        /// <param name="pageCount">文档页数</param>
        public static bool Convert(string saveDirectory, string fileName, ref string pageInfos, ref int pageCount)
        {
            var extName = Path.GetExtension(fileName);
            var fileTitle = Path.GetFileNameWithoutExtension(fileName);
            switch (extName)
            {
                case ".doc":
                case ".docx":
                    Spire.Doc.Document doc = new Spire.Doc.Document(fileName);
                    pageCount = doc.PageCount;
                    for (int i = 0; i < doc.PageCount; i++)
                    {
                        string convFileName = String.Format("{0}\\{1}_{2}.png", saveDirectory, fileTitle, i + 1);
                        using (Image image = doc.SaveToImages(i, Spire.Doc.Documents.ImageType.Bitmap))
                        {
                            string s = String.Format("{0}-{1}|", image.Width, image.Height);
                            pageInfos += s;
                            image.Save(convFileName, ImageFormat.Png);
                        }
                    }
                    break;
                case ".pdf":
                    PdfDocument pd = new PdfDocument(fileName);
                    pageCount = pd.Pages.Count;
                    for (int i = 0; i < pd.Pages.Count; i++)
                    {
                        string convFileName = String.Format("{0}\\{1}_{2}.png", saveDirectory, fileTitle, i + 1);
                        using (Image image = pd.SaveAsImage(i))
                        {
                            string s = String.Format("{0}-{1}|", image.Width, image.Height);
                            pageInfos += s;
                            image.Save(convFileName, ImageFormat.Png);
                        }
                    }
                    break;
                case ".ppt":
                case ".pptx":
                    Presentation pp = new Presentation(fileName, Spire.Presentation.FileFormat.Auto);
                    pageCount = pp.Slides.Count;
                    for (int i = 0; i < pp.Slides.Count; i++)
                    {
                        string convFileName = String.Format("{0}\\{1}_{2}.png", saveDirectory, fileTitle, i + 1);
                        using (Image image = pp.Slides[i].SaveAsImage())
                        {
                            string s = String.Format("{0}-{1}|", image.Width, image.Height);
                            pageInfos += s;
                            image.Save(convFileName, ImageFormat.Png);
                        }
                    }
                    break;
            }
            return true;
        }
    }
}
