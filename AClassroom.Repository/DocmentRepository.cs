using AClassroom.Core;
using AClassroom.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AClassroom.Repository
{
    public class DocumentRepository : BaseRepository<Document>
    {
        /// <summary>
        /// 通过Filemd5 查询 文件
        /// </summary>
        /// <param name="fileMd5"></param>
        /// <returns></returns>
        public Document GetByMd5(string fileMd5)
        {
            var document = new Document();
            var type = typeof(Document);
            var fields = string.Join(",", type.GetProperties().Select(i => string.Format("{0}", i.Name)));
            var sql = string.Format("select {0} from [{1}] where FileMD5='{2}'", fields, type.Name, fileMd5);
            using (SqlConnection conn = new SqlConnection(ConfigConstant.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    foreach (var property in type.GetProperties())
                    {
                        var proertyName = property.Name;
                        property.SetValue(document, reader[proertyName] == DBNull.Value ? null : reader[proertyName]);
                    }
                }
                else
                {
                    return null;
                }
            }
            return document;
        }
    }
}
