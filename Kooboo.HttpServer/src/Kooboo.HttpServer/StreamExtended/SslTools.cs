using StreamExtended.Models;
using StreamExtended.Network;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StreamExtended
{
    public class SslTools
    {
        public static async Task<bool> IsClientHello(CustomBufferedStream stream)
        {
            var clientHello = await PeekClientHello(stream);
            return clientHello != null;
        }

        public static async Task<ClientHelloInfo> PeekClientHello(CustomBufferedStream clientStream)
        {
            //detects the HTTPS ClientHello message as it is described in the following url:
            //https://stackoverflow.com/questions/3897883/how-to-detect-an-incoming-ssl-https-handshake-ssl-wire-format

            int recordType = await clientStream.PeekByteAsync(0);
            if (recordType == -1)
            {
                return null;
            }

            if ((recordType & 0x80) == 0x80)
            {
                //SSL 2
                var peekStream = new CustomBufferedPeekStream(clientStream, 1);

                // length value + minimum length
                if (!await peekStream.EnsureBufferLength(10))
                {
                    return null;
                }

                int length = ((recordType & 0x7f) << 8) + peekStream.ReadByte();
                if (length < 9)
                {
                    // Message body too short.
                    return null;
                }

                if (peekStream.ReadByte() != 0x01)
                {
                    // should be ClientHello
                    return null;
                }

                int majorVersion = peekStream.ReadByte();
                int minorVersion = peekStream.ReadByte();

                int ciphersCount = peekStream.ReadInt16() / 3;
                int sessionIdLength = peekStream.ReadInt16();
                int randomLength = peekStream.ReadInt16();

                if (!await peekStream.EnsureBufferLength(ciphersCount * 3 + sessionIdLength + randomLength))
                {
                    return null;
                }

                int[] ciphers = new int[ciphersCount];
                for (int i = 0; i < ciphers.Length; i++)
                {
                    ciphers[i] = (peekStream.ReadByte() << 16) + (peekStream.ReadByte() << 8) + peekStream.ReadByte();
                }

                byte[] sessionId = peekStream.ReadBytes(sessionIdLength);
                byte[] random = peekStream.ReadBytes(randomLength);

                var clientHelloInfo = new ClientHelloInfo
                {
                    HandshakeVersion = 2,
                    MajorVersion = majorVersion,
                    MinorVersion = minorVersion,
                    Random = random,
                    SessionId = sessionId,
                    Ciphers = ciphers,
                    ClientHelloLength = peekStream.Position,
                };

                return clientHelloInfo;
            }
            else if (recordType == 0x16)
            {
                var peekStream = new CustomBufferedPeekStream(clientStream, 1);

                //should contain at least 43 bytes
                // 2 version + 2 length + 1 type + 3 length(?) + 2 version +  32 random + 1 sessionid length
                if (!await peekStream.EnsureBufferLength(43))
                {
                    return null;
                }

                //SSL 3.0 or TLS 1.0, 1.1 and 1.2
                int majorVersion = peekStream.ReadByte();
                int minorVersion = peekStream.ReadByte();

                int length = peekStream.ReadInt16();

                if (peekStream.ReadByte() != 0x01)
                {
                    // should be ClientHello
                    return null;
                }

                length = peekStream.ReadInt24();

                majorVersion = peekStream.ReadByte();
                minorVersion = peekStream.ReadByte();

                byte[] random = peekStream.ReadBytes(32);
                length = peekStream.ReadByte();

                // sessionid + 2 ciphersData length
                if (!await peekStream.EnsureBufferLength(length + 2))
                {
                    return null;
                }

                byte[] sessionId = peekStream.ReadBytes(length);

                length = peekStream.ReadInt16();

                // ciphersData + compressionData length
                if (!await peekStream.EnsureBufferLength(length + 1))
                {
                    return null;
                }

                byte[] ciphersData = peekStream.ReadBytes(length);
                int[] ciphers = new int[ciphersData.Length / 2];
                for (int i = 0; i < ciphers.Length; i++)
                {
                    ciphers[i] = (ciphersData[2 * i] << 8) + ciphersData[2 * i + 1];
                }

                length = peekStream.ReadByte();
                if (length < 1)
                {
                    return null;
                }

                // compressionData
                if (!await peekStream.EnsureBufferLength(length))
                {
                    return null;
                }

                byte[] compressionData = peekStream.ReadBytes(length);

                int extenstionsStartPosition = peekStream.Position;

                var extensions = await ReadExtensions(majorVersion, minorVersion, peekStream);

                var clientHelloInfo = new ClientHelloInfo
                {
                    HandshakeVersion = 3,
                    MajorVersion = majorVersion,
                    MinorVersion = minorVersion,
                    Random = random,
                    SessionId = sessionId,
                    Ciphers = ciphers,
                    CompressionData = compressionData,
                    ClientHelloLength = peekStream.Position,
                    EntensionsStartPosition = extenstionsStartPosition,
                    Extensions = extensions,
                };

                return clientHelloInfo;
            }

            return null;
        }

        private static async Task<Dictionary<string, SslExtension>> ReadExtensions(int majorVersion, int minorVersion, CustomBufferedPeekStream peekStream)
        {
            Dictionary<string, SslExtension> extensions = null;
            if (majorVersion > 3 || majorVersion == 3 && minorVersion >= 1)
            {
                if (await peekStream.EnsureBufferLength(2))
                {
                    int extensionsLength = peekStream.ReadInt16();

                    if (await peekStream.EnsureBufferLength(extensionsLength))
                    {
                        extensions = new Dictionary<string, SslExtension>();
                        int idx = 0;
                        while (extensionsLength > 3)
                        {
                            int id = peekStream.ReadInt16();
                            int length = peekStream.ReadInt16();
                            byte[] data = peekStream.ReadBytes(length);
                            var extension = SslExtensions.GetExtension(id, data, idx++);
                            extensions[extension.Name] = extension;
                            extensionsLength -= 4 + length;
                        }
                    }
                }
            }

            return extensions;
        }

        public static async Task<bool> IsServerHello(CustomBufferedStream stream)
        {
            var serverHello = await PeekServerHello(stream);
            return serverHello != null;
        }

        public static async Task<ServerHelloInfo> PeekServerHello(CustomBufferedStream serverStream)
        {
            //detects the HTTPS ClientHello message as it is described in the following url:
            //https://stackoverflow.com/questions/3897883/how-to-detect-an-incoming-ssl-https-handshake-ssl-wire-format

            int recordType = await serverStream.PeekByteAsync(0);
            if (recordType == -1)
            {
                return null;
            }

            if ((recordType & 0x80) == 0x80)
            {
                //SSL 2
                // not tested. SSL2 is deprecated
                var peekStream = new CustomBufferedPeekStream(serverStream, 1);

                // length value + minimum length
                if (!await peekStream.EnsureBufferLength(39))
                {
                    return null;
                }

                int length = ((recordType & 0x7f) << 8) + peekStream.ReadByte();
                if (length < 38)
                {
                    // Message body too short.
                    return null;
                }

                if (peekStream.ReadByte() != 0x04)
                {
                    // should be ServerHello
                    return null;
                }

                int majorVersion = peekStream.ReadByte();
                int minorVersion = peekStream.ReadByte();

                // 32 bytes random + 1 byte sessionId + 2 bytes cipherSuite
                if (!await peekStream.EnsureBufferLength(35))
                {
                    return null;
                }

                byte[] random = peekStream.ReadBytes(32);
                byte[] sessionId = peekStream.ReadBytes(1);
                int cipherSuite = peekStream.ReadInt16();

                var serverHelloInfo = new ServerHelloInfo
                {
                    HandshakeVersion = 2,
                    MajorVersion = majorVersion,
                    MinorVersion = minorVersion,
                    Random = random,
                    SessionId = sessionId,
                    CipherSuite = cipherSuite,
                    ServerHelloLength = peekStream.Position,
                };

                return serverHelloInfo;
            }
            else if (recordType == 0x16)
            {
                var peekStream = new CustomBufferedPeekStream(serverStream, 1);

                //should contain at least 43 bytes
                // 2 version + 2 length + 1 type + 3 length(?) + 2 version +  32 random + 1 sessionid length
                if (!await peekStream.EnsureBufferLength(43))
                {
                    return null;
                }

                //SSL 3.0 or TLS 1.0, 1.1 and 1.2
                int majorVersion = peekStream.ReadByte();
                int minorVersion = peekStream.ReadByte();

                int length = peekStream.ReadInt16();

                if (peekStream.ReadByte() != 0x02)
                {
                    // should be ServerHello
                    return null;
                }

                length = peekStream.ReadInt24();

                majorVersion = peekStream.ReadByte();
                minorVersion = peekStream.ReadByte();

                byte[] random = peekStream.ReadBytes(32);
                length = peekStream.ReadByte();

                // sessionid + cipherSuite + compressionMethod
                if (!await peekStream.EnsureBufferLength(length + 2 + 1))
                {
                    return null;
                }

                byte[] sessionId = peekStream.ReadBytes(length);

                int cipherSuite = peekStream.ReadInt16();
                byte compressionMethod = peekStream.ReadByte();

                int extenstionsStartPosition = peekStream.Position;

                var extensions = await ReadExtensions(majorVersion, minorVersion, peekStream);

                //var rawBytes = new CustomBufferedPeekStream(serverStream).ReadBytes(peekStream.Position);

                var serverHelloInfo = new ServerHelloInfo
                {
                    HandshakeVersion = 3,
                    MajorVersion = majorVersion,
                    MinorVersion = minorVersion,
                    Random = random,
                    SessionId = sessionId,
                    CipherSuite = cipherSuite,
                    CompressionMethod = compressionMethod,
                    ServerHelloLength = peekStream.Position,
                    EntensionsStartPosition = extenstionsStartPosition,
                    Extensions = extensions,
                };

                return serverHelloInfo;
            }

            return null;
        }
    }
}
