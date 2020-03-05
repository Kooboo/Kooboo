using System;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Adyen.Lib
{
    public class AdyenSignatureValidator
    {
        private static string _oldKeyCache;
        private static string _currentKey;
        private static byte[] _currentKeyBytes;

        public AdyenSignatureValidator(string key)
        {
            if (_currentKey == key)
            {
                return;
            }

            // key changed
            if (_currentKey != null)
            {
                _oldKeyCache = _currentKey;
            }

            _currentKeyBytes = ConvertFromHex(key);
            _currentKey = key;
        }

        public bool Validate(AdyenNotification.NotificationRequestItem body)
        {
            object hmacSignature = null;
            if (!body.AdditionalData?.TryGetValue("hmacSignature", out hmacSignature) == true)
            {
                return false;
            }

            var signature = (string)hmacSignature;
            var payload = ConcatenatePayload(body);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var hash = HmacHash(_currentKeyBytes, payloadBytes);
            if (hash == signature)
            {
                // new key working, clear cache
                _oldKeyCache = null;
                return true;
            }

            if (_oldKeyCache == null)
            {
                return false;
            }

            return HmacHash(ConvertFromHex(_oldKeyCache), payloadBytes) == signature;
        }

        private static string ConcatenatePayload(AdyenNotification.NotificationRequestItem body)
        {
            return
                $"{body.PspReference}:{body.OriginalReference}:{body.MerchantAccountCode}:{body.MerchantReference}:{body.Amount.Value}:{body.Amount.Currency}:{body.EventCode}:{body.Success.ToString().ToLower()}";
        }

        private static string HmacHash(byte[] keyBytes, byte[] payloadBytes)
        {
            using (var hmacsha256 = new System.Security.Cryptography.HMACSHA256(keyBytes))
            {
                var hash = hmacsha256.ComputeHash(payloadBytes);
                return Convert.ToBase64String(hash);
            }
        }

        private static byte[] ConvertFromHex(string key)
        {
            if ((key.Length % 2) == 1)
            {
                key += '0';
            }

            var bytes = new byte[key.Length / 2];
            for (var i = 0; i < key.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(key.Substring(i, 2), 16);
            }

            return bytes;
        }
    }
}