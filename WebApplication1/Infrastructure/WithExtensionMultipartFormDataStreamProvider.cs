using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace AClassroom.DocConverter.Infrastructure
{
    public class WithExtensionMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public WithExtensionMultipartFormDataStreamProvider(string rootPath)
            : base(rootPath)
        {
        }

        public WithExtensionMultipartFormDataStreamProvider(string rootPath, int bufferSize)
            : base(rootPath, bufferSize)
        {
        }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            Random random = new Random();
            int rn = random.Next(100, 1000);
            string ext = string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? "" : Path.GetExtension(GetValidFileName(headers.ContentDisposition.FileName));
            string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff", System.Globalization.DateTimeFormatInfo.InvariantInfo) + rn.ToString() + ext;
            return filename;
        }

        private string GetValidFileName(string filePath)
        {
            char[] invalids = System.IO.Path.GetInvalidFileNameChars();
            return String.Join("_", filePath.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }
    }
}