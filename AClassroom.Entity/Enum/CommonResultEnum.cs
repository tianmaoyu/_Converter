using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AClassroom.Entity.Enum
{
    public enum CommonResultEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success = 1,

        /// <summary>
        /// 未登录
        /// </summary>
        [Description("未登录")]
        NotLoggedIn = 2,

        /// <summary>
        ///  没有数据
        /// </summary>
        [Description("没有数据")]
        NoData = 9,

        /// <summary>
        ///  参数有误
        /// </summary>
        [Description("参数有误")]
         ParameterErro = 10,

        /// <summary>
        /// 未知错误
        /// </summary>
        [Description("未知错误")]
        SystemError = 12,
    }
}
