using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public static class PipeReaderExtensions
{
    //see:  https://datatracker.ietf.org/doc/html/rfc2821#section-4.5.3.2

    private static readonly byte[] CRLF = { 13, 10 };
    private static readonly byte[] DotBlock = { 13, 10, 46, 13, 10 };

    private static async Task<byte[]> ReadUntilAsync(PipeReader reader, byte[] sequence, CancellationToken cancellationToken)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var read = await reader.ReadAsync(cancellationToken);
        var head = read.Buffer.Start;

        while (read.IsCanceled == false && read.IsCompleted == false && read.Buffer.IsEmpty == false)
        {
            if (read.Buffer.TryFind(sequence, ref head, out var tail))
            {
                byte[] result = null;
                try
                {
                    result = read.Buffer.Slice(read.Buffer.Start, head).ToArray();
                }
                finally
                {
                    reader.AdvanceTo(tail);
                }

                return result;
            }

            reader.AdvanceTo(read.Buffer.Start, read.Buffer.End);

            read = await reader.ReadAsync(cancellationToken);
        }

        if (read.IsCompleted && read.Buffer.Length == 0) return null;
        throw new InvalidDataException("Incomplete data");
    }

    private static bool TryFind(this ReadOnlySequence<byte> source, ReadOnlySpan<byte> sequence, ref SequencePosition head, out SequencePosition tail)
    {
        tail = default;

        // move to the first span
        var position = head;

        if (TryMoveNext(ref source, ref position, out var span) == false)
        {
            return false;
        }

        var index = span.IndexOf(sequence);

        if (index != -1)
        {
            head = source.GetPosition(index, head);
            tail = source.GetPosition(sequence.Length, head);

            return true;
        }

        if (source.IsSingleSegment)
        {
            // nothing else can be done here
            return false;
        }

        while (true)
        {
            tail = position;

            // move to the next span
            if (TryMoveNext(ref source, ref position, out var next) == false)
            {
                return false;
            }

            if (TryMatchAcrossBoundary(span, next, sequence, out index))
            {
                head = source.GetPosition(index, head);
                tail = source.GetPosition(sequence.Length - (span.Length - index), tail);

                return true;
            }

            span = next;
            head = tail;

            index = span.IndexOf(sequence);

            if (index != -1)
            {
                head = source.GetPosition(index, head);
                tail = source.GetPosition(sequence.Length, head);

                return true;
            }
        }
    }

    private static bool TryMoveNext(ref ReadOnlySequence<byte> source, ref SequencePosition position, out ReadOnlySpan<byte> span)
    {
        while (source.TryGet(ref position, out var memory, advance: true))
        {
            if (memory.Length > 0)
            {
                span = memory.Span;
                return true;
            }
        }

        span = default;
        return false;
    }

    private static bool TryMatchAcrossBoundary(ReadOnlySpan<byte> previous, ReadOnlySpan<byte> next, ReadOnlySpan<byte> sequence, out int index)
    {
        // we will only call this if a complete match in the previous span isnt found 
        // so we only need to start matching from one byte short of the full sequence
        var partial = sequence.Slice(0, sequence.Length - 1);

        if (TryMatchEnd(ref previous, ref partial, out index))
        {
            partial = sequence.Slice(index);

            if (next.StartsWith(partial))
            {
                // adjust the index to the position it was found in the previous span
                index = previous.Length - index;
                return true;
            }
        }

        return false;
    }

    private static bool TryMatchEnd(ref ReadOnlySpan<byte> span, ref ReadOnlySpan<byte> sequence, out int index)
    {
        var partial = sequence;

        while (partial.Length > 0)
        {
            if (span.EndsWith(partial))
            {
                index = partial.Length;
                return true;
            }

            partial = partial.Slice(0, partial.Length - 1);
        }

        index = default;
        return false;
    }

    #region Extensions

    public static async Task<byte[]> ReadLineAsync(this PipeReader reader, CancellationToken token = default)
    {
        return await ReadUntilAsync(reader, CRLF, token);
    }

    public static async Task<string> ReadLineAsync(this PipeReader reader, Encoding encoding, CancellationToken token = default)
    {
        var bytes = await ReadUntilAsync(reader, CRLF, token);
        return bytes == null ? null : encoding.GetString(bytes);
    }

    public static async Task<string> ReadLineAsync(this PipeReader reader, Encoding encoding, TimeSpan timeout)
    {
        using var source = new CancellationTokenSource();
        source.CancelAfter(timeout);

        return await ReadLineAsync(reader, encoding, source.Token);
    }

    public static async Task<byte[]> ReadToDotAsync(this PipeReader reader, CancellationToken token = default)
    {
        return await ReadUntilAsync(reader, DotBlock, token);
    }

    public static async Task<byte[]> ReadToDotAsync(this PipeReader reader, TimeSpan timeout)
    {
        using var source = new CancellationTokenSource();
        source.CancelAfter(timeout);
        return await ReadToDotAsync(reader, source.Token);
    }

    public static async Task<byte[]> ReadToEndAsync(this PipeReader reader, CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            var result = await reader.ReadAsync(token);

            if (result.IsCanceled)
            {
                break;
            }

            if (result.IsCompleted)
            {
                var block = result.Buffer.Slice(result.Buffer.Start, result.Buffer.End).ToArray();
                reader.AdvanceTo(result.Buffer.End);
                return block;
            }

            reader.AdvanceTo(result.Buffer.Start, result.Buffer.End);
        }

        throw new InvalidDataException("Incomplete read line.");
    }

    public static async Task<byte[]> ReadToAsync(this PipeReader reader, int count, CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            var result = await reader.ReadAtLeastAsync(count, token);

            if (result.IsCanceled || result.IsCompleted)
            {
                break;
            }

            var block = result.Buffer.Slice(result.Buffer.Start, count).ToArray();
            reader.AdvanceTo(result.Buffer.GetPosition(count, result.Buffer.Start));
            return block;
        }

        throw new InvalidDataException("Incomplete read line.");
    }

    public static async Task<byte[]> ReadToAsync(this PipeReader reader, int count, TimeSpan timeout)
    {
        using var source = new CancellationTokenSource();
        source.CancelAfter(timeout);
        return await ReadToAsync(reader, count, source.Token);
    }

    public static async Task WriteLineAsync(this PipeWriter writer, string line, Encoding encoding, CancellationToken token = default)
    {
        if (token == default)
        {
            using var source = new CancellationTokenSource();
            source.CancelAfter(TimeOutMilliseconds);
            token = source.Token;
        }
        var data = encoding.GetBytes(line + "\r\n");
        await writer.WriteAsync(data, token);
    }

    private static int TimeOutMilliseconds = 2 * 60 * 1000;

    public static async Task WriteLineAsync(this PipeWriter writer, string line, Encoding encoding, int TimeOutSeconds)
    {
        using var source = new CancellationTokenSource();
        source.CancelAfter(TimeOutSeconds * 1000);
        await WriteLineAsync(writer, line, encoding, source.Token);
    }

    #endregion
}