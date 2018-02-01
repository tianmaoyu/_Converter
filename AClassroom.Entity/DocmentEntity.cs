using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AClassroom.Entity
{
    /// <summary>
    /// 上传的文件，文件
    /// </summary>
    public  class Document: BaseEntity
    {

        /// <summary>
        /// 文件件名称 
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// 上传的文件的md5
        /// </summary>
        public string FileMD5 { get; set; }

        /// <summary>
        /// 上传的文件的原名称
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// 上传文件的物理路径
        /// </summary>
        public string PhysicalPath { get; set; }

        /// <summary>
        /// 文件转换为 png 后的 页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 转换为 Image 的相关 的长宽等信息
        /// </summary>
        public string PageInfos { get; set; }

        /// <summary>
        /// 文件下载地址
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public int DocmentType { get; set; }

        /// <summary>
        /// 文件类型名称
        /// </summary>
        public string DocmentTypeName { get; set; }

        /// <summary>
        /// 文档上传来源（上传客户端类型） 
        /// </summary>
        public int UploadClientType { get; set; }

        /// <summary>
        /// 文档上传来源名称（上传客户端类型名称） 
        /// </summary>
        public string UploadClientTypeName { get; set; }
        
        /// <summary>
        /// 文档的所属项目
        /// </summary>
        public int DocmentOnwerType { get; set; }

        /// <summary>
        /// 文档的所属项目
        /// </summary>
        public string DocmentOnwerTypeName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 是否失效
        /// </summary>
        public bool Enabled { get; set; }
    }
}
