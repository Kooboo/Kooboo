using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Extensions
{
    public static class PipeHelper
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(this ReadOnlySequence<byte> buffer, Encoding encoding)
        {
            if (buffer.IsSingleSegment)
            {
                return encoding.GetString(buffer.First.Span);
            }

            return string.Create((int)buffer.Length, buffer, (span, sequence) =>
            {
                foreach (var segment in sequence)
                {
                    encoding.GetChars(segment.Span, span);
                    span = span.Slice(segment.Length);
                }
            });
        }


        public static SequencePosition? PositionOf<T>(this ReadOnlySequence<T> source, ReadOnlySpan<T> value) where T : IEquatable<T>
        {
            if (source.IsEmpty || value.IsEmpty)
                return null;

            if (source.IsSingleSegment)
            {
                var index = source.First.Span.IndexOf(value);
                if (index > -1)
                    return source.GetPosition(index);
                else
                    return null;
            }

            return PositionOfMultiSegment(source, value);
        }

        public static SequencePosition? PositionOfMultiSegment<T>(in ReadOnlySequence<T> source, ReadOnlySpan<T> value) where T : IEquatable<T>
        {
            var firstVal = value[0];

            SequencePosition position = source.Start;
            SequencePosition result = position;
            while (source.TryGet(ref position, out ReadOnlyMemory<T> memory))
            {
                var offset = 0;
                while (offset < memory.Length)
                {
                    var index = memory.Span.Slice(offset).IndexOf(firstVal);
                    if (index == -1)
                        break;

                    var candidatePos = source.GetPosition(index + offset, result);
                    if (source.MatchesFrom(value, candidatePos))
                        return candidatePos;

                    offset += index + 1;
                }
                if (position.GetObject() == null)
                {
                    break;
                }
                result = position;
            }

            return null;
        }

        public static bool MatchesFrom<T>(in this ReadOnlySequence<T> source, ReadOnlySpan<T> value, SequencePosition? position = null) where T : IEquatable<T>
        {
            var slice = position == null ? source : source.Slice(position.Value);
            if (slice.Length < value.Length)
                return false;

            var candidate = slice.Slice(0, value.Length);

            int i = 0;
            foreach (var sequence in candidate)
            {
                foreach (var entry in sequence.Span)
                {
                    if (!entry.Equals(value[i++]))
                        return false;
                }
            }
            return true;
        }


        private static async ValueTask<string> ReadLineAsync2(this PipeReader reader)
        {
            ReadResult result = await reader.ReadAsync();

            while (!result.IsCanceled && !result.IsCompleted && !result.Buffer.IsEmpty)
            {
                ReadOnlySequence<byte> buffer = result.Buffer;
                SequencePosition? position = buffer.PositionOf((byte)'\n');

                if (position != null && position.HasValue)
                {
                    var bytes = buffer.Slice(0, position.Value);

                    reader.AdvanceTo(position.Value);
                    ///reader.Complete();
                    return PipeHelper.GetString(bytes, System.Text.Encoding.ASCII);
                }
            }


            return null;
        }
    }
}
