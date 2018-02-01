using AClassroom.Core;
using AClassroom.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AClassroom.DocConverter.Models
{
    public class CommonResult<T>
    {
        public bool Success
        {
            get
            {
                if (this.ResultEnum == CommonResultEnum.Success)
                {
                    return true;
                }
                return false;
            }
        }
        public CommonResultEnum ResultEnum { set; get; }

        public T Data { set; get; }

        public string Message
        {
            get
            {
                return EnumHelper.GetEnumDescription(this.ResultEnum);
            }
        }
    }
}