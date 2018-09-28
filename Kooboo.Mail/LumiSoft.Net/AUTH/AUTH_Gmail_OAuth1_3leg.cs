using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.Security.Cryptography;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// This class implements Google Gmail OAUTH version 1.0.
    /// </summary>
    public class AUTH_Gmail_OAuth1_3leg
    {
        private string m_ConsumerKey        = null;
        private string m_ConsumerSecret     = null;
        private string m_Scope              = "https://mail.google.com/ https://www.googleapis.com/auth/userinfo.email";        
        private string m_RequestToken       = null;
        private string m_RequestTokenSecret = null;
        private string m_AccessToken        = null;
        private string m_AccessTokenSecret  = null;
        private string m_Email              = null;
        private Random m_pRandom            = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AUTH_Gmail_OAuth1_3leg() : this("anonymous","anonymous")
        {            
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="consumerKey">OAuth consumer key.</param>
        /// <param name="consumerSecret">OAuth consumer secret.</param>
        /// <exception cref="ArgumentNullException">Is riased when <b>consumerKey</b> or <b>consumerSecret</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is riased when any of the arguments has invalid value.</exception>
        public AUTH_Gmail_OAuth1_3leg(string consumerKey,string consumerSecret)
        {
            if(consumerKey == null){
                throw new ArgumentNullException("consumerKey");
            }
            if(consumerKey == ""){
                throw new ArgumentException("Argument 'consumerKey' value must be specified.","consumerKey");
            }
            if(consumerSecret == null){
                throw new ArgumentNullException("consumerSecret");
            }
            if(consumerSecret == ""){
                throw new ArgumentException("Argument 'consumerSecret' value must be specified.","consumerSecret");
            }

            m_ConsumerKey    = consumerKey;
            m_ConsumerSecret = consumerSecret;

            m_pRandom = new Random();
        }


        #region method GetRequestToken

        /// <summary>
        /// Gets Gmail request Token.
        /// </summary>
        /// <exception cref="InvalidOperationException">Is raised when this method is called in invalid state.</exception>
        public void GetRequestToken()
        {
            GetRequestToken("oob");
        }

        /// <summary>
        /// Gets Gmail request Token.
        /// </summary>
        /// <param name="callback">OAuth callback Url.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>callback</b> is null reference.</exception>
        /// <exception cref="InvalidOperationException">Is raised when this method is called in invalid state.</exception>
        public void GetRequestToken(string callback)
        {
            if(callback == null){
                throw new ArgumentNullException("callback");
            }
            if(!string.IsNullOrEmpty(m_RequestToken)){
                throw new InvalidOperationException("Invalid state, you have already called this 'GetRequestToken' method.");
            }

            // For more info see: http://googlecodesamples.com/oauth_playground/

            string timestamp = GenerateTimeStamp();
            string nonce     = GenerateNonce();
                                    
            string url    = "https://www.google.com/accounts/OAuthGetRequestToken?scope=" + UrlEncode(m_Scope);
            string sigUrl = "https://www.google.com/accounts/OAuthGetRequestToken";

            // Build signature base.
            StringBuilder xxx = new StringBuilder();
            xxx.Append("oauth_callback=" + UrlEncode(callback));
            xxx.Append("&oauth_consumer_key=" + UrlEncode(m_ConsumerKey));
            xxx.Append("&oauth_nonce=" + UrlEncode(nonce));
            xxx.Append("&oauth_signature_method=" + UrlEncode("HMAC-SHA1"));
            xxx.Append("&oauth_timestamp=" + UrlEncode(timestamp));
            xxx.Append("&oauth_version=" + UrlEncode("1.0"));
            xxx.Append("&scope=" + UrlEncode(m_Scope));
            string signatureBase = "GET" + "&" + UrlEncode(sigUrl) + "&" +  UrlEncode(xxx.ToString());

            // Calculate signature.
            string signature = ComputeHmacSha1Signature(signatureBase,m_ConsumerSecret,null);
                        
            //Build Authorization header.
            StringBuilder authHeader = new StringBuilder();
            authHeader.Append("Authorization: OAuth ");
            authHeader.Append("oauth_version=\"1.0\", ");
            authHeader.Append("oauth_nonce=\"" + nonce + "\", ");
            authHeader.Append("oauth_timestamp=\"" + timestamp + "\", ");
            authHeader.Append("oauth_consumer_key=\"" + m_ConsumerKey + "\", ");
            authHeader.Append("oauth_callback=\"" + UrlEncode(callback) + "\", ");
            authHeader.Append("oauth_signature_method=\"HMAC-SHA1\", ");
            authHeader.Append("oauth_signature=\"" + UrlEncode(signature) + "\"");

            // Create web request and read response.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add(authHeader.ToString());
            using(WebResponse response = request.GetResponse()){
                using(StreamReader reader = new StreamReader(response.GetResponseStream())){
                    foreach(string parameter in HttpUtility.UrlDecode(reader.ReadToEnd()).Split('&')){
                        string[] name_value = parameter.Split('=');
                        if(string.Equals(name_value[0],"oauth_token",StringComparison.InvariantCultureIgnoreCase)){
                            m_RequestToken = name_value[1];
                        }
                        else if(string.Equals(name_value[0],"oauth_token_secret",StringComparison.InvariantCultureIgnoreCase)){
                            m_RequestTokenSecret = name_value[1];
                        }
                    }
                }
            }
        }

        #endregion

        #region method GetAuthorizationUrl

        /// <summary>
        /// Gets Gmail authorization Url.
        /// </summary>
        /// <returns>Returns Gmail authorization Url.</returns>
        public string GetAuthorizationUrl()
        {
            if(m_RequestToken == null){
                throw new InvalidOperationException("You need call method 'GetRequestToken' before.");
            }

            return "https://accounts.google.com/OAuthAuthorizeToken?oauth_token=" + UrlEncode(m_RequestToken) + "&hd=default";
        }

        #endregion

        #region method GetAccessToken

        /// <summary>
        /// Gets Gmail access token.
        /// </summary>
        /// <param name="verificationCode">Google provided verfification code on authorization Url.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>verificationCode</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <exception cref="InvalidOperationException">Is raised when this method is called in invalid state.</exception>
        public void GetAccessToken(string verificationCode)
        {
            if(verificationCode == null){
                throw new ArgumentNullException("verificationCode");
            }
            if(verificationCode == ""){
                throw new ArgumentException("Argument 'verificationCode' value must be specified.","verificationCode");
            }
            if(string.IsNullOrEmpty(m_RequestToken)){
                throw new InvalidOperationException("Invalid state, you need to call 'GetRequestToken' method first.");
            }
            if(!string.IsNullOrEmpty(m_AccessToken)){
                throw new InvalidOperationException("Invalid state, you have already called this 'GetAccessToken' method.");
            }

            // For more info see: http://googlecodesamples.com/oauth_playground/

            string url       = "https://www.google.com/accounts/OAuthGetAccessToken";
            string timestamp = GenerateTimeStamp();
            string nonce     = GenerateNonce();
            
            // Build signature base.
            StringBuilder xxx = new StringBuilder();
            xxx.Append("oauth_consumer_key=" + UrlEncode(m_ConsumerKey));
            xxx.Append("&oauth_nonce=" + UrlEncode(nonce));
            xxx.Append("&oauth_signature_method=" + UrlEncode("HMAC-SHA1"));
            xxx.Append("&oauth_timestamp=" + UrlEncode(timestamp));
            xxx.Append("&oauth_token=" + UrlEncode(m_RequestToken));
            xxx.Append("&oauth_verifier=" + UrlEncode(verificationCode));
            xxx.Append("&oauth_version=" + UrlEncode("1.0"));
            string signatureBase = "GET" + "&" + UrlEncode(url) + "&" +  UrlEncode(xxx.ToString());

            // Calculate signature.
            string signature = ComputeHmacSha1Signature(signatureBase,m_ConsumerSecret,m_RequestTokenSecret);
            
            //Build Authorization header.
            StringBuilder authHeader = new StringBuilder();
            authHeader.Append("Authorization: OAuth ");
            authHeader.Append("oauth_version=\"1.0\", ");
            authHeader.Append("oauth_nonce=\"" + nonce + "\", ");
            authHeader.Append("oauth_timestamp=\"" + timestamp + "\", ");
            authHeader.Append("oauth_consumer_key=\"" + m_ConsumerKey + "\", ");
            authHeader.Append("oauth_verifier=\"" + UrlEncode(verificationCode) + "\", ");
            authHeader.Append("oauth_token=\"" + UrlEncode(m_RequestToken) + "\", ");
            authHeader.Append("oauth_signature_method=\"HMAC-SHA1\", ");
            authHeader.Append("oauth_signature=\"" + UrlEncode(signature) + "\"");

            // Create web request and read response.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add(authHeader.ToString());
            using(WebResponse response = request.GetResponse()){
                using(StreamReader reader = new StreamReader(response.GetResponseStream())){
                    foreach(string parameter in HttpUtility.UrlDecode(reader.ReadToEnd()).Split('&')){
                        string[] name_value = parameter.Split('=');
                        if(string.Equals(name_value[0],"oauth_token",StringComparison.InvariantCultureIgnoreCase)){
                            m_AccessToken = name_value[1];
                        }
                        else if(string.Equals(name_value[0],"oauth_token_secret",StringComparison.InvariantCultureIgnoreCase)){
                            m_AccessTokenSecret = name_value[1];
                        }
                    }
                }
            }
        }

        #endregion

        #region method GetXOAuthStringForSmtp

        /// <summary>
        /// Gets Gmail XOAUTH authentication string.
        /// </summary>
        /// <returns>Returns Gmail XOAUTH authentication string.</returns>
        /// <exception cref="InvalidOperationException">Is raised when this method is called in invalid state.</exception>
        public string GetXOAuthStringForSmtp()
        {
            return GetXOAuthStringForSmtp(m_Email == null ? GetUserEmail() : m_Email);
        }

        /// <summary>
        /// Gets Gmail XOAUTH authentication string.
        /// </summary>
        /// <param name="email">Gmail email address.</param>
        /// <returns>Returns Gmail XOAUTH authentication string.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>email</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <exception cref="InvalidOperationException">Is raised when this method is called in invalid state.</exception>
        public string GetXOAuthStringForSmtp(string email)
        {
            if(email == null){
                throw new ArgumentNullException("email");
            }
            if(email == ""){
                throw new ArgumentException("Argument 'email' value must be specified.","email");
            }
            if(string.IsNullOrEmpty(m_AccessToken)){
                throw new InvalidOperationException("Invalid state, you need to call 'GetAccessToken' method first.");
            }

            string url       = "https://mail.google.com/mail/b/" + email + "/smtp/";
            string timestamp = GenerateTimeStamp();
            string nonce     = GenerateNonce();

            // Build signature base.
            StringBuilder xxx = new StringBuilder();
            xxx.Append("oauth_consumer_key=" + UrlEncode(m_ConsumerKey));
            xxx.Append("&oauth_nonce=" + UrlEncode(nonce));
            xxx.Append("&oauth_signature_method=" + UrlEncode("HMAC-SHA1"));
            xxx.Append("&oauth_timestamp=" + UrlEncode(timestamp));
            xxx.Append("&oauth_token=" + UrlEncode(m_AccessToken));
            xxx.Append("&oauth_version=" + UrlEncode("1.0"));
            string signatureBase = "GET" + "&" + UrlEncode(url) + "&" +  UrlEncode(xxx.ToString());

            // Calculate signature.
            string signature = ComputeHmacSha1Signature(signatureBase,m_ConsumerSecret,m_AccessTokenSecret);

            StringBuilder retVal = new StringBuilder();
            retVal.Append("GET ");
            retVal.Append(url);
            retVal.Append(" oauth_consumer_key=\"" + UrlEncode(m_ConsumerKey) + "\"");
            retVal.Append(",oauth_nonce=\"" + UrlEncode(nonce) + "\"");
            retVal.Append(",oauth_signature=\"" + UrlEncode(signature) + "\"");
            retVal.Append(",oauth_signature_method=\"" + "HMAC-SHA1\"");
            retVal.Append(",oauth_timestamp=\"" + UrlEncode(timestamp) + "\"");
            retVal.Append(",oauth_token=\"" + UrlEncode(m_AccessToken) + "\"");
            retVal.Append(",oauth_version=\"" + "1.0\"");

            return retVal.ToString();
        }

        #endregion

        #region method GetXOAuthStringForImap

        /// <summary>
        /// Gets Gmail XOAUTH authentication string.
        /// </summary>
        /// <returns>Returns Gmail XOAUTH authentication string.</returns>
        /// <exception cref="InvalidOperationException">Is raised when this method is called in invalid state.</exception>
        public string GetXOAuthStringForImap()
        {
            return GetXOAuthStringForImap(m_Email == null ? GetUserEmail() : m_Email);
        }

        /// <summary>
        /// Gets Gmail XOAUTH authentication string.
        /// </summary>
        /// <param name="email">Gmail email address.</param>
        /// <returns>Returns Gmail XOAUTH authentication string.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>email</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <exception cref="InvalidOperationException">Is raised when this method is called in invalid state.</exception>
        public string GetXOAuthStringForImap(string email)
        {
            if(email == null){
                throw new ArgumentNullException("email");
            }
            if(email == ""){
                throw new ArgumentException("Argument 'email' value must be specified.","email");
            }
            if(string.IsNullOrEmpty(m_AccessToken)){
                throw new InvalidOperationException("Invalid state, you need to call 'GetAccessToken' method first.");
            }

            string url       = "https://mail.google.com/mail/b/" + email + "/imap/";
            string timestamp = GenerateTimeStamp();
            string nonce     = GenerateNonce();

            // Build signature base.
            StringBuilder xxx = new StringBuilder();
            xxx.Append("oauth_consumer_key=" + UrlEncode(m_ConsumerKey));
            xxx.Append("&oauth_nonce=" + UrlEncode(nonce));
            xxx.Append("&oauth_signature_method=" + UrlEncode("HMAC-SHA1"));
            xxx.Append("&oauth_timestamp=" + UrlEncode(timestamp));
            xxx.Append("&oauth_token=" + UrlEncode(m_AccessToken));
            xxx.Append("&oauth_version=" + UrlEncode("1.0"));
            string signatureBase = "GET" + "&" + UrlEncode(url) + "&" +  UrlEncode(xxx.ToString());

            // Calculate signature.
            string signature = ComputeHmacSha1Signature(signatureBase,m_ConsumerSecret,m_AccessTokenSecret);

            StringBuilder retVal = new StringBuilder();
            retVal.Append("GET ");
            retVal.Append(url);
            retVal.Append(" oauth_consumer_key=\"" + UrlEncode(m_ConsumerKey) + "\"");
            retVal.Append(",oauth_nonce=\"" + UrlEncode(nonce) + "\"");
            retVal.Append(",oauth_signature=\"" + UrlEncode(signature) + "\"");
            retVal.Append(",oauth_signature_method=\"" + "HMAC-SHA1\"");
            retVal.Append(",oauth_timestamp=\"" + UrlEncode(timestamp) + "\"");
            retVal.Append(",oauth_token=\"" + UrlEncode(m_AccessToken) + "\"");
            retVal.Append(",oauth_version=\"" + "1.0\"");

            return retVal.ToString();
        }

        #endregion

        #region method GetUserEmail

        /// <summary>
        /// Gets user Gmail email address. 
        /// </summary>
        /// <returns>Returns user Gmail email address.</returns>
        /// <exception cref="InvalidOperationException">Is raised when this method is called in invalid state.</exception>
        public string GetUserEmail()
        {
            if(string.IsNullOrEmpty(m_AccessToken)){
                throw new InvalidOperationException("Invalid state, you need to call 'GetAccessToken' method first.");
            }

            string url       = "https://www.googleapis.com/userinfo/email";
            string timestamp = GenerateTimeStamp();
            string nonce     = GenerateNonce();

            // Build signature base.
            StringBuilder xxx = new StringBuilder();
            xxx.Append("oauth_consumer_key=" + UrlEncode(m_ConsumerKey));
            xxx.Append("&oauth_nonce=" + UrlEncode(nonce));
            xxx.Append("&oauth_signature_method=" + UrlEncode("HMAC-SHA1"));
            xxx.Append("&oauth_timestamp=" + UrlEncode(timestamp));
            xxx.Append("&oauth_token=" + UrlEncode(m_AccessToken));
            xxx.Append("&oauth_version=" + UrlEncode("1.0"));
            string signatureBase = "GET" + "&" + UrlEncode(url) + "&" +  UrlEncode(xxx.ToString());

            // Calculate signature.
            string signature = ComputeHmacSha1Signature(signatureBase,m_ConsumerSecret,m_AccessTokenSecret);

            //Build Authorization header.
            StringBuilder authHeader = new StringBuilder();
            authHeader.Append("Authorization: OAuth ");
            authHeader.Append("oauth_version=\"1.0\", ");
            authHeader.Append("oauth_nonce=\"" + nonce + "\", ");
            authHeader.Append("oauth_timestamp=\"" + timestamp + "\", ");
            authHeader.Append("oauth_consumer_key=\"" + m_ConsumerKey + "\", ");
            authHeader.Append("oauth_token=\"" + UrlEncode(m_AccessToken) + "\", ");
            authHeader.Append("oauth_signature_method=\"HMAC-SHA1\", ");
            authHeader.Append("oauth_signature=\"" + UrlEncode(signature) + "\"");

            // Create web request and read response.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add(authHeader.ToString());
            using(WebResponse response = request.GetResponse()){
                using(StreamReader reader = new StreamReader(response.GetResponseStream())){
                    foreach(string parameter in HttpUtility.UrlDecode(reader.ReadToEnd()).Split('&')){
                        string[] name_value = parameter.Split('=');
                        if(string.Equals(name_value[0],"email",StringComparison.InvariantCultureIgnoreCase)){
                            m_Email = name_value[1];
                        }
                    }
                }
            }

            return m_Email;
        }

        #endregion


        #region method UrlEncode

        private string UrlEncode(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            StringBuilder retVal = new StringBuilder();

            foreach (char symbol in value){
                if(unreservedChars.IndexOf(symbol) != -1){
                    retVal.Append(symbol);
                } 
                else{
                    retVal.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return retVal.ToString();
        }

        #endregion

        #region method ComputeHmacSha1Signature

        private string ComputeHmacSha1Signature(string signatureBase,string consumerSecret,string tokenSecret)
        {
            if(signatureBase == null){
                throw new ArgumentNullException("signatureBase");
            }
            if(consumerSecret == null){
                throw new ArgumentNullException("consumerSecret");
            }

            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));

            return Convert.ToBase64String(hmacsha1.ComputeHash(System.Text.Encoding.ASCII.GetBytes(signatureBase)));
        }

        #endregion

        #region method GenerateTimeStamp

        /// <summary>
        /// Creates the timestamp for the signature.        
        /// </summary>
        /// <returns></returns>
        private string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0,0);

            return Convert.ToInt64(ts.TotalSeconds).ToString();            
        }

        #endregion

        #region method GenerateNonce

        /// <summary>
        /// Creates a nonce for the signature.
        /// </summary>
        /// <returns></returns>
        private string GenerateNonce()
        {
            return m_pRandom.Next(123400, 9999999).ToString();
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets user Gmail email address. Returns null if no GetUserEmail method ever called.
        /// </summary>
        public string Email
        {
            get{ return m_Email; }
        }

        #endregion
    }
}
