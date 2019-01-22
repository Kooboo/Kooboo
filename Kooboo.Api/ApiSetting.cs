using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Api
{
  public  class ApiSetting
    {
        private static string _samplefolder; 
        public static string SampleFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_samplefolder))
                {
                    string root = AppDomain.CurrentDomain.BaseDirectory;
                     _samplefolder = System.IO.Path.Combine(root, "../../Sample");
                    if (System.IO.Directory.Exists(_samplefolder))
                    {
                        System.IO.Directory.CreateDirectory(_samplefolder); 
                    } 
                }
                return _samplefolder; 
            }

        }
    }
}
