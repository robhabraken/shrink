namespace robhabraken.SitecoreShrink.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ConfigurationHelper
    {
        public static string ReadSetting(string key)
        {
            string value = string.Empty;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                value = appSettings[key] ?? "Not found";
            }
            catch (ConfigurationErrorsException)
            {
                // do something
            }
            return value;
        }
    }
}
