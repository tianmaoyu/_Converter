using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AClassroom.Entity.Enum
{

    /// <summary>
    /// 客户端上传的课件类型 
    /// </summary>
    public enum UploadClientTypeEnum
    {
        [Description("管理员管理台")]
        AdminWeb = 1,
        [Description("老师客户端")]
        ClientTeacher = 2,
        [Description("学生客户端")]
        ClientStudent = 3,
        [Description("开发测试")]
        ClientTest = 4
    }
}
