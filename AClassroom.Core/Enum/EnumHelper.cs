using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AClassroom.Core
{
   public class EnumHelper
    {
        /// <summary>
        /// 获取枚举类子项描述信息
        /// </summary>
        /// <param name="enumSubitem">枚举类子项</param>        
        public static string GetEnumDescription(Enum enumSubitem)
        {
            Object obj = GetAttributeClass(enumSubitem, typeof(DescriptionAttribute));
            if (obj == null)
            {
                return enumSubitem.ToString();
            }
            else
            {
                DescriptionAttribute da = (DescriptionAttribute)obj;
                return da.Description;
            }
        }

        /// <summary>
        /// 根据枚举获取 值，描述，可以用于生产下拉列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> GetDescriptionValues<T>()
        {
            var dic = CacheHelper.Get(typeof(T).FullName);
            if (dic != null) return (Dictionary<int, string>)dic;

            var result = new Dictionary<int, string>();
            var values = Enum.GetValues(typeof(T));
            foreach (var value in values)
            {
                //取得枚举的描述：做下拉的名称
                var field = value.GetType().GetField(value.ToString());
                var objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                var text = objs.Length == 0 ? value.ToString() : ((DescriptionAttribute)objs[0]).Description;
                result.Add((int)value, text);
            }
            result.OrderBy(item => item.Key);
            CacheHelper.Insert(typeof(T).FullName, result, 15);
            return result;
        }

        /// <summary>
        /// 获取指定属性类的实例
        /// </summary>
        /// <param name="enumSubitem">枚举类子项</param>
        /// <param name="attributeType">DescriptionAttribute属性类或其自定义属性类 类型，例如：typeof(DescriptionAttribute)</param>
        private static Object GetAttributeClass(Enum enumSubitem, Type attributeType)
        {
            FieldInfo fieldinfo = enumSubitem.GetType().GetField(enumSubitem.ToString());
            Object[] objs = fieldinfo.GetCustomAttributes(attributeType, false);
            if (objs == null || objs.Length == 0)
            {
                return null;
            }
            return objs[0];
        }
    }
}
