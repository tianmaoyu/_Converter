using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace AClassroom.Core
{
    /// <summary>
    /// 配置常量
    /// </summary>
    public class ConfigConstant
    {
#if DEBUG

        /// <summary>
        /// sql server数据库连接
        /// </summary>
        public static readonly String ConnectionString = ConfigurationManager.ConnectionStrings["sqlServer_Debug"].ConnectionString;
#else
 
        /// <summary>
        /// sql server数据库连接
        /// </summary>
        public static readonly String ConnectionString = ConfigurationManager.ConnectionStrings["sqlServer"].ConnectionString;
       
#endif


        /// <summary>
        /// 文件上传
        /// </summary>
        public static readonly string UploadFolder = HostingEnvironment.MapPath("~/Upload/" + UploadFolder);

        /// <summary>
        /// 临时保存目录
        /// </summary>
        public static readonly string UploadFolderTemp = HostingEnvironment.MapPath("~/UploadTemp/" + UploadFolder);

        /// <summary>
        /// 文件所属项目
        /// </summary>
        public static readonly String DocmentOnwerType = ConfigurationManager.AppSettings["DocumentOnwerType"];
    }
}
