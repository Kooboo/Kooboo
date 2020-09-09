using Jint;
using Jint.Runtime.Debugger;
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Kooboo.Sites.ScriptDebugger
{
    public class DebugSession
    {
        public class Breakpoint
        {
            public Guid codeId { get; set; }
            public int Line { get; set; }
        }

        public enum GetWay
        {
            Normal,
            AutoCreate,
            CurrentContext
        }

        public List<Breakpoint> BreakLines { get; set; } = new List<Breakpoint>();
        public Jint.Engine JsEngine { get; set; }
        public bool End { get; set; }

        public Guid? CurrentCodeId { get; set; }

        public DateTime LastRefreshTime { get; set; } = DateTime.UtcNow;

        public DebugInformation DebugInfo { get; set; }

        public Exception Exception { get; set; }

        public StepMode StepMode { get; set; } = StepMode.None;

        public RenderContext DebuggingContext { get; set; }

        public RenderContext CurrentContext { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

        public void Next(StepMode stepMode)
        {
            StepMode = stepMode;
            CancellationTokenSource.Cancel();
            CancellationTokenSource = new CancellationTokenSource();
        }

        public void Clear()
        {
            DebugInfo = null;
            Exception = null;
            StepMode = StepMode.Out;
            if (CancellationTokenSource != null) CancellationTokenSource.Cancel();
            CancellationTokenSource = new CancellationTokenSource();
            StepMode = StepMode.None;
            CurrentCodeId = null;
            End = false;
            CurrentContext = null;
            DebuggingContext = null;
        }

    }
}
