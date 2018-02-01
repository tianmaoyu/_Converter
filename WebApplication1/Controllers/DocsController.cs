using System;
using System.Web.Http;
using AClassroom.Entity.Enum;
using AClassroom.Core;
using AClassroom.DocConverter.Models;
using AClassroom.Server;
using System.Web;

namespace AClassroom.DocConverter.Controllers
{


    public class DocsController : ApiController
    {

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileMd5">文件md5</param>
        /// <param name="onwerType">文件所属项目</param>
        /// <param name="clientType">文件上传客户端类型</param>
        /// <returns></returns>
        [HttpPost]
        public CommonResult<Entity.Document> Upload(string fileMd5, DocmentOnwerTypeEnum onwerType, UploadClientTypeEnum clientType)
        {
            var result = new CommonResult<Entity.Document>();
            try
            {
                var files = HttpContext.Current.Request.Files;
                var file = files[files.AllKeys[0]];
                var documentServer = new DocumentServer();
                var _document = documentServer.GetByMd5(fileMd5);//通过 md5 查找一遍 有不再进行 上传，保存等
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
                document.DownloadUrl = uploadFile.RelativePath;
                document.Enabled = true;
                document.FileMD5 = uploadFile.FileMd5;
                document.FolderName = uploadFile.FileMd5;
                document.OriginalFileName = uploadFile.OriginalFileName;
                document.PhysicalPath = uploadFile.SaveFolder;
                document.UploadClientType = (int)clientType;
                document.UploadClientTypeName = EnumHelper.GetEnumDescription(clientType);
                var _pageInfos = string.Empty;
                var _pageCount = 0;
                if (uploadFile.NeedConvert) //是否需要转换
                {
                    DocumentConverter.Convert(document.PhysicalPath, uploadFile.TempFileName, ref _pageInfos, ref _pageCount);
                }
                document.PageInfos = _pageInfos;
                document.PageCount = _pageCount;
                document.Id = documentServer.Insert(document);
                result.ResultEnum = CommonResultEnum.Success;
                result.Data = document;
                return result;
            }
            catch (Exception ex)
            {
                result.ResultEnum = CommonResultEnum.SystemError;
                result.Message = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 通过 document fileMd5 得 document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public CommonResult<Entity.Document> GetDocument(string fileMd5)
        {
            var result = new CommonResult<Entity.Document>();
            DocumentServer documentServer = new DocumentServer();
            var docmentInfo = documentServer.GetByMd5(fileMd5);
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
