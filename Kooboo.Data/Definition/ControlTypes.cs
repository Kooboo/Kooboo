using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Kooboo.Data.Definition
{
    public class ControlTypes
    {
        public const string TextBox = "TextBox";

        public const string Hidden = "Hidden";

        public const string Int32 = "Int32";

        public const string Checkbox = "CheckBox";

        public const string DateTime = "DateTime";

        public const string TextArea = "TextArea";

        public const string TinyMCE = "TinyMCE";

        public const string MediaImage = "MediaImage";

        public const string MediaImages = "MediaImages";

        public const string MediaFile = "MediaFile";

        public const string MediaFiles = "MediaFiles";

        public const string ImageUploader = "ImageUploader";

        public const string Number = "Number";

        public const string Boolean = "Boolean";

        public const string Selection = "Selection";

        public const string RadioBox = "RadioBox";


        private static List<string> _list;

        public static List<string> List
        {
            get
            {
                if (_list == null)
                {
                    _list = new List<string>();

                    var allfields = typeof(ControlTypes).GetFields(BindingFlags.Static | BindingFlags.Public);

                    var instance = Activator.CreateInstance(typeof(ControlTypes));

                    foreach (var item in allfields)
                    {
                        var value = item.GetValue(instance);
                        _list.Add(value.ToString());
                    }
                }
                return _list;
            }
        }


    }
}
