using AClassroom.Core;
using AClassroom.Entity.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AClassroom.DocConverter.Models
{
    public class UploadFile
    {
        /// <summary>
        /// 临时保存文件夹
        /// </summary>
        private static readonly string _uploadFolderTemp = ConfigConstant.UploadFolderTemp;

        /// <summary>
        /// 要保存的目录
        /// </summary>
        private static readonly string _uploadFolder = ConfigConstant.UploadFolderRoot;

        /// <summary>
        /// 需要转换的文档后缀的类型
        /// </summary>
        private static List<string> _extensionList = new List<string>() { ".ppt", ".pptx", ".pdf", ".doc", ".docx"};

        /// <summary>
        /// 临时保存文件带path
        /// </summary>
        public string TempFileName { private set; get; }

        /// <summary>
        /// 相对路径
        /// </summary>
        public string RelativePath { private set; get; }

        /// <summary>
        /// 是否需要转化
        /// </summary>
        public bool NeedConvert
        {
            get
            {
                var fileExtension = Path.GetExtension(this.OriginalFileName);
                return UploadFile._extensionList.Contains(fileExtension);
            }
        }

        /// <summary>
        /// 文件 md5
        /// </summary>
        public string FileMd5 { private set; get; }

        /// <summary>
        /// 要保存的文件
        /// </summary>
        public string SaveFolder { private set; get; }

        /// <summary>
        /// 原文件名称
        /// </summary>
        public string OriginalFileName { private set; get; }

        /// <summary>
        /// 文档类型
        /// </summary>
        public DocmentTypeEnum DocmentType { private set; get; }

        /// <summary>
        /// 保存文件，初始保存的路径
        /// </summary>
        /// <param name="file"></param>
        /// <param name="onwerType"></param>
        /// <param name="clientType"></param>
        public UploadFile(HttpPostedFile file, DocmentOnwerTypeEnum onwerType, UploadClientTypeEnum clientType)
        {
            if (!Directory.Exists(UploadFile._uploadFolderTemp))
                Directory.CreateDirectory(UploadFile._uploadFolderTemp);
            if (!string.IsNullOrEmpty(file.FileName))
            {
                this.TempFileName = Path.Combine(UploadFile._uploadFolderTemp, file.FileName);
                file.SaveAs(this.TempFileName);
                this.OriginalFileName = file.FileName;
                this.FileMd5 = FileHelper.GetMd5Hash(this.TempFileName);
                this.SaveFolder = this.CreateOrGetPath(onwerType, clientType, this.FileMd5);
            }
        }


        /// <summary>
        /// 删除临时文件
        /// </summary>
        public void Delete()
        {
            File.Delete(this.TempFileName);
        }

        /// <summary>
        /// 根据 所属的项目类型，上传的客户端来源类型，创建不同的文件件保存路径
        /// </summary>
        /// <param name="onwerType">所属项目</param>
        /// <param name="clientType">上传的客户端类型</param>
        /// <returns></returns>
        private string CreateOrGetPath(DocmentOnwerTypeEnum onwerType, UploadClientTypeEnum clientType,string fileMd5)
        {
            var onwerStr = EnumHelper.GetEnumDescription(onwerType);
            var clientStr= EnumHelper.GetEnumDescription(clientType);
            var todayStr = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            this.RelativePath= Path.Combine(ConfigConstant.RelativePathRoot, onwerStr, clientStr, todayStr, fileMd5); 
            var path = Path.Combine(UploadFile._uploadFolder, onwerStr, clientStr, todayStr, fileMd5);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}