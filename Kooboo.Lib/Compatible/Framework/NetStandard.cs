//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
#if !NET45 &&!NET461
using Kooboo.Lib.Helper;
using Kooboo.Lib.Security;
using Kooboo.Lib.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Kooboo.Lib.Compatible
{
    public class NetStandard : IFramework
    {
        public string GetMimeMapping(string extension)
        {
            return MimeMapping.MimeUtility.GetMimeMapping(extension);
        }

        #region
        public RsaKeys GenerateKeys(int size)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(size);
            RsaKeys keys = new RsaKeys();
            keys.PublicKey = ToXmlString(rsa, false);
            keys.PrivateKey = ToXmlString(rsa, true);

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

                sw.Write(ToXmlString(rsa, false));
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
            FromXmlString(rsa, publickey);

            bytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
            return Convert.ToBase64String(bytes);
        }

        public string RSADecrypt(string privatekey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] bytes;
            FromXmlString(rsa, privatekey);

            bytes = rsa.Decrypt(Convert.FromBase64String(content), false);
            return Encoding.UTF8.GetString(bytes);
        }

        private void FromXmlString(RSACryptoServiceProvider rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Exponent": parameters.Exponent = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "P": parameters.P = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Q": parameters.Q = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DP": parameters.DP = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DQ": parameters.DQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "InverseQ": parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "D": parameters.D = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }

        private String ToXmlString(RSACryptoServiceProvider rsa, bool includePrivateParameters)
        {

            // we extend appropriately for private components
            RSAParameters rsaParams = rsa.ExportParameters(includePrivateParameters);
            StringBuilder sb = new StringBuilder();
            sb.Append("<RSAKeyValue>");
            // Add the modulus
            sb.Append("<Modulus>" + Convert.ToBase64String(rsaParams.Modulus) + "</Modulus>");
            // Add the exponent
            sb.Append("<Exponent>" + Convert.ToBase64String(rsaParams.Exponent) + "</Exponent>");
            if (includePrivateParameters)
            {
                // Add the private components
                sb.Append("<P>" + Convert.ToBase64String(rsaParams.P) + "</P>");
                sb.Append("<Q>" + Convert.ToBase64String(rsaParams.Q) + "</Q>");
                sb.Append("<DP>" + Convert.ToBase64String(rsaParams.DP) + "</DP>");
                sb.Append("<DQ>" + Convert.ToBase64String(rsaParams.DQ) + "</DQ>");
                sb.Append("<InverseQ>" + Convert.ToBase64String(rsaParams.InverseQ) + "</InverseQ>");
                sb.Append("<D>" + Convert.ToBase64String(rsaParams.D) + "</D>");
            }
            sb.Append("</RSAKeyValue>");
            return (sb.ToString());
        }
        #endregion

        public double GetDistance(double xLa, double xLong, double yLa, double yLong)
        {
            GeoCoordinatePortable.GeoCoordinate cordx = new GeoCoordinatePortable.GeoCoordinate(xLa, xLong);
            GeoCoordinatePortable.GeoCoordinate cordy = new GeoCoordinatePortable.GeoCoordinate(yLa, yLong);
            return cordx.GetDistanceTo(cordy);
        }

        #region image
        public SizeMeansurement GetImageSize(byte[] imagebytes)
        {
            SizeMeansurement measure = new SizeMeansurement();

            try
            {
                MemoryStream stream = new MemoryStream(imagebytes);
                var image = Image.Identify(stream);
                if (image != null)
                {
                    measure.Height = image.Height;
                    measure.Width = image.Width;
                }
            }
            catch (Exception)
            {
            }
            return measure;
        }

        public byte[] GetThumbnailImage(byte[] contentBytes, int width, int height, string format = null)
        {
            if (contentBytes == null) return null;

            MemoryStream stream = new MemoryStream(contentBytes);

            Image systhumbnail = null;

            try
            {
                var imgformate = Image.DetectFormat(stream);
                var image = Image.Load(stream);

                if (image.Width < width && image.Height < height)
                {
                    return contentBytes;
                }
                image.Mutate(x => x.Resize(width, height));
                systhumbnail = image;

                MemoryStream memstream = new MemoryStream();
                SaveImageToStream(format, systhumbnail, memstream, imgformate);
                return memstream.ToArray();
            }
            catch (UnknownImageFormatException)
            {
                return contentBytes;
            }
        }

        private static void SaveImageToStream(string format, Image image, Stream stream, IImageFormat imageFormat)
        {
            switch (format)
            {
                case "gif":
                    image.SaveAsGif(stream);
                    break;
                case "png":
                    image.SaveAsPng(stream);
                    break;
                case "jpg":
                    image.SaveAsJpeg(stream);
                    break;
                case "webp":
                    image.SaveAsWebp(stream);
                    break;
                default:
                    image.Save(stream, imageFormat);
                    break;
            }
        }

        public Stream GetThumbnailImageStream(byte[] contentBytes, int width, int height, string format = null)
        {
            if (contentBytes == null) return null;

            MemoryStream stream = new MemoryStream(contentBytes);

            Image sysThumbnail = null;

            try
            {
                var imgFormat = Image.DetectFormat(stream);
                var image = Image.Load(stream);

                if (image.Width < width && image.Height < height)
                {
                    return stream;
                }
                image.Mutate(x => x.Resize(width, height));
                sysThumbnail = image;

                MemoryStream NewStream = new MemoryStream();
                SaveImageToStream(format, image, NewStream, imgFormat);
                return NewStream;
            }
            catch (UnknownImageFormatException)
            {
                return stream;
            }
        }



        public void SaveThumbnailImage(byte[] contentBytes, int width, int height, string path)
        {
            if (contentBytes == null) return;
            MemoryStream stream = new MemoryStream(contentBytes);

            var image = Image.Load(stream);

            image.Mutate(x => x.Resize(width, height));

            image.Save(path);
        }

        public string GetThumbnailImage(string base64Str, ImageSize size)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Str);

            MemoryStream memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length);
            var image = Image.Load(memoryStream);

            memoryStream.Close();

            using (MemoryStream ms = new MemoryStream())
            {
                size = Kooboo.Lib.Helper.ImageHelper.GetEqualProportionSize(image.Width, image.Height, size);
                image.Mutate(x => x.Resize(size.Width, size.Height));
                image.Save(ms, PngFormat.Instance);

                var length = Convert.ToInt32(ms.Length);
                byte[] data = new byte[length];
                ms.Position = 0;
                ms.Read(data, 0, length);
                ms.Flush();
                return Convert.ToBase64String(data);
            }
        }

        #endregion


        public void RegisterEncoding()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public void ConsoleWait()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        public bool IsUWP()
        {
            return false;
        }

    }
}
#endif