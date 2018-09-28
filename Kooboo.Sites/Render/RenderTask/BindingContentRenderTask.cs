using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.ViewModel;

namespace Kooboo.Sites.Render
{
    public class BindingContentRenderTask : BindingObjectRenderTask
    { 
        public override string Render(RenderContext context)
        {
            string objecttype = this.ObjectType;
            string nameorid = this.NameOrId;

            SetBindingInfo(context, ref objecttype, ref nameorid); 
             
            string result = "\r\n<!--#kooboo";
            if (this.IsEndBinding)
            {
                result += "--end='true'";

                if (!string.IsNullOrEmpty(objecttype))
                {
                    result += "--objecttype='" + this.ObjectType + "'";
                }

                if (!string.IsNullOrEmpty(this.Boundary))
                {
                    result += "--boundary='" + this.Boundary + "'";
                }
                result += "-->\r\n";
                return result;
            }
            else
            {

                if (!string.IsNullOrEmpty(objecttype))
                {
                    result += "--objecttype='" + this.ObjectType + "'";
                }

                if (!string.IsNullOrEmpty(nameorid))
                {
                    result += "--nameorid='" + nameorid + "'";
                }

                if (!string.IsNullOrEmpty(this.BindingValue))
                {
                    result += "--bindingvalue='" + this.BindingValue + "'";

                    int lastDotIndex = this.BindingValue.LastIndexOf(".");
                    if (lastDotIndex > -1)
                    {
                        string FieldName = this.BindingValue.Substring(lastDotIndex + 1);
                        result += "--fieldname='" + FieldName + "'";
                    }
                }

                if (!string.IsNullOrEmpty(this.KoobooId))
                {
                    result += "--koobooid='" + this.KoobooId + "'";
                }

                if (!string.IsNullOrEmpty(this.Boundary))
                {
                    result += "--boundary='" + this.Boundary + "'";
                }
                result += "-->\r\n";
                return result;
            }
        }
          
        private void SetBindingInfo(RenderContext context, ref string objecttype, ref string nameorid)
        {  
            if (string.IsNullOrEmpty(this.BindingValue))
            {
                return; 
            }

            string objectkey = this.BindingValue; 
             int lastDotIndex = this.BindingValue.LastIndexOf(".");
            if (lastDotIndex > -1)
            {
                objectkey = this.BindingValue.Substring(0, lastDotIndex); 
            }

            object value = context.DataContext.GetValue(objectkey);
            if (value == null)
            {
                return; 
            } 
            if (value is DataMethodResult)
            {
                value = ((DataMethodResult)value).Value;
            } 
            if (value is TextContentViewModel)
            {
                TextContentViewModel textcontent = value as TextContentViewModel;
                objecttype = "content";
                nameorid = textcontent.Id.ToString();
            } 
        } 
    }
}
