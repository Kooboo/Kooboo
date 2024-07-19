//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Kooboo.Mail.Factory;
using Kooboo.Mail.Utility;

namespace Kooboo.Mail.Imap.Commands
{
    public class LOGIN : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "LOGIN";
            }
        }

        public bool RequireAuth
        {
            get
            {
                return false;
            }
        }

        public bool RequireFolder
        {
            get
            {
                return false;
            }
        }

        public bool RequireTwoPartCommand
        {
            get
            {
                return false;
            }
        }

        public Task<List<ImapResponse>> Execute(ImapSession session, string args)
        {
            EmailAddress emailAddress = null;
            if (string.IsNullOrEmpty(args))
            {
                throw new Exception("User Or Password not provided");
            }

            string[] user_pass = TextUtils.SplitQuotedString(args, ' ', true);
            if (user_pass.Length != 2)
            {
                throw new Exception("Error in arguments");
            }

            Guid OrganizationId = default(Guid);    // user may have multiple orgs. 

            string username = user_pass[0];

            if (username.IndexOf("@") > -1 && Utility.AddressUtility.IsValidEmailAddress(username))
            {
                var emailadd = Utility.AddressUtility.GetEmailAddress(username);

                if (emailadd == null)
                {
                    throw new Exception("LOGIN Failed");
                }
                else
                {
                    username = Data.GlobalDb.Users.GetUserName(emailadd.UserId);
                    OrganizationId = emailadd.OrgId;
                }

                emailAddress = emailadd;
            }
            else
            {
                throw new Exception("Login user must be an email address");
            }


            var orgdb = DBFactory.OrgDb(OrganizationId);
            var validate = orgdb.Email.LoginByAuthorizationCode(emailAddress.Address, user_pass[1]);
            var user = Data.GlobalDb.Users.Get(emailAddress.UserId);

            if (!validate)
            {
                throw new Exception("LOGIN Failed");
            }
            session.AuthenticatedUserIdentity = new GenericIdentity(user.UserName, "IMAP-LOGIN");

            session.MailDb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user.Id, OrganizationId);

            return this.NullResult();
        }
    }
}



//6.2.3.  LOGIN Command

//   Arguments:  user name
//               password

//   Responses:  no specific responses for this command

//   Result:     OK - login completed, now in authenticated state
//               NO - login failure: user name or password rejected
//               BAD - command unknown or arguments invalid

//      The LOGIN command identifies the client to the server and carries
//      the plaintext password authenticating this user.






//Crispin Standards Track[Page 30]

//RFC 3501                         IMAPv4 March 2003


//      A server MAY include a CAPABILITY response code in the tagged OK
//      response to a successful LOGIN command in order to send
//      capabilities automatically.It is unnecessary for a client to
//      send a separate CAPABILITY command if it recognizes these
//      automatic capabilities.

//   Example:    C: a001 LOGIN SMITH SESAME
//               S: a001 OK LOGIN completed

//        Note: Use of the LOGIN command over an insecure network
//        (such as the Internet) is a security risk, because anyone
//        monitoring network traffic can obtain plaintext passwords.
//        The LOGIN command SHOULD NOT be used except as a last
//        resort, and it is recommended that client implementations
//        have a means to disable any automatic use of the LOGIN
//        command.

//        Unless either the STARTTLS command has been negotiated or
//        some other mechanism that protects the session from
//        password snooping has been provided, a server
//        implementation MUST implement a configuration in which it
//        advertises the LOGINDISABLED capability and does NOT permit
//        the LOGIN command.  Server sites SHOULD NOT use any
//        configuration which permits the LOGIN command without such
//        a protection mechanism against password snooping.  A client
//        implementation MUST NOT send a LOGIN command if the
//        LOGINDISABLED capability is advertised.