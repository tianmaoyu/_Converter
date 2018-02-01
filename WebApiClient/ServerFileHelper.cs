using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    public class ServerFileHelper
    {
        private readonly string api = "http://localhost:5247/api/docsvcs/upload";

        public ServerFileHelper(string apiurl)
        {
            api = apiurl;
        }
        public string UploadFile(string FullFileName, string folderid, string token, string filehash)
        {
            string sApiUri = api + string.Format("?folderid={0}&token={1}&filehash={2}", folderid, token, filehash);
            Uri server = new Uri(/*api*/sApiUri);
            HttpClient httpClient = new HttpClient();

            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();

            string filename = Path.GetFileName(FullFileName);
            if (!string.IsNullOrEmpty(filename))
            {
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(FullFileName);
                //这里会向服务器上传一个png图片和一个txt文件
                StreamContent streamConent = new StreamContent(new FileStream(FullFileName, FileMode.Open, FileAccess.Read, FileShare.Read));

                multipartFormDataContent.Add(streamConent, filenameWithoutExtension, filename);
            }

            HttpResponseMessage responseMessage = httpClient.PostAsync(server, multipartFormDataContent).Result;

           return responseMessage.Content.ReadAsStringAsync().Result;
        }

        public bool DownLoad(string ServerFileName, string SaveFileName)
        {
            Uri server = new Uri(String.Format("{0}?filename={1}", api, ServerFileName));
            HttpClient httpClient = new HttpClient();

            string p = Path.GetDirectoryName(SaveFileName);

            if (!Directory.Exists(p))
                Directory.CreateDirectory(p);

            HttpResponseMessage responseMessage = httpClient.GetAsync(server).Result;

            if (responseMessage.IsSuccessStatusCode)
            {
                using (FileStream fs = File.Create(SaveFileName))
                {
                    Stream streamFromService = responseMessage.Content.ReadAsStreamAsync().Result;
                    streamFromService.CopyTo(fs);
                    return true;
                }
            }
            else
                return false;
        }
    }
}
