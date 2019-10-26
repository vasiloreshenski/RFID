namespace RFID.REST.Test.Common
{
    using RFID.REST.Areas.Auth.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;


    internal static class Settings
    {
        public static AuthSettings GetDevelopmentAuthSettings()
        {
            return new AuthSettings(
                secret: "51cc364e-9ace-4745-84c5-410b0ba06dad",
                issuer: "rfid",
                audience: "rfid",
                accessExpiration: TimeSpan.FromMinutes(30),
                refreshExpiration: TimeSpan.FromMinutes(60)
            );
        }

        public static String GetDevelopmentConnectionString()
        {
            return "data source=192.168.0.105,1433; initial catalog=Rfid; user id=sa; password=P@ssw0rd";
        }
    }
}
