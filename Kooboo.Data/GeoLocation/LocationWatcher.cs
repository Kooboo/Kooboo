//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Data.Events.Global;

namespace Kooboo.Data.GeoLocation
{

    public class LocationStartEvent :  Events.IHandler<ApplicationStartUp>
    {
        public void Handle(ApplicationStartUp theEvent, RenderContext context)
        {
            LocationWatcher.Start(); 
        }
    }

    public static class LocationWatcher
    {
        internal static string GlobalSettingLocationKey = "Location"; 

        private static GeoCoordinateWatcher Watcher; 

        public static void Start()
        { 
            if (Watcher == null)
            {
                Watcher = new GeoCoordinateWatcher();
            } 
            Watcher.PositionChanged += Watcher_PositionChanged;

            Watcher.Start();  

            var location = GlobalDb.GlobalSetting.GetByName(GlobalSettingLocationKey);

            if (location != null && location.HasKey("latitude") && location.HasKey("longitude"))
            {
                string strla = location.GetValue("latitude");
                string strlong = location.GetValue("longitude");
                if (!string.IsNullOrEmpty(strla) && !string.IsNullOrEmpty(strlong))
                {
                    double la = 0;
                    double lon = 0;
                    if (double.TryParse(strla, out la) && double.TryParse(strlong, out lon))
                    {
                        CurrentLatitude = la;
                        CurrentLongitude = lon; 
                    }
                }
            }  
        }

        public static double CurrentLatitude = double.MinValue;
        public static double CurrentLongitude = double.MinValue;
        private static double MaxChange = 3.0; 

        private static void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            LocationChange(e.Position.Location.Latitude, e.Position.Location.Longitude); 
        }

        public static void LocationChange(double NewLatitude, double NewLongitude)
        {
            if (NewLongitude - CurrentLongitude > MaxChange || NewLatitude - CurrentLatitude > MaxChange)
            {
                CurrentLongitude = NewLongitude;
                CurrentLatitude = NewLatitude; 
                var setting = new Models.GlobalSetting() { Name = GlobalSettingLocationKey };
                setting.KeyValues["latitude"] = NewLatitude.ToString();
                setting.KeyValues["longitude"] = NewLongitude.ToString(); 
                GlobalDb.GlobalSetting.AddOrUpdate(setting); 
                AppSettings.ResetLocation(); 
            }
        }


    }
}
