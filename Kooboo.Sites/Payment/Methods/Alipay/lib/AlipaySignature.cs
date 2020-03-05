using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Alipay.lib
{
    public class AlipaySignature
    {
        /** 默认编码字符集 */
        private static readonly string DEFAULT_CHARSET = "GBK";

        public static string RSASign(string data, string privateKeyPem, string charset, string signType)
        {
            return RSASignCharSet(data, privateKeyPem, charset, signType);
        }

        //*/
        public static string RSASignCharSet(string data, string privateKeyPem, string charset, string signType)
        {
            RSA rsaCsp = LoadCertificateString(privateKeyPem, signType);
            byte[] dataBytes = null;
            if (string.IsNullOrEmpty(charset))
                dataBytes = Encoding.UTF8.GetBytes(data);
            else
                dataBytes = Encoding.GetEncoding(charset).GetBytes(data);

            var signatureBytes = rsaCsp.SignData(dataBytes, "RSA2".Equals(signType) ? HashAlgorithmName.SHA256 : HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signatureBytes);
        }

        public static bool RSACheckContent(string signContent, string sign, string publicKeyPem, string charset,
            string signType)
        {
            try
            {
                if (string.IsNullOrEmpty(charset))
                    charset = DEFAULT_CHARSET;

                string sPublicKeyPEM;
                sPublicKeyPEM = "-----BEGIN PUBLIC KEY-----\r\n";
                sPublicKeyPEM += publicKeyPem;
                sPublicKeyPEM += "-----END PUBLIC KEY-----\r\n\r\n";


                if ("RSA2".Equals(signType))
                {
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.PersistKeyInCsp = false;
                    RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);

                    bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), "SHA256", Convert.FromBase64String(sign));
                    return bVerifyResultOriginal;
                }
                else
                {

                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.PersistKeyInCsp = false;
                    RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);

                    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), sha1, Convert.FromBase64String(sign));
                    return bVerifyResultOriginal;
                }
            }
            catch
            {
                return false;
            }
        }

        public static RSA LoadCertificateString(string strKey, string signType)
        {
            byte[] data = null;
            //读取带
            //ata = Encoding.Default.GetBytes(strKey);
            data = Convert.FromBase64String(strKey);
            //data = GetPem("RSA PRIVATE KEY", data);
            try
            {
                var rsa = DecodeRSAPrivateKey(data, signType);
                return rsa;
            }
            catch (Exception ex)
            {
                throw new AliPayException("Alipay.AopSdk.Core.Util.AlipaySignature LoadCertificateString DecodeRSAPrivateKey Error. Error message:" + ex.Message);
            }
        }

        private static RSA DecodeRSAPrivateKey(byte[] privkey, string signType)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            var mem = new MemoryStream(privkey);
            var binr = new BinaryReader(mem); //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            var elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte(); //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16(); //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);


                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                var CspParameters = new CspParameters();
                CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;

                var bitLen = 1024;
                if ("RSA2".Equals(signType))
                    bitLen = 2048;

                var rsa = RSA.Create();
                rsa.KeySize = bitLen;
                var rsAparams = new RSAParameters();
                rsAparams.Modulus = MODULUS;
                rsAparams.Exponent = E;
                rsAparams.D = D;
                rsAparams.P = P;
                rsAparams.Q = Q;
                rsAparams.DP = DP;
                rsAparams.DQ = DQ;
                rsAparams.InverseQ = IQ;
                rsa.ImportParameters(rsAparams);
                return rsa;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            var count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02) //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
            {
                count = binr.ReadByte(); // data size in next byte
            }
            else if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt; // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
                //remove high order zeros in data
                count -= 1;
            binr.BaseStream.Seek(-1, SeekOrigin.Current); //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }
    }
}
