using System;

namespace Kooboo.IndexedDB.Condition
{
    public class ConditionParseException : Exception
    {
        public ConditionParseException(int positon) : base($"Parse condition error at positon {positon}")
        {

        }
    }
}
