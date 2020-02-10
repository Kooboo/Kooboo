using System;
using System.Collections.Generic;
using System.Web;
using WxPayAPI.lib;

namespace WxPayAPI
{
    /**
    * 	配置账号信息
    */
    public class WxPayConfig
    {

        private static volatile IConfig config;
        private static object syncRoot = new object();

        public static IConfig GetConfig(){
            if(config==null){
                lock(syncRoot){
                    if (config == null)
                        config = new  DefaultConfig();
                }
            }
            return config;
        } 

        

    }
}