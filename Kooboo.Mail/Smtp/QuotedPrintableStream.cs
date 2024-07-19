//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;

namespace Kooboo.Mail.Smtp
{
    public class QuotedPrintableStream : Stream
    {
        private Stream _stream = null;
        private byte[] _buffer = null;
        private int _count = 0;

        public QuotedPrintableStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            _stream = stream;

            _buffer = new byte[78];
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override void Flush()
        {
            if (_count > 0)
            {
                _stream.Write(_buffer, 0, _count);
                _count = 0;
                _buffer = new byte[78];
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Encodes a sequence of bytes, writes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>buffer</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <exception cref="NotSupportedException">Is raised when reading not supported.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentException("Invalid argument 'offset' value.");
            }
            if (offset + count > buffer.Length)
            {
                throw new ArgumentException("Invalid argument 'count' value.");
            }

            // Process bytes.
            for (int i = 0; i < count; i++)
            {
                byte b = buffer[offset + i];

                // We don't need to encode byte.
                if ((b >= 33 && b <= 60) || (b >= 62 && b <= 126))
                {
                    // Maximum allowed quoted-printable line length reached, do soft line break.
                    if (_count >= 75)
                    {
                        _buffer[_count++] = (byte)'=';
                        _buffer[_count++] = (byte)'\r';
                        _buffer[_count++] = (byte)'\n';

                        // Write encoded data to underlying stream.
                        Flush();
                    }
                    _buffer[_count++] = b;
                }
                // We need to encode byte.
                else
                {
                    // Maximum allowed quote-printable line length reached, do soft line break.
                    if (_count >= 73)
                    {
                        _buffer[_count++] = (byte)'=';
                        _buffer[_count++] = (byte)'\r';
                        _buffer[_count++] = (byte)'\n';

                        // Write encoded data to underlying stream.
                        Flush();
                    }

                    // Encode byte.
                    _buffer[_count++] = (byte)'=';
                    _buffer[_count++] = (byte)(b >> 4).ToString("X")[0];
                    _buffer[_count++] = (byte)(b & 0xF).ToString("X")[0];
                }
            }

            Flush();
        }

    }
}