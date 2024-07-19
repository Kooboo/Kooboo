namespace Kooboo.IndexedDB
{
    public static class DBSetting
    {
        private static int _maxseq = 0;
        public static int MaxSequenceObjectSize
        {
            get
            {
                if (_maxseq == 0)
                {
                    _maxseq = 1024 * 1024 * 200;
                }
                return _maxseq;
            }
            set
            {
                _maxseq = value;
            }
        }
    }
}
