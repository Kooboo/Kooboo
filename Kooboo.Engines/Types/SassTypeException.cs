using System;

namespace LibSass.Types
{
    public class SassTypeException : Exception
    {
        internal static readonly string
            ArbitraryInterfaceImplmentationMessage = string.Join(" ",
                     "The value must not contain an object of type that is",
                     "an arbitrary implementation of ISassType. Please use",
                     "the predefined Sass types or extend the predefined type's",
                     "functionality using inheritance or extension methods.");

        internal static readonly string
            CircularReferenceMessage = string.Join("",
                     "Circular reference detected in a SassMap.", Environment.NewLine,
                     "Values cannot contain self-referencing instance.");

        public SassTypeException(string message) : base(message) { }
    }
}
