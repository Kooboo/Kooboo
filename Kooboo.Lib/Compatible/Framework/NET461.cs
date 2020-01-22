//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
#if NET461
using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Lib.Security;
using Kooboo.Lib.Utilities;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;
using Kooboo.Lib.Helper;

namespace Kooboo.Lib.Compatible
{
    public class NET461 : IFramework
    {
        public string GetMimeMapping(string extension)
        {
            return System.Web.MimeMapping.GetMimeMapping(extension);
        }
        

#region RSA
        public RsaKeys GenerateKeys(int size = 512)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(size);
            RsaKeys keys = new RsaKeys();
            keys.PublicKey = rsa.ToXmlString(false);
            keys.PrivateKey = rsa.ToXmlString(true);
            return keys;
        }

        public void GenerateRsa(string privateKeyPath, string publicKeyPath, int size)
        {
            //stream to save the keys
            FileStream fs = null;
            StreamWriter sw = null;

            //create RSA provider
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(size);

            try
            {
                //save private key
                fs = new FileStream(privateKeyPath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.Write(rsa.ToXmlString(true));
                sw.Flush();
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }

            try
            {
                //save public key
                fs = new FileStream(publicKeyPath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.Write(rsa.ToXmlString(false));
                sw.Flush();
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
            rsa.Clear();
        }

        public string RSAEncrypt(string publickey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] bytes;
            rsa.FromXmlString(publickey);

            bytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
            return Convert.ToBase64String(bytes);
        }
        public string RSADecrypt(string privatekey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] bytes;
            rsa.FromXmlString(privatekey);

            bytes = rsa.Decrypt(Convert.FromBase64String(content), false);
            return Encoding.UTF8.GetString(bytes);
        }
#endregion
        public double GetDistance(double xLa, double xLong, double yLa, double yLong)
        {
            System.Device.Location.GeoCoordinate cordx = new System.Device.Location.GeoCoordinate(xLa, xLong);
            System.Device.Location.GeoCoordinate cordy = new System.Device.Location.GeoCoordinate(yLa, yLong);
            return cordx.GetDistanceTo(cordy);
        }

#region image
        public SizeMeansurement GetImageSize(byte[] imagebytes)
        {
            SizeMeansurement measure = new SizeMeansurement();

            try
            {
                MemoryStream stream = new MemoryStream(imagebytes);
                System.Drawing.Image image = null;
                image = System.Drawing.Image.FromStream(stream);
                measure.Height = image.Height;
                measure.Width = image.Width;
            }
            catch (Exception ex)
            {

            }
            return measure;
        }

        public byte[] GetThumbnailImage(byte[] contentBytes, int width, int height)
        {
            if (contentBytes == null) return null;

            MemoryStream stream = new MemoryStream(contentBytes);

            System.Drawing.Image image = null;
            System.Drawing.Image systhumbnail = null;

            image = System.Drawing.Image.FromStream(stream);

            if (image.Width < width && image.Height < height)
            {
                return contentBytes;
            }

            systhumbnail = image.GetThumbnailImage(width, height, null, new IntPtr());

            MemoryStream memstream = new MemoryStream();
            systhumbnail.Save(memstream, System.Drawing.Imaging.ImageFormat.Png);
            return memstream.ToArray();
        }

        public void SaveThumbnailImage(byte[] contentBytes, int width, int height, string path)
        {
            if (contentBytes == null) return;
            Image image = Image.FromStream(new System.IO.MemoryStream(contentBytes));

            var thumbnail = image.GetThumbnailImage(width, height, null, new IntPtr());
            thumbnail.Save(path);
        }

        public string GetThumbnailImage(string base64Str, ImageSize size)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Str);

            MemoryStream memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(memoryStream, false);
            memoryStream.Close();

            using (MemoryStream ms = new MemoryStream())
            {
                size = Kooboo.Lib.Helper.ImageHelper.GetEqualProportionSize(image.Width, image.Height, size);
                Image thumbImage = image.GetThumbnailImage(size.Width, size.Height, null, IntPtr.Zero);
                //generate thumbImage
                thumbImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                var length = Convert.ToInt32(ms.Length);
                byte[] data = new byte[length];
                ms.Position = 0;
                ms.Read(data, 0, length);
                ms.Flush();
                return Convert.ToBase64String(data);
            }
        }
#endregion

        public void OpenDefaultUrl(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        public void RegisterEncoding()
        {
            //net45 doesn't need registerEncoding.
        }

        public void ConsoleWait()
        {
            var line = Console.ReadLine();

            while (line != null)
            {
                Console.Write(line);
                line = Console.ReadLine();
            }
        }

         public bool IsUWP()
        {
            var uwp= System.Configuration.ConfigurationManager.AppSettings.Get("IsUWP");
           
            if (string.IsNullOrEmpty(uwp))
            {
                return false; 
            }
            else
            {
                bool boolValue; 
                bool.TryParse(uwp, out boolValue);

                return boolValue; 
            } 
        }

    }
}
#endif