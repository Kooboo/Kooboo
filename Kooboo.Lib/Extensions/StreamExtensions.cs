using System;
using System.IO;
using System.Threading.Tasks;

namespace Kooboo.Extensions
{
    public static class StreamExtensions
    {
        public static async Task ChunkCopyAsync(this Stream from, Stream to, long length, int chunkSize = 2046)
        {
            var buffer = new byte[chunkSize];
            var bytesRemaining = length;

            while (bytesRemaining > 0)
            {
                int count;
                if (bytesRemaining <= buffer.Length)
                {
                    count = await from.ReadAsync(buffer, 0, (int)bytesRemaining);
                }
                else
                {
                    count = await from.ReadAsync(buffer, 0, buffer.Length);
                }

                if (count == 0)
                    return;

                await to.WriteAsync(buffer, 0, count);

                bytesRemaining -= count;
            }
        }

        public static async Task ChunkCopyAsync(this byte[] from, Stream to, int chunkSize = 2046)
        {
            for (var i = 0; i < from.Length; i += chunkSize)
            {
                await to.WriteAsync(from, i, Math.Min(from.Length - i, chunkSize));
            }
        }
    }
}
