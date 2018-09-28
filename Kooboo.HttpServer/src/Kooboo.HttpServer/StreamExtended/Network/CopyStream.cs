using System;
using System.Threading.Tasks;
using StreamExtended.Helpers;

namespace StreamExtended.Network
{
    public class CopyStream : IBufferedStream, IDisposable
    {
        private readonly CustomBinaryReader reader;

        private readonly CustomBinaryWriter writer;

        private readonly int bufferSize;

        private int bufferLength;

        private byte[] buffer;

        private bool disposed;

        public bool DataAvailable => reader.DataAvailable;

        public long ReadBytes { get; private set; }

        public CopyStream(CustomBinaryReader reader, CustomBinaryWriter writer, int bufferSize)
        {
            this.reader = reader;
            this.writer = writer;
            this.bufferSize = bufferSize;
            buffer = BufferPool.GetBuffer(bufferSize);
        }

        public async Task<bool> FillBufferAsync()
        {
            await FlushAsync();
            return await reader.FillBufferAsync();
        }

        public async Task FlushAsync()
        {
            //send out the current data from from the buffer
            if (bufferLength > 0)
            {
                await writer.WriteAsync(buffer, 0, bufferLength);
                bufferLength = 0;
            }
        }

        public byte ReadByteFromBuffer()
        {
            byte b = reader.ReadByteFromBuffer();
            buffer[bufferLength++] = b;
            ReadBytes++;
            return b;
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            int result = await reader.ReadBytesAsync(buffer, offset, count);
            if (result > 0)
            {
                if (bufferLength + result > bufferSize)
                {
                    await FlushAsync();
                }

                Buffer.BlockCopy(buffer, offset, this.buffer, bufferLength, result);
                bufferLength += result;
                ReadBytes += result;
                await FlushAsync();
            }

            return result;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                var buf = buffer;
                buffer = null;
                BufferPool.ReturnBuffer(buf);
            }
        }
    }
}
