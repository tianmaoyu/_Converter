using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AClassroom.Entity.Enum
{
    /// <summary>
    /// 上传的文件类型
    /// </summary>
    public enum DocmentTypeEnum
    {
        [Description(".ppt")]
        PPT = 1,
        [Description(".pptx")]
        PPTX = 2,

        [Description(".doc")]
        DOC = 10,
        [Description(".docx")]
        DOCX = 11,

        [Description(".pdf")]
        PDF = 20,


        [Description(".txt")]
        TXT = 30,


        [Description(".mp3")]
        MP3 = 40,
        [Description(".mp4")]
        MP4 = 41,


    }
}
