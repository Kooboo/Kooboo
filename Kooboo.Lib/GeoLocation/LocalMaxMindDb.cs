using System;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;

namespace Kooboo.ShareInfo.GeoLocation
{
    public class LocalMaxMindDb
    {
        public static LocalMaxMindDb instance = new LocalMaxMindDb();
        public LocalMaxMindDb()
        {

        }
        private string _cityFile;
        public string IpCityFileName
        {
            get
            {
                if (_cityFile == null)
                {
                    _cityFile = searchDb("city");
                }
                return _cityFile;
            }
            set { _cityFile = value; }
        }

        private string _countryFile;
        public string IpCountryFileName
        {
            get
            {
                if (_countryFile == null)
                {
                    _countryFile = searchDb("country");
                }
                return _countryFile;
            }
            set { _countryFile = value; }
        }

        private string searchDb(string containsPart)
        {
            var files = System.IO.Directory.GetFiles(DataFolder, "*.mmdb");
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(file))
                    {
                        var info = new System.IO.FileInfo(file);
                        if (info.Name.ToLower().Contains(containsPart))
                        {
                            return info.FullName;
                        }
                    }
                }
            }
            return null;
        }


        public string DataFolder
        {
            get
            {
                var root = AppDomain.CurrentDomain.BaseDirectory;

                var folder = System.IO.Path.Combine(root, "AppData");
                folder = System.IO.Path.Combine(folder, "IpData");

                Lib.Helper.IOHelper.EnsureDirectoryExists(folder);
                return folder;
            }
        }

        private object _locker = new object();

        public CityResponse ReadCity(string IP)
        {
            if (CityReader == null)
            {
                return null;
            }

            if (CityReader.TryCity(IP, out CityResponse cityResponse))
            {
                return cityResponse;
            }
            return null;
        }


        private object locker = new object();
        private DatabaseReader _CityReader;
        public DatabaseReader CityReader
        {
            get
            {
                if (_CityReader == null)
                {
                    lock (locker)
                    {
                        if (!string.IsNullOrEmpty(IpCityFileName))
                        {
                            var reader = getReader(IpCityFileName);
                            _CityReader = reader;
                        }

                    }
                }
                return _CityReader;
            }
        }


        private DatabaseReader _countryReader;
        public DatabaseReader CountryReader
        {
            get
            {
                if (_countryReader == null)
                {
                    lock (locker)
                    {
                        if (!string.IsNullOrEmpty(IpCountryFileName))
                        {
                            var reader = getReader(IpCountryFileName);
                            _countryReader = reader;
                        }

                    }
                }
                return _countryReader;
            }
        }

        private DatabaseReader getReader(string fullFilename)
        {
            if (string.IsNullOrEmpty(fullFilename))
            {
                return null;
            }

            try
            {
                var reader = new DatabaseReader(fullFilename);
                if (reader != null)
                {
                    return reader;
                }
            }
            catch (Exception)
            {
            }

            try
            {
                var reader = new DatabaseReader(fullFilename, MaxMind.Db.FileAccessMode.Memory);
                return reader;
            }
            catch (Exception)
            {
            }
            return null;
        }


        public CountryResponse ReadCountry(string IP)
        {
            if (CountryReader == null)
            {
                return null;
            }

            if (CountryReader.TryCountry(IP, out CountryResponse res))
            {
                return res;
            }
            return null;
        }

    }
}
