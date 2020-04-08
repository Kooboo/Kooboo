using Jint;
using Jint.Parser;
using Jint.Parser.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Extensions
{
    public static class JsEngineExtensions
    {
        private static string GetInnerError(Exception ex)
        {
            var error = ex.Message;
            if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
            {
                error += " " + ex.InnerException.Message;
            }

            return error;
        }

        public static Engine ExecuteWithErrorHandle(this Engine engine, string source, ParserOptions parserOptions = null)
        {
            try
            {
                if (parserOptions != null) return engine.Execute(source, parserOptions);
                return engine.Execute(source);
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex);
                throw new Exception(GetInnerError(ex));
            }

        }

        public static Engine ExecuteWithErrorHandle(this Engine engine, Program program)
        {
            try
            {
                return engine.Execute(program);
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex);
                throw new Exception(GetInnerError(ex));
            }
        }
    }
}
