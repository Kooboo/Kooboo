using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Alipay.lib
{
    public class Config
    {
        // 应用ID,您的APPID
        //public static string app_id = "2016101800712909";

        // 支付宝网关
        //public static string gatewayUrl = "https://openapi.alipay.com/gateway.do";
       // public static string gatewayUrl = "https://openapi.alipaydev.com/gateway.do";

        // 商户私钥，您的原始格式RSA私钥
        public static string private_key = "MIIEpAIBAAKCAQEAv516Nn60iOm3Z3Eo11ShBwREgRogrF6UygLf2t9prMi9QEz1ypy1cV3tO/Kl24bPzN8TciDWaEoTJDgJ664+t/QDIJ3tIavxWd5CPUvMmq2YQmEFwWKZJrzc+lAmv3leu1Cz0SRfWcKlav4WQU0/9KMgVup0JLFhO4yx7XipwlwW8WRZf+T6cJ8jYYv0bOpZlg/YdVKSN1TWYEL/NYMpjR6v6hz1krqPLXXGzpayXXSgROnHTxmMx9TnMBV0AImqeiDn8zXNHMxMHA6pTnhNmc1fDhIOige9p8PwSkdUJ+xut5hO9z27pebumWpu/mZ9RmXLeElwEu1k2EeqagkU3wIDAQABAoIBAQC3r5Xdsb8NVMcNcawOKEGpgUzOWiaiNpT5xqGjpvIdwd2yS1SAY5OIVts50ZVviZ+grORuTs5a8U/CM7pG5SWYoGFzHhGZasQWO3tSWVyNlwZT88BZr1RTj09i4f0TJp1KgcWvIugJGxAKLgNVnGFH+izhRSYLmsM0G97hX/+UoZs0VAWUgSSOXVykV6Fa6C4tQK3QxYRDPLEhmE8eYcWyLNvXw2CZDrVoRo6wJ4hO9fQrcVAlGR6phzwTiycG4mj5W8xA79zAOq0msFo/OkE3dAocmtQ/bKZ7cZ42+HG6RSfc1/USi36anBupwpVFWQtTVZt7w00K8bNr29OPQWKZAoGBAO8unMXMl1LeMK5lcWBx2Rmk3b1qg0FrRb5p7apDBBSU+Yhv/QDa2zk4CFwS+pYZKQa8t6G8ZtAN3x1irs00iWGmh5i0gCVSYckVmwbz28Gj0mMaKaftGFfByNyoIibdNWMaJtmX1k3HrdPC0NxsZ/c0ICTTqAMBR8uJYcTLZyOLAoGBAM0Wo2ayMB6+9Q9nr5U4qw6n00SpFyZ7L7YBtmUeC1th3BnWDC74UHVy62TYjm3sigNZYvXqnHUF/QTECUAfq+dj/7YMqIDSqWesb+PDuB6BJXlZLvPk0sj8M0ci97pLMCwzhwGlX9ZSQzvUF8cHmaMFeCN7R7+XTDIuO7w3FW59AoGAMkqGxEBkgRQfAExsLm9BytaTmURQHO1FWOhFBMvai8lXPoztkcRy2/EgMNv8vJrFuZjss6E9rPl9tFwPjS5CxLmTQVSSrUZJMVLNqgSUbH7ZueTCZYDA4ZLpfRwNMkbI9vGEwbPfZ7NYpXINIIvawGvBZnzqktcuFWnpmMt8AEkCgYA2iDh2jBbDdh8PM5C6atEBGprQfO2M6+Bp0ta+2FhIuiKeNA4VRy2NrTruG67zh5vC/DeWEAX+D9OROtcC1+kLo8lQjNkZN+qXiIBP3bDNW6WtOIuFimiJzJWN6rxYKrnqZuEVDPYItOLvu5t+7oRploC1XohbZ015YlW73CG0uQKBgQCELJ1AdmrIoQY7HqEHlEnr7LMM92YBzm4/bQivEgLJ52LCShLTsmwLzYjT7iKi0qRWaVopU/yYURnJMFtw4h0OblfKbMk8E9MoqdIiUt4RKOp4xgqwN05CFcZ5Q2vUHf1epFw7j74RDSuFhIdhrnFfl+wvgCqwrpbMupxNzAOX1Q==";

        // 支付宝公钥,查看地址：https://openhome.alipay.com/platform/keyManage.htm 对应APPID下的支付宝公钥。
        //public static string alipay_public_key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAv516Nn60iOm3Z3Eo11ShBwREgRogrF6UygLf2t9prMi9QEz1ypy1cV3tO/Kl24bPzN8TciDWaEoTJDgJ664+t/QDIJ3tIavxWd5CPUvMmq2YQmEFwWKZJrzc+lAmv3leu1Cz0SRfWcKlav4WQU0/9KMgVup0JLFhO4yx7XipwlwW8WRZf+T6cJ8jYYv0bOpZlg/YdVKSN1TWYEL/NYMpjR6v6hz1krqPLXXGzpayXXSgROnHTxmMx9TnMBV0AImqeiDn8zXNHMxMHA6pTnhNmc1fDhIOige9p8PwSkdUJ+xut5hO9z27pebumWpu/mZ9RmXLeElwEu1k2EeqagkU3wIDAQAB";

        // 签名方式
        //public static string sign_type = "RSA2";

        // 编码格式
        //public static string charset = "UTF-8";
    }
}
