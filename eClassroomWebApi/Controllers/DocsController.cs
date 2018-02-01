using eClassroomWebAPI.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using Spire.Doc;
using Spire.Pdf;
using Spire.Presentation;

namespace eClassroomWebAPI.Controllers
{
    public class DocsController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //public HttpResponseMessage Get(string fileName)
        //{
        //    HttpResponseMessage result = null;

        //    DirectoryInfo directoryInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/App_Data/" + UploadFolder));
        //    FileInfo foundFileInfo = directoryInfo.GetFiles().Where(x => x.Name == fileName).FirstOrDefault();
        //    if (foundFileInfo != null)
        //    {
        //        FileStream fs = new FileStream(foundFileInfo.FullName, FileMode.Open);

        //        result = new HttpResponseMessage(HttpStatusCode.OK);
        //        result.Content = new StreamContent(fs);
        //        result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        //        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //        result.Content.Headers.ContentDisposition.FileName = foundFileInfo.Name;
        //    }
        //    else
        //    {
        //        result = new HttpResponseMessage(HttpStatusCode.NotFound);
        //    }

        //    return result;
        //}

        [HttpPost]
        public HttpResponseMessage List(UInt32 folderId)
        {           
            string strSQL = "";
            try
            {
                SqlConnection dbConn = new SqlConnection(DbConnectionString);
                dbConn.Open();
                if (folderId > 0)
                    strSQL = String.Format("select a.docId, b.originalFileName, b.pageCount, b.pageInfos from coursewares a inner join documents b on a.docId=b.docId where a.folderId={0}", folderId);
                else
                    strSQL = "select docId, originalFileName, pageCount, pageInfos from documents";
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, dbConn);
                da.Fill(ds);
                ds.Tables[0].TableName = "docInfos";
                return Request.CreateResponse(HttpStatusCode.OK, ds);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateCourseware(Byte action, string docId, UInt32 folderId)
        {
            var ht = new Hashtable();
            ht["docId"] = docId;
            ht["folderId"] = folderId;
            string strMsg = "";
            if (folderId > 0)
            {
                string strSQL = "";
                SqlConnection dbConn = new SqlConnection(DbConnectionString);
                try
                {
                    dbConn.Open();
                    switch (action)
                    {
                        case 0:
                            strSQL = String.Format("insert into coursewares (docId, folderId) values ('{0}',{1})", docId, folderId);
                            break;
                        case 1:
                            strSQL = String.Format("delete from coursewares where docId='{0}' and folderId={1}", docId, folderId);
                            break;
                    }                    
                    SqlCommand cmd = new SqlCommand(strSQL, dbConn);
                    cmd.ExecuteNonQuery();
                    ht["code"] = 0;
                    strMsg = "更新成功";
                }
                catch (Exception ex)
                {
                    ht["code"] = 1;
                    strMsg = ex.Message;                    
                }
            }
            else
            {
                ht["code"] = 2;
                strMsg = "参数非法";
            }
            ht["message"] = strMsg;
            return Request.CreateResponse(HttpStatusCode.OK, ht);
        }

        [HttpPost]
        public Task<Hashtable> Upload(string folderid, string token, string filehash)
        {
            //const string fileTypes = "doc,docx,pdf,ppt,pptx";
            var ht = new Hashtable();
            try
            {
                //uploadFolderPath variable determines where the files should be temporarily uploaded into server. 
                //Remember to give full control permission to IUSER so that IIS can write file to that folder.
                string uploadFolderPath = HostingEnvironment.MapPath("~/App_Data/" + UploadFolder);

                //如果路径不存在，创建路径
                if (!Directory.Exists(uploadFolderPath))
                    Directory.CreateDirectory(uploadFolderPath);

                string conversionFolderPath = HostingEnvironment.MapPath("~/App_Data/" + ConversionFolder);
                if (!Directory.Exists(conversionFolderPath))
                    Directory.CreateDirectory(conversionFolderPath);

                string orignalFilename;
                string localFileName;
                string docId;
                string uploadedFileHash;
                string pageInfos;

                int pageCount = 0;

                if (Request.Content.IsMimeMultipartContent()) //If the request is correct, the binary data will be extracted from content and IIS stores files in specified location.
                {
                    var streamProvider = new WithExtensionMultipartFormDataStreamProvider(uploadFolderPath, 262144);
                    var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<Hashtable>(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            ht["code"] = 1;
                            ht["message"] = "传输中断";
                            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ht));
                        }

                        if (streamProvider.FileData.Count > 0)
                        {
                            var mfd = streamProvider.FileData[0];

                            orignalFilename = mfd.Headers.ContentDisposition.FileName.TrimStart('"').TrimEnd('"');
                            pageInfos = "";
                            localFileName = mfd.LocalFileName;
                            string sExt = Path.GetExtension(localFileName).ToLower();

                            var fileInfo = new FileInfo(localFileName);

                            uploadedFileHash = HashFile(localFileName);

                            if (uploadedFileHash == filehash)
                            {
                                docId = Path.GetFileNameWithoutExtension(localFileName);//should also the fileId;
                                ht["docId"] = docId;
                                ht["orignalFilename"] = orignalFilename;

                                try
                                {
                                    //EncoderParameters eps = null;
                                    //ImageCodecInfo ici = GetJpegImageCodecInfo();
                                    //if (ici != null)
                                    //{
                                    //    eps = new EncoderParameters(1);
                                    //    eps.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                                    //}

                                    switch (sExt)
                                    {
                                        case ".doc":
                                        case ".docx":
                                            Document doc = new Document(localFileName);
                                            pageCount = doc.PageCount;
                                            for (int i = 0; i < doc.PageCount; i++)
                                            {
                                                string convFileName = String.Format("{0}\\{1}_{2}.png", conversionFolderPath, docId, i + 1);
                                                using (Image image = doc.SaveToImages(i, Spire.Doc.Documents.ImageType.Bitmap))
                                                {
                                                    string s = String.Format("{0}-{1}|", image.Width, image.Height);
                                                    pageInfos += s;
                                                    image.Save(convFileName, ImageFormat.Png);
                                                }
                                            }
                                            break;
                                        case ".pdf":
                                            PdfDocument pd = new PdfDocument(localFileName);
                                            pageCount = pd.Pages.Count;
                                            for (int i = 0; i < pd.Pages.Count; i++)
                                            {
                                                string convFileName = String.Format("{0}\\{1}_{2}.png", conversionFolderPath, docId, i+1);
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
                                            Presentation pp = new Presentation(localFileName, Spire.Presentation.FileFormat.Auto);
                                            pageCount = pp.Slides.Count;
                                            for (int i = 0; i < pp.Slides.Count; i++)
                                            {
                                                string convFileName = String.Format("{0}\\{1}_{2}.png", conversionFolderPath, docId, i+1);
                                                using (Image image = pp.Slides[i].SaveAsImage())
                                                {
                                                    string s = String.Format("{0}-{1}|", image.Width, image.Height);
                                                    pageInfos += s;
                                                    image.Save(convFileName, ImageFormat.Png);
                                                }
                                            }
                                            break;
                                    }

                                    ht["code"] = 0;
                                    ht["message"] = "上传转码成功";
                                    ht["pageCount"] = pageCount;
                                    ht["pageInfos"] = pageInfos;

                                    string strSQL;
                                    SqlCommand cmd = null;
                                    SqlConnection dbConn = new SqlConnection(DbConnectionString);
                                    try
                                    {
                                        dbConn.Open();
                                        strSQL = String.Format("insert into documents (docId, originalFileName, localFileName, pageCount, pageInfos) values ('{0}','{1}','{2}',{3}, '{4}')",
                                            docId, orignalFilename, localFileName, pageCount, pageInfos);
                                        cmd = new SqlCommand(strSQL, dbConn);
                                        cmd.ExecuteNonQuery();

                                        UInt32 folderId;
                                        if (UInt32.TryParse(folderid, out folderId) && folderId > 0)
                                        {
                                            strSQL = String.Format("insert into coursewares (docId, folderId) values ('{0}',{1})",
                                                docId, folderId);
                                            cmd = new SqlCommand(strSQL, dbConn);
                                            cmd.ExecuteNonQuery();
                                        }
                                        dbConn.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        ht["message"] = "上传转码成功,但写入数据库异常:" + ex.Message;
                                    }
                                    dbConn.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    ht["code"] = 2;
                                    ht["message"] = "转码失败:" + ex.Message;
                                    ht["pageCount"] = 0;
                                }
                            }
                            else
                            {
                                ht["code"] = 3;
                                ht["message"] = "文件特征码校验失败";
                                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ht));
                            }
                        }
                        return ht;
                    });

                    return task;
                }
                else
                {
                    ht["code"] = 4;
                    ht["message"] = "不支持的媒体格式";
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ht));
                }
            }
            catch (Exception ex)
            {
                ht["code"] = -1;
                ht["message"] = ex.Message;
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ht));
            }
        }

        protected static string DbConnectionString = WebConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;

        static protected ImageCodecInfo GetJpegImageCodecInfo()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == "image/jpeg")
                {
                    return codec;
                }
            }
            return null;
        }

        private const string UploadFolder = "uploads";
        private const string ConversionFolder = "conversions";

        private string HashFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                byte[] hashBytes = HashData(fs);
                fs.Close();
                return ByteArrayToHexString(hashBytes);            
            }
            return string.Empty;
        }

        /// <summary>
        /// 计算哈希值
        /// </summary>
        /// <param name="stream">要计算哈希值的 Stream</param>
        /// <param name="algName">算法:sha1,md5</param>
        /// <returns>哈希值字节数组</returns>
        private byte[] HashData(System.IO.Stream stream)
        {
            System.Security.Cryptography.HashAlgorithm algorithm = System.Security.Cryptography.MD5.Create();
            return algorithm.ComputeHash(stream);
        }

        /// <summary>
        /// 字节数组转换为16进制表示的字符串
        /// </summary>
        private string ByteArrayToHexString(byte[] buf)
        {
            return BitConverter.ToString(buf).Replace("-", "").ToLower();
        }
    }
}
