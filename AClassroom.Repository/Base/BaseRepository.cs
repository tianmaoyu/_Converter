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

    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        public int Delete(T t)
        {
            var type = typeof(T);
            var info = type.GetProperty("Id");
            var sql = string.Format("where Id='{1}'", info.GetValue(t, null));
            sql = string.Format("Delete [{0}] ", type.Name) + sql;
            using (SqlConnection conn = new SqlConnection(ConfigConstant.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                conn.Open();
                var r = command.ExecuteNonQuery();
                return r;
            }
        }

        public T GetById(int Id)
        {
            var type = typeof(T);
            var t = Activator.CreateInstance(type);
            var fields = string.Join(",", type.GetProperties().Select(i => string.Format("{0}", i.Name)));
            var sql = string.Format("select {0} from [{1}] where Id={2}", fields, type.Name, Id);
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
                        property.SetValue(t, reader[proertyName] == DBNull.Value ? null : reader[proertyName]);
                    }
                }
            }
            return (T)t;
        }
        public int Insert(T t)
        {
            var type = typeof(T);
            string fields = string.Empty;
            string values = string.Empty;
            foreach (var info in type.GetProperties())
            {
                fields += info.Name + ',';
                values += "'" + info.GetValue(t, null) + "',";
            }
            fields = fields.Substring(0, fields.Length - 1);
            values = values.Substring(0, values.Length - 1);
            var sql = string.Format("insert into [{2}]  ({0}) values({1}) select cast(scope_identity() as bigint)", fields, values, type.Name);
            using (SqlConnection conn = new SqlConnection(ConfigConstant.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var r = reader[0] != DBNull.Value ? "-1" : reader[0];
                    return Int32.Parse(r.ToString());
                }
            }
            return -1;
        }

        public int Update(T t)
        {
            var type = typeof(T);
            string sql = string.Empty;
            string where = string.Empty;
            foreach (var info in type.GetProperties())
            {
                sql += string.Format("{0}='{1}',", info.Name, info.GetValue(t, null));
            }
            sql = string.Format("update [{0}] set {1} ", type.Name, sql.Substring(0, sql.Length - 1) + " " + where);
            using (SqlConnection conn = new SqlConnection(ConfigConstant.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                conn.Open();
                var r = command.ExecuteNonQuery();
                return r;
            }
        }
    }
}
