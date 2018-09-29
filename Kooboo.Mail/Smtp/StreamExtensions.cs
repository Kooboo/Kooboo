//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Mail.Smtp;

namespace Kooboo.Mail.Smtp
{
    public static class StreamExtensions
    {
        private const int DefaultBufferSize = 1024 * 10;
        private static byte[] EndBytes = new byte[] { 0x0D, 0x0A, 0x2E, 0x0D, 0x0A };
        private static byte EndByte = EndBytes[EndBytes.Length - 1];

        public static async Task<byte[]> ReadToDotLine(this Stream stream, TimeSpan? timeout = null, int bufferSize = DefaultBufferSize)
        {
            using (var ms = new MemoryStream())
            {
                await DoRead(stream, ms, timeout, bufferSize);

                var result = ms.ToArray();
                return result.Take(result.Length - EndBytes.Length).ToArray();
            }
        }

        private static async Task DoRead(Stream from, Stream to, TimeSpan? timeout, int bufferSize)
        {
            var prev = new byte[EndBytes.Length - 1];
            var buffer = new byte[bufferSize];
            var length = 0;

            var task = timeout == null ? null : Task.Delay(timeout.Value);
            do
            {
                var readLen = await from.ReadAsync(buffer, length, buffer.Length - length);
                // 如果暂时未读到数据，则循环读取
                if (readLen == 0)
                    continue;

                // 检查读的buffer中是否包含结束byte，有则返回结束点所在buffer的长度
                var endLen = CheckEnd(prev, buffer, length, readLen);
                if (endLen != -1)
                {
                    to.Write(buffer, 0, endLen);
                    return;
                }

                // 不包含结束byte，则继续装填buffer
                length += readLen;

                // Buffer满，则先flush到目标stream
                if (length == buffer.Length)
                {
                    // 存最后一小段，以备回溯匹配需要
                    Buffer.BlockCopy(buffer, buffer.Length - prev.Length, prev, 0, prev.Length);

                    // Flush到目标stream
                    to.Write(buffer, 0, buffer.Length);

                    length = 0;
                }
            }
            while (timeout == null || !task.IsCompleted);

            throw new TimeoutException();
        }

        private static int CheckEnd(byte[] prev, byte[] buffer, int length, int readLen)
        {
            for (int i = length; i < length + readLen; i++)
            {
                // 挨个字符检查是否是结束字符，是则回溯匹配整个dot结束串
                if (buffer[i] == EndByte)
                {
                    if (LookBack(prev, buffer, i))
                        return i + 1;
                }
            }

            return -1;
        }

        private static bool LookBack(byte[] prev, byte[] buffer, int i)
        {
            // 先匹配buffer
            var k = EndBytes.Length - 1;
            for (int j = i; j >= 0 && k >= 0; j--, k--)
            {
                if (buffer[j] != EndBytes[k])
                    return false;
            }

            // buffer到头仍然匹配成功，继续匹配之前的部分数据
            if (k >= 0)
            {
                for (int j = prev.Length - 1; j >= 0 && k >= 0; j--, k--)
                {
                    if (prev[j] != EndBytes[k])
                        return false;
                }
            }

            return true;
        }
    }
}
