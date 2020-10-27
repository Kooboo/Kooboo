using Newtonsoft.Json;
using System;
using System.Text;

namespace Kooboo.Sites.SMS.ChinaMobile.Helpers
{
    public class Base64Helper
    {
        /// <summary>
        /// Encode object to Base64 string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Encode<T>(T obj) where T : class
        {
            var requestString = JsonConvert.SerializeObject(obj);
            var requestBytes = Encoding.UTF8.GetBytes(requestString);
            return Convert.ToBase64String(requestBytes);
        }
    }
}
