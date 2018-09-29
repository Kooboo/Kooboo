//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.App.Validators
{
    public class FolderExistsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            return System.IO.Directory.Exists(value.ToString());
        }

        public override string FormatErrorMessage(string name)
        {
            return Data.Language.Hardcoded.GetValue("Folder") + " " + name + " " + Data.Language.Hardcoded.GetValue("is not exists"); 
        }
    }
}
