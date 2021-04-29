using Kooboo.Sites.Scripting.Global.Api.Meta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Api
{
    public class KApiException : Exception
    {
        public KApiException(string message) : base(message)
        {
        }
    }

    public class RequiredException : KApiException
    {
        public RequiredException(string[] path) : base($"{string.Join(".", path)} is Required")
        {
        }
    }

    public class TypeException : KApiException
    {
        public TypeException(string[] path, Types type) : base($"{string.Join(".", path)} is not type {type}")
        {
        }
    }

    public class MinException : KApiException
    {
        public MinException(string[] path, double? number) : base($"The {string.Join(".", path)} must be greater than or equal to {number}")
        {
        }
    }

    public class MaxException : KApiException
    {
        public MaxException(string[] path, double? number) : base($"The {string.Join(".", path)} must be less than or equal to {number}")
        {
        }
    }

    public class MinLengthException : KApiException
    {
        public MinLengthException(string[] path, double? number) : base($"The {string.Join(".", path)} length must be greater than or equal to {number}")
        {
        }
    }

    public class MaxLengthException : KApiException
    {
        public MaxLengthException(string[] path, double? number) : base($"The {string.Join(".", path)} length must be less than or equal to {number}")
        {
        }
    }

    public class NotMatchException : KApiException
    {
        public NotMatchException(string[] path) : base($"The {string.Join(".", path)} pattern matching failed")
        {
        }
    }

    public class InvalidException : KApiException
    {
        public InvalidException(string[] path) : base($"The {string.Join(".", path)} invalid validator")
        {
        }
    }
}
