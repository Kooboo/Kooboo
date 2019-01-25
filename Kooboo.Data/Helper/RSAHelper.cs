//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Helper
{

    // NOTE: this class should only be used by account project to return keys to client request. 
  public static class RSAHelper
    {

        public static int MinEncryptedLenth = 1024 / 8; 

        public static string LatestPublickey
        {
            get
            {
                // TODO: get out from update folder.  
                return @"<RSAKeyValue><Modulus>lz3PVfCAOcWx89KbDBABjF1YZZFHs+eEHmh9ElkBHkS2kt1R1s5h6fIAq9bj/LABXK49QfeirihMPMjW6zNtuBxFt2WsrSuwMbiCG7eNm7uhbYVNifevf+nLjFXo9kYcX6+CKDzID7AutSQ6OMjCC4khh/q8M9j0abJpoMJfAy3gnvAcPdfTSQii7OsYi9YbMGqV965dQWj+JdfgCzkSvoLzKaHl+L9Ur5p0C5zcW5eBBfgUfRXi9ukQ8UaFw1z5zsAyyi+13MGeliqV49vMtDwmAJ4113jXTcx1xMCzyx+GoXN31YaMKbCf4x/6XExfV4kJvJSeaZjLRn/zwto3Kw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            }
        }
         
        public static List<string> PrivateKeys
        {
            get
            {
                // TODO: generate from update folder, in the client site, this must be null. 
                List<string> result = new List<string>();

                string last = @"<RSAKeyValue><Modulus>lz3PVfCAOcWx89KbDBABjF1YZZFHs+eEHmh9ElkBHkS2kt1R1s5h6fIAq9bj/LABXK49QfeirihMPMjW6zNtuBxFt2WsrSuwMbiCG7eNm7uhbYVNifevf+nLjFXo9kYcX6+CKDzID7AutSQ6OMjCC4khh/q8M9j0abJpoMJfAy3gnvAcPdfTSQii7OsYi9YbMGqV965dQWj+JdfgCzkSvoLzKaHl+L9Ur5p0C5zcW5eBBfgUfRXi9ukQ8UaFw1z5zsAyyi+13MGeliqV49vMtDwmAJ4113jXTcx1xMCzyx+GoXN31YaMKbCf4x/6XExfV4kJvJSeaZjLRn/zwto3Kw==</Modulus><Exponent>AQAB</Exponent><P>y4EKwxASWFJ8SmgvLLfBJW6RCbHOCEEazgllFD9kUXpgmY8jYwh646MTHzs8GHWv/XOix+BXBoglGWAF3NOQ7VNrC6ecK8dPg6lg/gb5wIa742KGfSiq3vZ46IM74y5AG8mg0aQ2fKAQk7gzguCbRRwIm96lKP7SmVo4hWyuM6k=</P><Q>vkF1uX7lD8GLaQp4QAh0SN97sNFkV45R7rkbBj/mGJJDZOVA9oc7cXVjZ2L6xGnU37yB1b/ao5Kqp1JMtfimfKpTGPtEN84w1bYdMZxswzPwH33c929cOJnfNa4vJSVhHlz6kEreRr2v195MT9adwZfAbjAD3sS8lui8SRc6WLM=</Q><DP>OXCI2xn0M53EmzPg7NxI1gnpQjU/lDDcIf+g2iAybpT+Ixm1wUQpe4sR4KTVh4ngTMeQ0J3PsnZHEmx5+yR0Kah79GQwvmKZXO44BDtvzxm4eqvajgUmhZxH8EjqZGsviShWDhYtkuuTAj7huzUuXklVXLlxIKscQlCm13IfkVE=</DP><DQ>tYDJiK9wY3mnQAyG77+hACyEWglJCuZxb6cUCwBGyUYwGT9EnGwi33i33lflGCBGvzOdQYgIU2iGeZ3gJPIUEnaDVB/R0tNYb2mPrFaoxAj2mXR/q2aHuk/BS/riazOf1VGqkKF8MEyiRvSCJOCAG8JkCDqyVoGl0OozRh9swFU=</DQ><InverseQ>vp7I651ensQDReea8eQ3LygbJFsuOLaHyGmOUPSSSsuT8wBcATD13qeYtLqfqS0bwyp9oKchGkKEACvp6YbIdXJpdpmTGp7VQMsNmyrjw3NsG/M35MQ3lGEBcmAA/QT59IaG/i0mXrGRYeQ7ZjWxfuw4jm0XG/rL5cFNDcT4+DE=</InverseQ><D>GAOJQV4o9e4FGy+UTRODiMEk29JNU2w4sZ1UnTkC8PE8bfr0gh9jM5f9xvXICGNSgV/sZmYxRJGDHV75pP8VoOyNne49FIKXJ3s3iWVud3eQzPN5WXqO4/KqzkhbxN1ipvIWlVilSWcSMNJ6Phw+Qez5pSjI1jtQqjGuYjH1OmtBYB8xzby2Dlns1vnEtCskbY2KiVHLRFx8bg94fzHwmMwhCF/9RIuDDav3iG3FZZ1jZHxUPK7YtQsojykBKdXNzS6+3DtU10SD/rIxSsh0mE+XDDMtY5X7jv4t45JwNyPCH/qMa6UjGqKJTftbSsjUw6yk0P19qN+7PGeiYn6maQ==</D></RSAKeyValue>";

                result.Add(last);

                return result;
            } 

        }

        public static string Encrypt(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            return Lib.Security.RSAEncryption.RSAEncrypt(LatestPublickey, input); 
        }

        public static string Decrypt(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null; 
            }

            foreach (var item in PrivateKeys)
            {
                string result = null;
                try
                {
                    result = Lib.Security.RSAEncryption.RSADecrypt(item, input); 
                }
                catch (Exception)
                { 
                }   

                if (!string.IsNullOrEmpty(result))
                {
                    return result; 
                }
            }
            return null; 
        }      

    }
}
