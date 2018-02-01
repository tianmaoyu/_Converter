using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AClassroom.Core
{
    public class FileHelper
    {
        /// <summary>
        /// 计算一个文件的 MD5 哈希值
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMd5Hash(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = MD5.Create().ComputeHash(fs);
                    var str = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                    return str;
                }
            }
            return string.Empty;
        }



    }
}
