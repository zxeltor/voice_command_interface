using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTrekNut.VoiceCom.Lib.Helpers
{
    public static class WebVersionHelper
    {
        public static readonly string _projectUrl = @"http://starfleet.engineer/projects";

        private static System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"VCI Latest Version: (\d\.\d\.\d)");

        public static Version GetLatestVersionFromUrl()
        {
            using (var client = new System.Net.WebClient())
            {
                Version versionResult = null;
                var webResult = client.DownloadString(_projectUrl);

                if(regex.IsMatch(webResult))
                {
                    var versionString = regex.Match(webResult).Groups[1].Value;
                    Version.TryParse(versionString, out versionResult);
                }

                return versionResult;
            }
        }
    }
}
