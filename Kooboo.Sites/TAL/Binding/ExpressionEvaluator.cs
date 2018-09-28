using Kooboo.Data.Context;
using Kooboo.TAL.Functions;

namespace Kooboo.TAL.Binding
{
    public static class ExpressionEvaluator
    {
    
        public static object Evaluate(string expression, DataContext dataContext)
        {
            if (StringParser.isString(expression))
            {
                return expression;
            }

            if (FunctionParser.isExpression(expression))
            {
                var func = FunctionParser.Parse(expression);

                var renderfunction = FunctionContainer.getFunction(func.FunctionName);
                if (renderfunction == null)
                {
                    return expression;
                }

                int count = func.ParameterValues.Count;

                for (int i = 0; i < count; i++)
                {
                    object para = func.ParameterValues[i];
                    string objectstring = para.ToString();
                    if (FunctionParser.isExpression(objectstring))
                    {
                        func.ParameterValues[i] = Evaluate(objectstring, dataContext);
                    }
                    else
                    {
                        func.ParameterValues[i] = dataContext.GetValue(objectstring) ?? objectstring;
                    }
                }

                return renderfunction.Execute(func.ParameterValues.ToArray());
            }
            return dataContext.GetValue(expression);
        }
    }
}
