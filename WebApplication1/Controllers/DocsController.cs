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
using AClassroom.Entity.Enum;
using AClassroom.Core;
using AClassroom.DocConverter.Infrastructure;
using AClassroom.DocConverter.Models;
using AClassroom.Entity;
using AClassroom.Server;
using System.Web;

namespace AClassroom.DocConverter.Controllers
{
    public class DocsController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

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

                            uploadedFileHash = FileHelper.GetMd5Hash(localFileName);

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

                                    NewMethod(conversionFolderPath, localFileName, docId, ref pageInfos, ref pageCount, sExt);

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

        private static void NewMethod(string conversionFolderPath, string localFileName, string docId, ref string pageInfos, ref int pageCount, string sExt)
        {
            switch (sExt)
            {
                case ".doc":
                case ".docx":
                    Spire.Doc.Document doc = new Spire.Doc.Document(localFileName);
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
                        string convFileName = String.Format("{0}\\{1}_{2}.png", conversionFolderPath, docId, i + 1);
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
                        string convFileName = String.Format("{0}\\{1}_{2}.png", conversionFolderPath, docId, i + 1);
                        using (Image image = pp.Slides[i].SaveAsImage())
                        {
                            string s = String.Format("{0}-{1}|", image.Width, image.Height);
                            pageInfos += s;
                            image.Save(convFileName, ImageFormat.Png);
                        }
                    }
                    break;
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


        [HttpPost]
        public CommonResult<Entity.Document> Upload(string fileMd5, DocmentOnwerTypeEnum onwerType, UploadClientTypeEnum clientType)
        {
            var result = new CommonResult<Entity.Document>();
            var files = HttpContext.Current.Request.Files;
            var file = files[files.AllKeys[0]];
            //通过 md5 查找一遍 有不再进行 上传，保存等
            var documentServer = new DocumentServer();
            var _document = documentServer.GetByMd5(fileMd5);
            if (_document != null && _document.Id > 0)
            {
                result.ResultEnum = CommonResultEnum.Success;
                result.Data = _document;
                return result;
            }
            var uploadFile = new UploadFile(file, onwerType, clientType);
            var document = new Entity.Document();
            document.CreateTime = DateTime.Now;
            document.DocmentOnwerType = (int)onwerType;
            document.DocmentOnwerTypeName = EnumHelper.GetEnumDescription(onwerType);
            document.DocmentType = (int)uploadFile.DocmentType;
            document.DocmentTypeName = EnumHelper.GetEnumDescription(uploadFile.DocmentType);

            document.DownloadUrl = uploadFile.FileMd5;
            document.Enabled = true;

            //是否需要转换
            if (uploadFile.NeedConvert)
            {
                DocumentConverter.Convert()
            }
          





            return result;
        }

        /// <summary>
        /// 通过 docment id 得 docment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public CommonResult<Entity.Document> GetDocment(int id)
        {
            var result = new CommonResult<Entity.Document>();
            DocumentServer docmentServer = new DocumentServer();
            var docmentInfo = docmentServer.GetById(id);
            if (docmentInfo.Id > 0)
            {
                result.ResultEnum = CommonResultEnum.Success;
                result.Data = docmentInfo;
            }
            else
            {
                result.ResultEnum = CommonResultEnum.NoData;
            }
            return result;
        }

    }
}
