//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Diagnosis
{  
    public class DiagnosisChecker
    {
        public string Group { get; set; }

        public string Name { get; set; }

        public bool IsCode { get; set; }

        public Type Type { get; set; }

        public Guid Id
        {
            get {
                string unique = this.Group + this.Name;
                return Lib.Security.Hash.ComputeGuidIgnoreCase(unique); 
            }
        }

    } 
}
