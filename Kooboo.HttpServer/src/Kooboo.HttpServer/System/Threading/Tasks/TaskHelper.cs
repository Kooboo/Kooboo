using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    /// <summary>
    /// https://github.com/dotnet/corefx/blob/bffef76f6af208e2042a2f27bc081ee908bb390b/src/Common/src/System/Threading/Tasks/TaskHelpers.cs
    /// </summary>
    /// <summary>
    /// Helpers for assemblies that don't yet depend on the System.Threading.Tasks contract
    /// that includes Task.CompletedTask, Task.FromCanceled, and Task.FromException.
    /// </summary>
    internal static class TaskHelpers
    {
        private struct VoidTaskResult { }

        internal static Task FromCancellation(CancellationToken cancellationToken)
        {
            return FromCancellation<VoidTaskResult>(cancellationToken);
        }

        internal static Task<T> FromCancellation<T>(CancellationToken cancellationToken)
        {
            Debug.Assert(cancellationToken.IsCancellationRequested, "Can only create a canceled task from a cancellation token if cancellation was requested.");
            return new Task<T>(DelegateCache<T>.DefaultT, cancellationToken);
        }

        internal static Task FromException(Exception e)
        {
            return FromException<VoidTaskResult>(e);
        }

        internal static Task<T> FromException<T>(Exception e)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(e);
            return tcs.Task;
        }

        internal static Task CompletedTask()
        {
            return s_completedTask ?? (s_completedTask = Task.FromResult(default(VoidTaskResult)));
        }

        private static Task s_completedTask;

        private static class DelegateCache<T>
        {
            private static Func<T> s_defaultT;

            internal static Func<T> DefaultT
            {
                get { return s_defaultT ?? (s_defaultT = () => default(T)); }
            }
        }
    }
}
