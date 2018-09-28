using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// This class implements <b>XOAUTH2</b> authentication.
    /// </summary>
    public class AUTH_SASL_Client_XOAuth2 : AUTH_SASL_Client
    {
        private bool m_IsCompleted = false;
        private int m_State = 0;
        private string m_UserName = null;
        private string m_AccessToken = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="userName">User login name.</param>
        /// <param name="accessToken">The access Token.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>userName</b> or <b>accessToken</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public AUTH_SASL_Client_XOAuth2(string userName, string accessToken)
        {
            if(userName == null){
                throw new ArgumentNullException("userName");
            }
            if(userName == ""){
                throw new ArgumentException("Argument 'userName' value must be specified.", "userName");
            }
            if(accessToken == null){
                throw new ArgumentNullException("accessToken");
            }
            if(accessToken == ""){
                throw new ArgumentException("Argument 'accessToken' value must be specified.", "accessToken");
            }

            m_UserName = userName;
            m_AccessToken = accessToken;
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

                string initialClientResponse = "user=" + m_UserName + "\u0001auth=Bearer " + m_AccessToken + "\u0001\u0001"; 

                return Encoding.UTF8.GetBytes(initialClientResponse);
            }
            else{
                return null;
            }
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
            get{ return "XOAUTH2"; }
        }

        /// <summary>
        /// Gets user login name.
        /// </summary>
        public override string UserName
        {
            get{ return m_UserName; }
        }

        /// <summary>
        /// Returns always true, because XOAUTH2 authentication method supports SASL client "inital response".
        /// </summary>
        public override bool SupportsInitialResponse
        {
            get{ return true; }
        }

        #endregion
    }
}
