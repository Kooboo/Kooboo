//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooboo.Mail.Imap.Commands
{
    public class CAPABILITY : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "CAPABILITY";
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
            return Task.FromResult(new List<ImapResponse>
            {
                new ImapResponse(ResultLine.CAPABILITY(session.Capabilities))
            });
        }
    }
}



//6.1.1.  CAPABILITY Command

//   Arguments:  none

//   Responses:  REQUIRED untagged response: CAPABILITY

//   Result:     OK - capability completed
//               BAD - command unknown or arguments invalid

//      The CAPABILITY command requests a listing of capabilities that the
//      server supports.The server MUST send a single untagged
//    CAPABILITY response with "IMAP4rev1" as one of the listed
//      capabilities before the(tagged) OK response.

//     A capability name which begins with "AUTH=" indicates that the
//     server supports that particular authentication mechanism.All
//     such names are, by definition, part of this specification.For
//     example, the authorization capability for an experimental
//      "blurdybloop" authenticator would be "AUTH=XBLURDYBLOOP" and not
//      "XAUTH=BLURDYBLOOP" or "XAUTH=XBLURDYBLOOP".


//     Other capability names refer to extensions, revisions, or
//     amendments to this specification.See the documentation of the
//     CAPABILITY response for additional information.  No capabilities,
//     beyond the base IMAP4rev1 set defined in this specification, are
//     enabled without explicit client action to invoke the capability.


//     Client and server implementations MUST implement the STARTTLS,
//     LOGINDISABLED, and AUTH = PLAIN(described in [IMAP - TLS])
//      capabilities.See the Security Considerations section for
//      important information.

//      See the section entitled "Client Commands -
//      Experimental/Expansion" for information about the form of site or
//      implementation-specific capabilities.





//Crispin Standards Track[Page 24]

//RFC 3501                         IMAPv4 March 2003


//   Example:    C: abcd CAPABILITY
//               S: * CAPABILITY IMAP4rev1 STARTTLS AUTH = GSSAPI
//               LOGINDISABLED
//               S: abcd OK CAPABILITY completed
//               C: efgh STARTTLS
//               S: efgh OK STARTLS completed
//               <TLS negotiation, further commands are under [TLS] layer>
//               C: ijkl CAPABILITY
//               S: * CAPABILITY IMAP4rev1 AUTH= GSSAPI AUTH= PLAIN
//               S: ijkl OK CAPABILITY completed