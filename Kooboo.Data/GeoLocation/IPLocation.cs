//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;
using Kooboo.Lib.Security;
using System;
using System.IO;
using System.Linq;

namespace Kooboo.Data.GeoLocation
{
    public static class IPLocation
    {
        static IPLocation()
        {
            IPLocation.InitDatabase(false);
        }

        public static ObjectStore<int, CityInfo> CityStore { get; set; }

        public static ObjectStore<int, StateInfo> StateStore { get; set; }

        public static ObjectStore<int, IPCity> IpCityStore { get; set; }

        public static ObjectStore<int, IPCountry> IPCountryStore { get; set; }

        public static Database Database { get; set; }

        public static void InitDatabase(bool createFolder = false)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(baseDirectory, "IpData");
            if (createFolder)
            {
                IOHelper.EnsureDirectoryExists(path);
            }

            if (Directory.Exists(path))
            {
                if (!createFolder)
                {
                    string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                    if (files == null || files.Length == 0)
                    {
                        return;
                    }
                }

                if (Database == null)
                {
                    Database = new Database(path);
                }

                if (CityStore == null)
                {
                    ObjectStoreParameters paras = new ObjectStoreParameters();
                    paras.SetPrimaryKeyField<CityInfo>(o => o.Id);
                    paras.EnableLog = false;
                    paras.EnableVersion = false;

                    CityStore = Database.GetOrCreateObjectStore<int, CityInfo>("City", paras);
                }

                if (StateStore == null)
                {
                    ObjectStoreParameters parastate = new ObjectStoreParameters();
                    parastate.SetPrimaryKeyField<StateInfo>(o => o.Id);
                    parastate.EnableVersion = false;
                    parastate.EnableLog = false;

                    StateStore = Database.GetOrCreateObjectStore<int, StateInfo>("State", parastate);
                }

                if (IpCityStore == null)
                {
                    ObjectStoreParameters paracity = new ObjectStoreParameters();
                    paracity.SetPrimaryKeyField<IPCity>(o => o.IpStart);
                    paracity.AddColumn<IPCity>(o => o.IpEnd);
                    paracity.EnableLog = false;
                    paracity.EnableVersion = false;

                    IpCityStore = Database.GetOrCreateObjectStore<int, IPCity>("IpCity", paracity);
                }

                if (IPCountryStore == null)
                {
                    ObjectStoreParameters paracountry = new ObjectStoreParameters();
                    paracountry.SetPrimaryKeyField<IPCity>(o => o.IpStart);
                    paracountry.AddColumn<IPCity>(o => o.IpEnd);
                    paracountry.EnableVersion = false;
                    paracountry.EnableLog = false;

                    IPCountryStore = Database.GetOrCreateObjectStore<int, IPCountry>("IpCountry", paracountry);
                }
            }
        }

        public static int GetOrAddStateId(string stateName, string countryName)
        {
            if (StateStore == null)
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(stateName) && !string.IsNullOrWhiteSpace(countryName))
            {
                stateName = countryName;
            }

            string s = stateName + countryName;
            int id = Hash.ComputeIntCaseSensitive(s);

            StateInfo stateInfo = IPLocation.StateStore.get(id);

            if (stateInfo == null)
            {
                stateInfo = new StateInfo {StateName = stateName, Country = CountryCode.ToShort(countryName), Id = id};
                StateStore.add(id, stateInfo, false);
            }

            return id;
        }

        public static int GetOrAddCityId(string cityname, string state, string country)
        {
            if (CityStore == null)
            {
                return 0;
            }
            int stateid = IPLocation.GetOrAddStateId(state, country);

            int cityid = Hash.ComputeIntCaseSensitive(cityname);

            var cityinfo = CityStore.get(cityid);

            while (cityinfo != null)
            {
                if (cityinfo.State == stateid)
                {
                    return cityinfo.Id;
                }

                cityid = Hash.ComputeIntCaseSensitive(cityid.ToString());
                cityinfo = CityStore.get(cityid);
            }

            cityinfo = new CityInfo {Id = cityid, State = stateid, CityName = cityname};

            IPLocation.CityStore.add(cityid, cityinfo, false);
            return cityid;
        }

        public static CityInfo GetCity(int cityid)
        {
            return CityStore?.get(cityid);
        }

        public static IPViewModel GetIpCity(string clientIp)
        {
            var intip = Lib.Helper.IPHelper.ToInt(clientIp);
            return GetIpCity(intip);
        }

        public static IPViewModel GetIpCity(int intIp)
        {
            if (IpCityStore != null)
            {
                var item = GetIpCityRecord(intIp);

                if (item != null && item.CityId != 0)
                {
                    var cityinfo = CityStore.get(item.CityId);

                    if (cityinfo != null)
                    {
                        var stateinfo = StateStore.get(cityinfo.State);

                        IPViewModel model = new IPViewModel
                        {
                            Ip = Lib.Helper.IPHelper.FromInt(intIp), City = cityinfo.CityName
                        };


                        if (stateinfo != null)
                        {
                            model.State = stateinfo.StateName;
                            model.CountryCode = CountryCode.FromShort(stateinfo.Country);
                            model.CountryName = CountryCode.GetCountryName(stateinfo.Country);
                        }

                        return model;
                    }
                }
            }

            return GetIpCountry(intIp);
        }

        public static IPViewModel GetIpCountry(string clientIp)
        {
            var intip = Lib.Helper.IPHelper.ToInt(clientIp);
            return GetIpCountry(intip);
        }

        public static IPViewModel GetIpCountry(int intIp)
        {
            if (IPCountryStore != null)
            {
                var item = GetIPCountryRecord(intIp);

                if (item != null && item.CountryId != 0)
                {
                    var countrycode = CountryCode.FromShort(item.CountryId);

                    var countryname = CountryCode.GetCountryName(countrycode);

                    if (!string.IsNullOrWhiteSpace(countryname))
                    {
                        IPViewModel model = new IPViewModel
                        {
                            Ip = Lib.Helper.IPHelper.FromInt(intIp),
                            CountryCode = countrycode,
                            CountryName = countryname
                        };


                        return model;
                    }
                }
            }

            return null;
        }

        private static IPCountry GetIPCountryRecord(int intip)
        {
            if (IPCountryStore != null)
            {
                var last = IPCountryStore.Where(o => o.IpStart <= intip).OrderByDescending().Take(1);
                if (last != null && last.Count >= 1)
                {
                    if (last[0].IpEnd >= intip)
                    {
                        return last[0];
                    }
                }
            }
            return null;
        }

        private static IPCity GetIpCityRecord(int intip)
        {
            //var item = IpCityStore.Where(o => o.IpStart <= IntIP && o.IpEnd >= IntIP).OrderByDescending().FirstOrDefault();

            var last = IpCityStore?.Where(o => o.IpStart <= intip).OrderByDescending().Take(1);
            if (last != null && last.Count >= 1)
            {
                if (last[0].IpEnd >= intip)
                {
                    return last[0];
                }
            }
            return null;
        }

        public static string GetCountryCode(string clientIP)
        {
            var model = GetIpCountry(clientIP);

            return model?.CountryCode;
        }

        public static void RenewDataBase()
        {
            InitDatabase(false);
        }
    }
}