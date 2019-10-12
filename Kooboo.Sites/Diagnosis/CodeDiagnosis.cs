//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System;

namespace Kooboo.Sites.Diagnosis
{
    // this is only used to generate the list..
    public class CodeDiagnosis : IDiagnosis
    {
        public DiagnosisSession session { get; set; }

        public void Check()
        {
            throw new NotImplementedException();
        }

        public string Group(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Code", context);
        }

        public string Name(RenderContext context)
        {
            return null;
        }
    }
}