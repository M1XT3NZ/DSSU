using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSU.SettingsAndHelpers
{
    public class Settings
    {
        public static string SteamAPIKEY { get; set; } = "YOUR_API_KEY_HERE";
        public static string DiscordToken { get; set; } = "YOUR_TOKEN_HERE";
        public static int HelpCommandTimeout { get; set; } = 20;//20 minutes
        public static int ServerCommandTimeout { get; set; } = 5;//5 minutes
    }
}