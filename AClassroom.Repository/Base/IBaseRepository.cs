using AClassroom.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AClassroom.Repository
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        T GetById(int Id);

        int Insert(T t);

        int Update(T t);

        int Delete(T t);
    }
}
