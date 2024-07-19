namespace Kooboo.Mail.MassMailing
{
    public class HeaderScanner
    {

        private string chars;
        private int _readIndex;
        private int _length;

        public HeaderScanner(string RawMsg)
        {
            chars = RawMsg;
            _length = chars.Length;
            _readIndex = 0;
        }


        // line include staring \r\n, which is fine.
        public LineResult ConsumeLine()
        {
            if (_readIndex >= _length)
            {
                return null;
            }

            string line = null;

            int start = 0;
            int end = 0;

            if (_readIndex + 2 < _length)
            {
                for (int i = _readIndex; i < _length; i++)
                {
                    if (chars[i] == '\r' && chars[i + 1] == '\n')
                    {

                        if ((i + 1) >= _length && (chars[i + 2] == ' ' || chars[i + 2] == '\t'))
                        {
                            // should unfolder. 
                            line += chars.Substring(_readIndex, i - _readIndex).Trim();
                            start = i;
                            i = i + 3;
                            _readIndex = i;
                            // continue;
                        }
                        else
                        {
                            if (i > _readIndex)
                            {
                                line += chars.Substring(_readIndex, i - _readIndex).Trim();

                                if (start == 0)
                                {
                                    start = _readIndex;
                                }

                                _readIndex = i + 2;  // i = \r, +1 = \n. +2 = new start. 
                                end = i + 1;
                                return new LineResult() { Value = line, Start = start, End = end };
                            }
                            else
                            {
                                // this should not be possible
                            }
                        }
                    }
                }

            }


            if (_readIndex < _length)
            {
                var lastLine = chars.Substring(_readIndex, _length - _readIndex);

                LineResult result = new LineResult() { Value = lastLine.Trim(), Start = _readIndex, End = _length - 1 };
                _readIndex = _length;

                return result;
            }

            return null;
        }


    }


}
