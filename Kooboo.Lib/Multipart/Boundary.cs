//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Lib.NETMultiplePart
{
    public static class Boundary
    {

        public static string GetBoundary(byte[] input)
        {
            // ------WebKitFormBoundaryIgQAzBmhPGnrftlQ
            //Content - Disposition: form - data; name = "file_0"; filename = "test.css"
            //Content - Type: text / css 

            //test
            //------WebKitFormBoundaryIgQAzBmhPGnrftlQ--
            int len = input.Length;

            for (int i = 0; i < input.Length; i++)
            {
                if (i > len - 3)
                {
                    return null;
                }

                if (input[i] == 13)
                { return null; }

                if (input[i] == 45 && input[i + 1] == 45)
                {
                    for (int j = i + 2; j < len - 2; j++)
                    {
                        if (input[j] == 13 && input[j + 1] == 10)
                        {
                            int bytelen = j - i - 2;
                            return System.Text.Encoding.ASCII.GetString(input, i + 2, bytelen);
                        }
                    }
                }

            }

            return null;

        }

    }
}
