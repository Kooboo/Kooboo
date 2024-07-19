namespace Kooboo.Mail.Extension
{


    public class MailResourceType
    {
        public MailResourceType(EnumMailResourceType type)
        {
            this.Type = type;
        }
        public MailResourceType(string objectType)
        {
            var type = Kooboo.Lib.Helper.EnumHelper.GetEnum<EnumMailResourceType>(objectType);
            this.Type = type;
        }

        public string Name
        {
            get
            {
                return this.Type.ToString().ToLower();
            }
        }
        public string DisplayName
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        public EnumMailResourceType Type { get; set; }

        public bool IsText
        {
            get
            {
                if (this.Type == EnumMailResourceType.read || this.Type == EnumMailResourceType.compose || this.Type == EnumMailResourceType.css || this.Type == EnumMailResourceType.js || this.Type == EnumMailResourceType.api || this.Type == EnumMailResourceType.backend || this.Type == EnumMailResourceType.root)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsBinary
        {
            get
            {
                return !this.IsText;
            }
        }

        private string GetDisplayName()
        {
            switch (this.Type)
            {
                case EnumMailResourceType.read:
                    {
                        return "Read";
                    }
                case EnumMailResourceType.compose:
                    {
                        return "Compose";
                    }
                case EnumMailResourceType.css:
                    {
                        return "StyleSheet";
                    }
                case EnumMailResourceType.img:
                    {
                        return "Image";
                    }
                case EnumMailResourceType.api:
                    {
                        return "Api";
                    }
                case EnumMailResourceType.file:
                    {
                        return "File";
                    }
                case EnumMailResourceType.js:
                    {
                        return "JavaScript";
                    }

                case EnumMailResourceType.root:
                    {
                        return "Root";
                    }

                case EnumMailResourceType.undefined:
                    {
                        return "undefined";
                    }
                case EnumMailResourceType.backend:
                    {
                        return "Backend";
                    }

                default:
                    {
                        return "View";
                    }
            }


        }

        public string defaultExtension
        {
            get
            {
                if (this.IsText)
                {
                    if (this.Type == EnumMailResourceType.read || this.Type == EnumMailResourceType.compose || this.Type == EnumMailResourceType.backend)
                    {
                        return null;
                    }
                    else if (this.Type == EnumMailResourceType.css)
                    {
                        return ".css";
                    }
                    else if (this.Type == EnumMailResourceType.api)
                    {
                        return null;
                    }
                    else if (this.Type == EnumMailResourceType.js)
                    {
                        return ".js";
                    }
                }
                return null;
            }
        }
        public string DefaultContentType
        {

            get
            {
                switch (this.Type)
                {
                    case EnumMailResourceType.undefined:
                        break;
                    case EnumMailResourceType.read:
                        return "text/html;charset=utf-8";
                    case EnumMailResourceType.compose:
                        return "text/html;charset=utf-8";
                    case EnumMailResourceType.backend:
                        return "text/html;charset=utf-8";

                    case EnumMailResourceType.root:
                        return "text/html;charset=utf-8";

                    case EnumMailResourceType.css:
                        return "text/css;charset=utf-8";

                    case EnumMailResourceType.js:
                        return "text/javascript;charset=utf-8";

                    case EnumMailResourceType.api:
                        return "text/javascript;charset=utf-8";

                    case EnumMailResourceType.img:
                        return "application/image;";

                    case EnumMailResourceType.file:
                        return "application/octet-stream";

                    default:
                        return "application/octet-stream";
                }

                return "application/octet-stream";

            }


        }

        //public Extension[] Extensions
        //{
        //    get
        //    {
        //        if (this.Type == EnumResourceType.backend)
        //        {
        //            return new[]{
        //                new  Extension(FileType.javascript,".js","Dashboard"),
        //                new  Extension(FileType.html,".html","BackendView"),
        //                new  Extension(FileType.javascript,".event","Events"),
        //            };
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }

        //}


    }

    //public class Extension
    //{
    //    public Extension(FileType type, string name, string display)
    //    {
    //        Type = type;
    //        Name = name;
    //        Display = display;
    //    }

    //    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    //    public FileType Type { get; }
    //    public string Name { get; }
    //    public string Display { get; }
    //}






}
