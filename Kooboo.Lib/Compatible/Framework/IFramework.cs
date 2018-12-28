using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Lib.Utilities;
using Kooboo.Lib.Security;

namespace Kooboo.Lib.Compatible
{
    public interface IFramework
    {
        void OpenDefaultUrl(string url);

        string GetMimeMapping(string extension);

        void ConsoleWait();

        double GetDistance(double xLa, double xLong, double yLa, double yLong);

        void RegisterEncoding();

        SizeMeansurement GetImageSize(byte[] imagebytes);

        byte[] GetThumbnailImage(byte[] contentBytes, int width, int height);

        string RSADecrypt(string privatekey, string content);

        string RSAEncrypt(string publickey, string content);

        void GenerateRsa(string privateKeyPath, string publicKeyPath, int size);

        RsaKeys GenerateKeys(int size = 512);
    }
}
