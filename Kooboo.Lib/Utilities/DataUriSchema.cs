//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Utilities
{
    // If <MIME-type> is omitted, it defaults to text/plain;charset=US-ASCII.
    //data:[<MIME-type>][;charset=<encoding>][;base64],<data>
    //<data> is a sequence of octets. If ;base64 is present, the data is encoded as base64. Otherwise, the data is represented using percent-encoding, using ASCII for octets inside the range of safe URL characters and %xx hex encoding for octets outside that range.

    public class DataUriSchema
    {
        private string _minetype;

        public string MineType
        {
            get
            {
                if (string.IsNullOrEmpty(_minetype))
                {
                    return "text/plain";
                }
                else
                {
                    return _minetype;
                }
            }
            set
            {
                _minetype = value;
            }
        }

        private string _charset;
        public string CharSet {
            get
            {
                if (string.IsNullOrEmpty(_charset)  && string.IsNullOrEmpty(_minetype))
                {
                    return "US-ASCII";
                }
                else
                {
                    return _charset; 
                }
            }
            set
            {
                _charset = value; 
            }
        }

        public string DataString { get; set; }

        public bool isBase64 { get; set; }
        
    }
}
