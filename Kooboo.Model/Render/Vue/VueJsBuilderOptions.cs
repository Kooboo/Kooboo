using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.Render.Vue
{
    public partial class VueJsBuilderOptions
    {
        public Dictionary<Type, IVueRenderer> Renderers { get; } = new Dictionary<Type, IVueRenderer>();

        public VueJsBuilderOptions UseRenderer<TElement, TRenderer>()
            where TRenderer : IVueRenderer
        {
            return UseRenderer<TElement>(Activator.CreateInstance<TRenderer>());
        }

        public VueJsBuilderOptions UseRenderer<TElement>(IVueRenderer renderer)
        {
            Renderers.Add(typeof(TElement), renderer);
            return this;
        }
    }

    partial class VueJsBuilderOptions
    {
        static VueJsBuilderOptions()
        {
            RootViewOptions = new VueJsBuilderOptions()
                .UseRenderer<Data, Data.Renderer>()
                .UseRenderer<Validation, Validation.Renderer>()
                .UseRenderer<LoadData, LoadData.RootViewRenderer>()
                .UseRenderer<El, El.Renderer>()
                .UseRenderer<SubmitData, SubmitData.Renderer>();

            SubViewOptions = new VueJsBuilderOptions()
                .UseRenderer<Data, Data.Renderer>()
                .UseRenderer<Validation, Validation.Renderer>()
                .UseRenderer<LoadData, LoadData.SubViewRenderer>()
                .UseRenderer<SubmitData, SubmitData.Renderer>();
        }

        public static VueJsBuilderOptions RootViewOptions { get; set; }

        public static VueJsBuilderOptions SubViewOptions { get; set; }
    }
}
