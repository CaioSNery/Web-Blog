using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog
{
    public static class Configuration
    {
        public static string JwtKey { get; set; } = "mxxxxxxxxxxxxxxxxxxxxxxx";
        public static string ApiKeyName = "xxxxxxxxxxxxxxxxxxxxxxx";
        public static string ApiKey = "xxxxxxxxxxxxxxxxxxxxxxx";
        public static SmtpConfiguration Smtp = new();

        public class SmtpConfiguration
        {
            public string Host { get; set; }
            public int Port { get; set; } = 25;
            public string UserName { get; set; }
            public string Password { get; set; }


        }



    }
}