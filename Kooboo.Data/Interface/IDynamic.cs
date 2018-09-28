using Kooboo.Data.Context;
using System;

namespace Kooboo.Data.Interface
{
   public interface IDynamic
    {
        Object GetValue(string FieldName);

        Object GetValue(string FieldName, RenderContext Context); 
         
        void SetValue(string FieldName, Object Value); 
    }
}
