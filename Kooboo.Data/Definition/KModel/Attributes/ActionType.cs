using System;

namespace Kooboo.Data.Definition.KModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ActionType : Attribute, IMetaAttribute
    {
        public EnumActionType actionType { get; set; }

        public ActionType(EnumActionType action)
        {
            actionType = action;
        }

        public bool IsHeader => false;

        public string PropertyName => "Action";

        public string Value()
        {
            return actionType.ToString();
        }
    }
}