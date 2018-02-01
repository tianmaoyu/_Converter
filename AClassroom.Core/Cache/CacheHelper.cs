using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;

namespace AClassroom.Core
{
    public class CacheHelper
    {
        private static readonly System.Web.Caching.Cache _cache;
        /**/
        /// <summary>
        /// 单件模式
        /// </summary>
        static CacheHelper()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context != null)
            {
                _cache = context.Cache;
            }
            else
            {
                _cache = System.Web.HttpRuntime.Cache;
            }

        }
        /// <summary>
        /// 创建缓存项的文件依赖
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="obj">object对象</param>
        /// <param name="fileName">文件绝对路径</param>
        public static void Insert(string key, object obj, string fileName)
        {
            //创建缓存依赖项
            CacheDependency dep = new CacheDependency(fileName);
            //创建缓存
            _cache.Insert(key, obj, dep);
        }

        /// <summary>
        /// 创建缓存项过期
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="obj">object对象</param>
        /// <param name="expires">过期时间(分钟)</param>
        public static void Insert(string key, object obj, int expires)
        {
            _cache.Insert(key, obj, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, expires, 0));
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>object对象</returns>
        public static object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            return _cache.Get(key);
        }
        /// <summary>
        /// 一次性清除所有缓存
        /// </summary>
        public static void Clear()
        {
            System.Collections.IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            while (CacheEnum.MoveNext()) //逐个清除
            {
                al.Add(CacheEnum.Key);
            }

            foreach (string key in al)
            {
                _cache.Remove(key);
            }
        }
        /// <summary>
        /// 清除特定的缓存
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="T">T对象</typeparam>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            object obj = Get(key);
            return obj == null ? default(T) : (T)obj;
        }

        /// <summary>
        /// 是否存在缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ExistsCache(string key)
        {
            if (_cache.Get(key) == null)
                return false;
            else
                return true;
        }

    }
}

