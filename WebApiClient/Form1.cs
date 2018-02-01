using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebApiClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public ServerFileHelper getServer()
        {
            ServerFileHelper sf = new ServerFileHelper(txt_API.Text);
            return sf;
        }

        //添加文件
        private void btn_AddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Multiselect = true;
            if (o.ShowDialog() == DialogResult.OK)
            {
                foreach (string str in o.FileNames)
                {
                    txt_FileNamesUpload.AppendText(str + System.Environment.NewLine);
                }
            }
        }

        public static String ComputeMD5(String fileName)
        {
            var hashMD5 = String.Empty;
            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    System.Security.Cryptography.MD5 calculator = System.Security.Cryptography.MD5.Create();
                    Byte[] buffer = calculator.ComputeHash(fs);
                    calculator.Clear();
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        stringBuilder.Append(buffer[i].ToString("x2"));
                    }
                    hashMD5 = stringBuilder.ToString();
                }
            }
            return hashMD5.ToLower();
        }

        //上传文件
        private void btn_UpLoad_Click(object sender, EventArgs e)
        {
            var sf = getServer();
            foreach (string fullfilename in txt_FileNamesUpload.Lines)
            {
                string filehash = ComputeMD5(fullfilename);
                var rs = sf.UploadFile(fullfilename, "100", "01234567", filehash);
                txt_Result.Text += rs;
            }
        }
                
        //下载文件
        private void btn_Down_Click(object sender, EventArgs e)
        {
            var sf = getServer();
            String FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\\Downs\\" + txt_FileNameDown.Text);
            bool success = sf.DownLoad(txt_FileNameDown.Text, FileName);
            if (success == true)
            {
                if (MessageBox.Show("文件下载成功，要打开文件所在目录吗？", "提醒", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string path = Path.GetDirectoryName(FileName);
                    System.Diagnostics.Process.Start("explorer.exe", path);
                }
            }


        }


        private string ConvertJsonString(string str)
        {
            //格式化json字符串
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            TextReader tr = new StringReader(str);
            Newtonsoft.Json.JsonTextReader jtr = new Newtonsoft.Json.JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(textWriter)
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }


    }
}
