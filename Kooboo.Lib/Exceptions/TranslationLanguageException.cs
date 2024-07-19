using System;

namespace Kooboo.Lib.Exceptions;

public class TranslationLanguageException : Exception
{
    public TranslationLanguageException(string message) : base(message)
    {
    }
}