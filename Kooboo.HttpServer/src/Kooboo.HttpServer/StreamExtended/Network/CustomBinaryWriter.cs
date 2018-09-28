using System.IO;
using System.Threading.Tasks;

namespace StreamExtended.Network
{
    public abstract class CustomBinaryWriter
    {
        private readonly Stream stream;

        protected CustomBinaryWriter(Stream stream)
        {
            this.stream = stream;
        }

        public Task WriteAsync(byte[] data, int offset, int count)
        {
            return stream.WriteAsync(data, offset, count);
        }

        protected Task FlushAsync()
        {
            return stream.FlushAsync();
        }
    }
}
