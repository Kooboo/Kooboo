// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace System
{
    partial class SR
    {
        /// <summary>
        ///   Looks up a localized string similar to The buffer is not associated with this pool and may not be returned to it..
        /// </summary>
        internal static string ArgumentException_BufferNotFromPool
        {
            get
            {
                return ResourceManager.GetString("ArgumentException_BufferNotFromPool");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Pipe is already advanced past provided cursor..
        /// </summary>
        internal static string AdvanceToInvalidCursor
        {
            get
            {
                return ResourceManager.GetString("AdvanceToInvalidCursor");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Format specifier was invalid..
        /// </summary>
        internal static string Argument_BadFormatSpecifier
        {
            get
            {
                return ResourceManager.GetString("Argument_BadFormatSpecifier");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Characters following the format symbol must be a number of {0} or less..
        /// </summary>
        internal static string Argument_CannotParsePrecision
        {
            get
            {
                return ResourceManager.GetString("Argument_CannotParsePrecision");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Destination is too short..
        /// </summary>
        internal static string Argument_DestinationTooShort
        {
            get
            {
                return ResourceManager.GetString("Argument_DestinationTooShort");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The &apos;G&apos; format combined with a precision is not supported..
        /// </summary>
        internal static string Argument_GWithPrecisionNotSupported
        {
            get
            {
                return ResourceManager.GetString("Argument_GWithPrecisionNotSupported");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cannot use type &apos;{0}&apos;. Only value types without pointers or references are supported..
        /// </summary>
        internal static string Argument_InvalidTypeWithPointersNotSupported
        {
            get
            {
                return ResourceManager.GetString("Argument_InvalidTypeWithPointersNotSupported");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Overlapping spans have mismatching alignment..
        /// </summary>
        internal static string Argument_OverlapAlignmentMismatch
        {
            get
            {
                return ResourceManager.GetString("Argument_OverlapAlignmentMismatch");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Precision cannot be larger than {0}..
        /// </summary>
        internal static string Argument_PrecisionTooLarge
        {
            get
            {
                return ResourceManager.GetString("Argument_PrecisionTooLarge");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Advancing examined to the end would cause pipe to deadlock because FlushAsync is waiting..
        /// </summary>
        internal static string BackpressureDeadlock
        {
            get
            {
                return ResourceManager.GetString("BackpressureDeadlock");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t advance past buffer size..
        /// </summary>
        internal static string CannotAdvancePastCurrentBufferSize
        {
            get
            {
                return ResourceManager.GetString("CannotAdvancePastCurrentBufferSize");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t complete reader while reading..
        /// </summary>
        internal static string CannotCompleteWhileReading
        {
            get
            {
                return ResourceManager.GetString("CannotCompleteWhileReading");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t complete writer while writing..
        /// </summary>
        internal static string CannotCompleteWhiteWriting
        {
            get
            {
                return ResourceManager.GetString("CannotCompleteWhiteWriting");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Concurrent reads or writes are not supported..
        /// </summary>
        internal static string ConcurrentOperationsNotSupported
        {
            get
            {
                return ResourceManager.GetString("ConcurrentOperationsNotSupported");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to End position was not reached during enumeration..
        /// </summary>
        internal static string EndPositionNotReached
        {
            get
            {
                return ResourceManager.GetString("EndPositionNotReached");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t GetResult unless awaiter is completed..
        /// </summary>
        internal static string GetResultBeforeCompleted
        {
            get
            {
                return ResourceManager.GetString("GetResultBeforeCompleted");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Memory&lt;T&gt; has been disposed..
        /// </summary>
        internal static string MemoryDisposed
        {
            get
            {
                return ResourceManager.GetString("MemoryDisposed");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to No reading operation to complete..
        /// </summary>
        internal static string NoReadingOperationToComplete
        {
            get
            {
                return ResourceManager.GetString("NoReadingOperationToComplete");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Equals() on Span and ReadOnlySpan is not supported. Use operator== instead..
        /// </summary>
        internal static string NotSupported_CannotCallEqualsOnSpan
        {
            get
            {
                return ResourceManager.GetString("NotSupported_CannotCallEqualsOnSpan");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to GetHashCode() on Span and ReadOnlySpan is not supported..
        /// </summary>
        internal static string NotSupported_CannotCallGetHashCodeOnSpan
        {
            get
            {
                return ResourceManager.GetString("NotSupported_CannotCallGetHashCodeOnSpan");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to No writing operation. Make sure GetMemory() was called..
        /// </summary>
        internal static string NoWritingOperation
        {
            get
            {
                return ResourceManager.GetString("NoWritingOperation");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Release all references before disposing this instance..
        /// </summary>
        internal static string OutstandingReferences
        {
            get
            {
                return ResourceManager.GetString("OutstandingReferences");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Both reader and writer has to be completed to be able to reset the pipe..
        /// </summary>
        internal static string ReaderAndWriterHasToBeCompleted
        {
            get
            {
                return ResourceManager.GetString("ReaderAndWriterHasToBeCompleted");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Reading is not allowed after reader was completed..
        /// </summary>
        internal static string ReadingAfterCompleted
        {
            get
            {
                return ResourceManager.GetString("ReadingAfterCompleted");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Reading is already in progress..
        /// </summary>
        internal static string ReadingIsInProgress
        {
            get
            {
                return ResourceManager.GetString("ReadingIsInProgress");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Unexpected segment type..
        /// </summary>
        internal static string UnexpectedSegmentType
        {
            get
            {
                return ResourceManager.GetString("UnexpectedSegmentType");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Writing is not allowed after writer was completed..
        /// </summary>
        internal static string WritingAfterCompleted
        {
            get
            {
                return ResourceManager.GetString("WritingAfterCompleted");
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to The parameter should be a ValueTuple type of appropriate arity..
        /// </summary>
        internal static string ArgumentException_ValueTupleIncorrectType
        {
            get
            {
                return ResourceManager.GetString("ArgumentException_ValueTupleIncorrectType");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The TRest type argument of ValueTuple`8 must be a ValueTuple..
        /// </summary>
        internal static string ArgumentException_ValueTupleLastArgumentNotAValueTuple
        {
            get
            {
                return ResourceManager.GetString("ArgumentException_ValueTupleLastArgumentNotAValueTuple");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to GetHashCode() on Span and ReadOnlySpan is not supported..
        /// </summary>
        internal static string CannotCallGetHashCodeOnSpan
        {
            get
            {
                return ResourceManager.GetString("CannotCallGetHashCodeOnSpan");
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Equals() on Span and ReadOnlySpan is not supported. Use operator== instead..
        /// </summary>
        internal static string CannotCallEqualsOnSpan
        {
            get
            {
                return ResourceManager.GetString("CannotCallEqualsOnSpan");
            }
        }
    }
}