using AClassroom.Entity;
using AClassroom.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AClassroom.Server
{
    public class DocumentServer: IServer<Document>
    {
        private static DocumentRepository _documentRepository = new DocumentRepository();

        public int Delete(Document  docment)
        {
            return _documentRepository.Delete(docment);
        }

        public Document GetById(int Id)
        {
            return _documentRepository.GetById(Id);
        }

        public int Insert(Document docment)
        {
            return _documentRepository.Insert(docment);
        }

        public int Update(Document docment)
        {
            return _documentRepository.Update(docment);
        }

        public Document GetByMd5(string fileMd5)
        {
            return _documentRepository.GetByMd5(fileMd5);
        }
    }
}
