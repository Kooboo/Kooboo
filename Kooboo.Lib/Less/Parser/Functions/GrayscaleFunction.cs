namespace dotless.Core.Parser.Functions
{
    using Infrastructure.Nodes;
    using System.Linq;
    using Tree;

    public class GreyscaleFunction : ColorFunctionBase
    {
        protected override Node Eval(Color color)
        {
            var grey = (color.RGB.Max() + color.RGB.Min()) / 2;

            return new Color(grey, grey, grey);
        }
    }

    public class GrayscaleFunction : GreyscaleFunction
    {
        protected override Node Eval(Color color)
        {
            WarnNotSupportedByLessJS("grayscale(color)", "greyscale(color)");
            return base.Eval(color);
        }
    }
}