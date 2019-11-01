//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Events;

namespace Kooboo.Web.Backend.Events.Hanlders
{
    public class HandleBindingChange : Data.Events.IHandler<BindingChange>
    {
        public void Handle(BindingChange theEvent, RenderContext context)
        {
            switch (theEvent.ChangeType)
            {
                case ChangeType.Add:
                {
                    if (theEvent.Binding.Port > 0)
                    {
                        SystemStart.StartNewWebServer(theEvent.Binding.Port);
                    }

                    break;
                }
                case ChangeType.Update:
                {
                    if (theEvent.OldBinding.Port != theEvent.Binding.Port)
                    {
                        if (theEvent.OldBinding.Port > 0)
                        {
                            //SystemStart.Stop(theEvent.OldBinding.Port);
                        }

                        if (theEvent.Binding.Port > 0)
                        {
                            SystemStart.StartNewWebServer(theEvent.Binding.Port);
                        }
                    }

                    break;
                }
                default:
                {
                    // it is a deletion.
                    if (theEvent.Binding.Port > 0)
                    {
                        // SystemStart.Stop(theEvent.binding.Port);
                    }

                    break;
                }
            }
        }
    }
}