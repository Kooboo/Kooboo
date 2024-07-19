//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using Kooboo.Lib.Helper;
using Kooboo.Lib.Security;
using Kooboo.Lib.Utilities;

namespace Kooboo.Lib.Compatible
{
    public interface IFramework
    {
        string GetMimeMapping(string extension);

        void ConsoleWait();

        double GetDistance(double xLa, double xLong, double yLa, double yLong);

        void RegisterEncoding();

        SizeMeansurement GetImageSize(byte[] imagebytes);

        byte[] GetThumbnailImage(byte[] contentBytes, int width, int height, string format = null);
        Stream GetThumbnailImageStream(byte[] contentBytes, int width, int height, string format = null);

        void SaveThumbnailImage(byte[] contentBytes, int width, int height, string path);

        string GetThumbnailImage(string base64Str, ImageSize size);

        string RSADecrypt(string privatekey, string content);

        string RSAEncrypt(string publickey, string content);

        void GenerateRsa(string privateKeyPath, string publicKeyPath, int size);

        RsaKeys GenerateKeys(int size = 512);

        bool IsUWP();

    }
}
