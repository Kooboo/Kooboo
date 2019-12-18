using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface UploadFile
    {
        string ContentType { get; set; }
        string FileName { get; set; }
        int[] Bytes { get; set; }

        [Description("save the file into disk")]
        string save(string filename);
    }
}
