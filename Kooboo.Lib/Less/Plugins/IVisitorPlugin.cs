namespace dotless.Core.Plugins
{
    using dotless.Core.Parser.Infrastructure;
    using Parser.Tree;

    public interface IVisitorPlugin : IPlugin
    {
        Root Apply(Root tree);

        VisitorPluginType AppliesTo { get; }

        void OnPreVisiting(Env env);
        void OnPostVisiting(Env env);
    }

    public enum VisitorPluginType
    {
        BeforeEvaluation,
        AfterEvaluation
    }
}