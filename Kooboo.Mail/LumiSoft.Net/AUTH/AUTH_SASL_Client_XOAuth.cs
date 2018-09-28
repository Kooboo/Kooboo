using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// This class implements <b>XOAUTH</b> authentication.
    /// </summary>
    public class AUTH_SASL_Client_XOAuth : AUTH_SASL_Client
    {
        private bool   m_IsCompleted = false;
        private int    m_State       = 0;
        private string m_UserName    = null;
        private string m_AuthString  = null;       

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="userName">User login name.</param>
        /// <param name="authString">OAUTH authentication string. For example you can use <see cref="AUTH_Gmail_OAuth1_3leg.GetXOAuthStringForImap()"/> to get valid string.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>userName</b> or <b>authString</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is riased when any of the arguments has invalid value.</exception>
        public AUTH_SASL_Client_XOAuth(string userName,string authString)
        {
            if(userName == null){
                throw new ArgumentNullException("userName");
            }
            if(userName == ""){
                throw new ArgumentException("Argument 'userName' value must be specified.","userName");
            }
            if(authString == null){
                throw new ArgumentNullException("authString");
            }
            if(authString == ""){
                throw new ArgumentException("Argument 'authString' value must be specified.","authString");
            }

            m_UserName   = userName;
            m_AuthString = authString;
        }


        #region method Continue

        /// <summary>
        /// Continues authentication process.
        /// </summary>
        /// <param name="serverResponse">Server sent SASL response.</param>
        /// <returns>Returns challenge request what must be sent to server or null if authentication has completed.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>serverResponse</b> is null reference.</exception>
        /// <exception cref="InvalidOperationException">Is raised when this method is called when authentication is completed.</exception>
        public override byte[] Continue(byte[] serverResponse)
        {            
            if(m_IsCompleted){
                throw new InvalidOperationException("Authentication is completed.");
            }

            if(m_State == 0){
                m_IsCompleted = true;

                return Encoding.UTF8.GetBytes(m_AuthString);
            }

            return null;
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets if the authentication exchange has completed.
        /// </summary>
        public override bool IsCompleted
        {
            get{ return m_IsCompleted; }
        }

        /// <summary>
        /// Returns always "LOGIN".
        /// </summary>
        public override string Name
        {
            get { return "XOAUTH"; }
        }

        /// <summary>
        /// Gets user login name.
        /// </summary>
        public override string UserName
        {
            get{ return m_UserName; }
        }

        /// <summary>
        /// Returns always true, because XOAUTH authentication method supports SASL client "inital response".
        /// </summary>
        public override bool SupportsInitialResponse
        {
            get{ return true; }
        }

        #endregion
    }
}
