using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using Kooboo.Data.Context;
using Kooboo.Extensions;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Jwt
{
    public class kJwt
    {
        private RenderContext context;

        public kJwt(RenderContext context)
        {
            this.context = context;
        }

        [Description(@"
1.Config
site=>system=>settings=>JwtSetting,set jwtsecret exp and enableExp

2.Example
var token= k.security.jwt.encode({
name:'alex',
id: 'xxxx'
})

k.response.write(token)
")]
        public string Encode(IDictionary<string, object> claims)
        {
            var setting = GetJwtSetting();

            if (setting.EnableExp && !claims.ContainsKey("exp"))
            {
                var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                claims.Add("exp", setting.Exp + unixTimestamp);
            }

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(claims, setting.Secret);
        }

        [Description(@"
This method will get token in http request authorization header 

1.Config
site=>system=>settings=>JwtSetting,set jwtsecret exp and enableExp

2.Example
var result= k.security.jwt.dncode()

result: 

success
{
    code :0,
    value :{
        name:""alex"",
        id:""xxxx""
    }
}

error
{
    code :1,
    value :""error message""
}
")]
        public string Decode()
        {

            var authorizationValue = context.Request.Headers.Get("Authorization");

            if (string.IsNullOrWhiteSpace(authorizationValue))
            {
                return JsonHelper.Serialize(new
                {
                    Code = 1,
                    Value = "Not authorization header"
                });
            }

            if (!authorizationValue.StartsWith("Bearer "))
            {
                return JsonHelper.Serialize(new
                {
                    Code = 1,
                    Value = "Authorization not start with Bearer"
                });
            }

            var token = authorizationValue.Substring(7);

            return Decode(token);
        }

        [Description(@"
1.Config
site=>system=>settings=>JwtSetting,set jwtsecret exp and enableExp

2.Example
var result= k.security.jwt.dncode('eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJuYW1lIjoiaHVhbmVudCIsImV4cCI6MTYwMjIxNTM4OH0.ZunonM2w-3PJURhW9eBD90zdnw9NCDDIZbCMM6Izsb4')

result: 

success
{
    code :0,
    value :{
        name:""alex"",
        id:""xxxx""
    }
}

error
{
    code :1,
    value :""error message""
}
")]
        public string Decode(string token)
        {
            var setting = GetJwtSetting();

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                var provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                var json = decoder.Decode(token, setting.Secret, verify: true);

                return JsonHelper.Serialize(new
                {
                    Code = 0,
                    Value = JsonHelper.DeserializeObject(json)
                });
            }
            catch (TokenExpiredException)
            {
                return JsonHelper.Serialize(new
                {
                    Code = 1,
                    Value = "Token has expired"
                });
            }
            catch (SignatureVerificationException)
            {
                return JsonHelper.Serialize(new
                {
                    Code = 1,
                    Value = "Token has invalid signature"
                });
            }
        }

        public object Payload => context.Items.GetValueOrDefault("jwt_payload");

        private JwtSetting GetJwtSetting()
        {
            var setting = context.WebSite.SiteDb().CoreSetting.GetSetting<JwtSetting>();
            if (setting == null) throw new Exception("You need set JwtSetting first!");
            return setting;
        }
    }
}
