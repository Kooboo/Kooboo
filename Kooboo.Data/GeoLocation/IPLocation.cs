//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Linq;
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;
using Kooboo.Lib.Security;

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

        public static void InitDatabase(bool CreateFolder = false)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(baseDirectory, "IpData");
            if (CreateFolder)
            {
                IOHelper.EnsureDirectoryExists(path);
            }

            if (Directory.Exists(path))
            {
                if (!CreateFolder)
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

        public static int GetOrAddStateId(string StateName, string CountryName)
        {
            if (StateStore == null)
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(StateName) && !string.IsNullOrWhiteSpace(CountryName))
            {
                StateName = CountryName;
            }

            string s = StateName + CountryName;
            int id = Hash.ComputeIntCaseSensitive(s);

            StateInfo stateInfo = IPLocation.StateStore.get(id);

            if (stateInfo == null)
            {
                stateInfo = new StateInfo();
                stateInfo.StateName = StateName;
                stateInfo.Country = CountryCode.ToShort(CountryName);
                stateInfo.Id = id;
                IPLocation.StateStore.add(id, stateInfo, false);
            }

            return id;

        }

        public static int GetOrAddCityId(string cityname, string state, string Country)
        {
            if (CityStore == null)
            {
                return 0;
            }
            int stateid = IPLocation.GetOrAddStateId(state, Country);

            int cityid = Hash.ComputeIntCaseSensitive(cityname);

            var cityinfo = CityStore.get(cityid);

            while (cityinfo != null)
            {
                if (cityinfo.State == stateid)
                {
                    return cityinfo.Id;
                }
                else
                {
                    cityid = Hash.ComputeIntCaseSensitive(cityid.ToString());
                    cityinfo = CityStore.get(cityid);
                }
            }


            cityinfo = new CityInfo();
            cityinfo.Id = cityid;
            cityinfo.State = stateid;
            cityinfo.CityName = cityname;

            IPLocation.CityStore.add(cityid, cityinfo, false);
            return cityid;

        }

        public static CityInfo GetCity(int cityid)
        {
            if (CityStore != null)
            {
                return CityStore.get(cityid);
            }
            return null;
        }


        public static IPViewModel GetIpCity(string ClientIp)
        {
            var intip = Lib.Helper.IPHelper.ToInt(ClientIp);
            return GetIpCity(intip);
        }

        public static IPViewModel GetIpCity(int IntIP)
        {
            if (IpCityStore != null)
            {
                var item = GetIpCityRecord(IntIP); 

                if (item != null && item.CityId != 0)
                {
                    var cityinfo = CityStore.get(item.CityId);

                    if (cityinfo != null)
                    {
                        var stateinfo = StateStore.get(cityinfo.State);

                        IPViewModel model = new IPViewModel();
                        model.Ip = Lib.Helper.IPHelper.FromInt(IntIP);

                        model.City = cityinfo.CityName;

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

            return GetIpCountry(IntIP);
        }

        public static IPViewModel GetIpCountry(string ClientIp)
        {
            var intip = Lib.Helper.IPHelper.ToInt(ClientIp);
            return GetIpCountry(intip);
        }

        public static IPViewModel GetIpCountry(int IntIP)
        {
            if (IPCountryStore != null)
            {
                var item = GetIPCountryRecord(IntIP); 

                if (item != null && item.CountryId != 0)
                {
                    var countrycode = CountryCode.FromShort(item.CountryId);

                    var countryname = CountryCode.GetCountryName(countrycode);

                    if (!string.IsNullOrWhiteSpace(countryname))
                    {
                        IPViewModel model = new IPViewModel();
                        model.Ip = Lib.Helper.IPHelper.FromInt(IntIP);

                        model.CountryCode = countrycode;
                        model.CountryName = countryname;

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
                if (last != null && last.Count() >= 1)
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

            if (IpCityStore != null)
            {
                var last = IpCityStore.Where(o => o.IpStart <= intip).OrderByDescending().Take(1);
                if (last != null && last.Count() >= 1)
                {
                    if (last[0].IpEnd >= intip)
                    {
                        return last[0];
                    }
                }
            }
            return null;

        }

        public static string GetCountryCode(string clientIP)
        {
            var model = GetIpCountry(clientIP); 

            if (model !=null)
            {
                return model.CountryCode; 
            }

            return null; 
        }

        public static void RenewDataBase()
        {
            IPLocation.InitDatabase(false);
        }
    }
}
