using Kooboo.Data.Context;
using System.ComponentModel;


namespace Kooboo.Sites.Scripting.Global
{  
    public class ClientJs
    {
        private RenderContext context { get; set; }
        public ClientJs(RenderContext context)
        {
            this.context = context;
        }

        [Description(@"Send an Object as a client JS object to Browser;
var obj = {};
obj.firstname = ""name""; 
obj.lastname = ""lastname""; 
k.clientJS.setVariable(""myname"", obj); ")]
        public void SetVariable(string key, object obj)
        {
            string output = "";
            output = "<script>\r\n";

            var valuestring = ToJson(obj);

            valuestring = System.Web.HttpUtility.JavaScriptStringEncode(valuestring, true);

            output += "var " + key + " =JSON.parse(" + valuestring + "); \r\n"; 
            output += "</script>"; 
            write(output);  
        }


        [Description(@"Send object properties as client JS variables to browser;
 var obj = {};
obj.varone = {};
obj.varone.name =""nameone"";  
obj.vartwo = {};
obj.vartwo.headline = ""headline"";  
k.clientJS.setVariable(obj);")]
        public void SetVariable(object obj)
        {
            string output = "";
            output = "<script>\r\n";

            var data = kHelper.CleanDynamicObject(obj);

            foreach (var item in data)
            {
                var valuestring = ToJson(item.Value);

                valuestring = System.Web.HttpUtility.JavaScriptStringEncode(valuestring, true);

                output += "var " + item.Key + " =JSON.parse(" + valuestring + "); \r\n";
            }

           
            output += "</script>";
            write(output);
        }

        private string ToJson(object value)
        {
            string output;
            if (!(value is string) && value.GetType().IsClass)
            {
                output = Kooboo.Lib.Helper.JsonHelper.SerializeCaseSensitive(value, new Kooboo.Lib.Helper.IntJsonConvert());
            }
            else
            {
                output = value.ToString();
            }
            return output;
        }
        
        //copy from response.write
        private void write(string output)
        {
            if (output == null)
            {
                return;
            } 

            var item = this.context.GetItem<string>(Kooboo.Sites.Scripting.Constants.OutputName);
            if (item == null)
            {
                this.context.SetItem<string>(output, Kooboo.Sites.Scripting.Constants.OutputName);
            }
            else
            {
                item += output;
                this.context.SetItem<string>(item, Kooboo.Sites.Scripting.Constants.OutputName);
            }
        }
 
    }
}
