//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Events; 
using Kooboo.Data.Context;

namespace Kooboo.Web.Backend.Events.Hanlders
{
    public class HandleBindingChange : Data.Events.IHandler<BindingChange>
    {
        public void Handle(BindingChange theEvent, RenderContext context)
        {
            if (theEvent.ChangeType == ChangeType.Add)
            {
                if (theEvent.binding.Port >0)
                {  
                    SystemStart.StartNewWebServer(theEvent.binding.Port); 
                }
            }
            else if (theEvent.ChangeType == ChangeType.Update)
            {
                if (theEvent.OldBinding.Port != theEvent.binding.Port)
                {
                    if (theEvent.OldBinding.Port >0)
                    {
                       //SystemStart.Stop(theEvent.OldBinding.Port); 
                    }

                    if (theEvent.binding.Port >0)
                    {    
                        SystemStart.StartNewWebServer(theEvent.binding.Port); 
                    }
                }
            }
            else
            {
                // it is a deletion. 
                if (theEvent.binding.Port >0)
                {
                 // SystemStart.Stop(theEvent.binding.Port); 
                }
            }
        }
    }
}
