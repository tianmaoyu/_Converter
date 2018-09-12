using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AClassroom.Entity.Enum
{
    /// <summary>
    /// 文档所属的项目
    /// </summary>
    public enum DocmentOnwerTypeEnum
    {
        [Description("test生产环境")]
        test = 1,
        [Description("test测试环境")]
        test_Debugger = 2,


        [Description("testB2B生产环境")]
        testB2B = 5,
        [Description("testB2B测试环境")]
        testB2B_Debugger = 6,


        [Description("AliceABC生产环境")]
        AliceABC = 10,
        [Description("AliceABC试环境")]
        AliceABC_Debugger = 11,

        [Description("AliceABCB2B生产环境")]
        AliceABCB2B = 15,
        [Description("AliceABCB2B试环境")]
        AliceABCB2B_Debugger = 16,


        [Description("Box生产环境")]
        Box = 30,
        [Description("Box试环境")]
        Box_Debugger = 31,



    }
}
