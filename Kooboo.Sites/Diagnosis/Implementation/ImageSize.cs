//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context; 
using Kooboo.Data.Language;
using Kooboo.Sites.Extensions;

namespace Kooboo.Sites.Diagnosis.Implementation
{
    public class ImageSize : IDiagnosis
    {
        public DiagnosisSession session { get; set; }


        public string Group(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Normal", context);
        }

        public string Name(RenderContext context)
        {
            return Hardcoded.GetValue("Check the size of image, should below 1MB", context); 
        }

        public void Check()
        {
            string name = Hardcoded.GetValue("image size", session.context);

            session.Headline = Hardcoded.GetValue("Checking", session.context) + " " + name; 

            var sitedb = session.context.WebSite.SiteDb();

            var allimages = sitedb.Images.All(true);

            int maxsize = 1024 * 1024;
            int terriblesize = 5 * maxsize;

            foreach (var item in allimages)
            {
                var imageNameText = Hardcoded.GetValue("Name", session.context);
                var imageSizeText = Hardcoded.GetValue("Size", session.context);

                int size = item.Size;

                string message = string.Format("<b>{0}: {1}, {2}: {3}<b><br/>", imageNameText, item.Name, imageSizeText, Kooboo.Lib.Utilities.CalculateUtility.GetSizeString(size));

                if (size > terriblesize)
                {
                    var usedby = sitedb.Images.GetUsedBy(item.Id);   
                    message += DiagnosisHelper.DisplayUsedBy(session.context, usedby); 

                    session.AddMessage(name, message, MessageType.Critical);  

                }
                else if (size > maxsize)
                {
                    var usedby = sitedb.Images.GetUsedBy(item.Id);
                    message += DiagnosisHelper.DisplayUsedBy(session.context, usedby);
                     
                    session.AddMessage(name, message, MessageType.Warning);  
                }
            } 
        }

       
    }
}