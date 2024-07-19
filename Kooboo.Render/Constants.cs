//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Render
{
    public class Constants
    {
        /// <summary>
        /// the Kooboo  placeholder tag, can be:
        /// <div tal-placeholder=myposition></div> or <!--position:myid-->    <!--end-->
        /// </summary>
        public static readonly string[] PlaceHolderAttributes = new[] { "tal-placeholder", "position", "placeholder" };

        public const string CommentPlaceHolderEndTag = "end";

        public const string KoobooIdAttributeName = "kooboo-id";

        public const string BindingKoobooContent = "kooboo_content";

        public const string BindingKoobooLabel = "kooboo_label";

        public const string BindingDefaultAttributeContent = "kooboo_content";

        /// <summary>
        /// used when there is no attribute place to put the binding info. 
        /// this is in the case like tal-replace. 
        /// </summary>
        public const string BindingAdditionTag = "<kooboo-begin binding='{0}'></kooboo-begin>";

        public const string BindingAttributeName = " kooboo-binding='{0}'";

        public const string BindingEndTag = "<kooboo-end></kooboo-end>";

        public const string BindingRepeaterStart = "\r\n<!--kooboo repeater: {{ alias: \"{0}\", datakey: \"{1}\", objectid: \"{2}\", folder: \"{3}\" }}-->\r\n";

        public const string BindingRepeaterEnd = "\r\n<!--/kooboo repeater-->\r\n";

    }
}
