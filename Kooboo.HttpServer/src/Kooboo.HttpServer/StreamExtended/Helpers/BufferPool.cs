using System.Collections.Concurrent;

namespace StreamExtended.Helpers
{
    public static class BufferPool
    {
        private static readonly ConcurrentQueue<byte[]> buffers = new ConcurrentQueue<byte[]>();

        /// <summary>
        /// Gets a buffer.
        /// </summary>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns></returns>
        public static byte[] GetBuffer(int bufferSize)
        {
            if (!buffers.TryDequeue(out var buffer) || buffer.Length != bufferSize)
            {
                buffer = new byte[bufferSize];
            }

            return buffer;
        }

        /// <summary>
        /// Returns the buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public static void ReturnBuffer(byte[] buffer)
        {
            if (buffer != null)
            {
                buffers.Enqueue(buffer);
            }
        }
    }
}
